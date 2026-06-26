let servantModal;
let servantEditModal;
let allServants = [];

$(async function () {
    servantModal = new bootstrap.Modal(document.getElementById('servantModal'));
    servantEditModal = new bootstrap.Modal(document.getElementById('servantEditModal'));
    await loadServants();
    await loadRoles();
    await loadClassesDropDown(apiBaseUrl, 'drpServantClasses', '#servantModal', true);
    await loadClassesDropDown(apiBaseUrl, 'drpEditServantClasses', '#servantEditModal', true);

    

    // =========================================
    // PASSWORD COMPLEXITY
    // =========================================
    $.validator.addMethod("passwordComplexity", function (value, element) {
        const passwordRegex =
            /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@@\$!%*?&])[A-Za-z\d@@\$!%*?&]{8,}$/;
        return this.optional(element)
            || passwordRegex.test(value);
    }, "Password must be at least 8 characters, with 1 uppercase, 1 lowercase, 1 number, and 1 special character.");
    // =========================================
    // VALIDATOR
    // =========================================
    $("#servantForm").validate({
        ignore: [],
        errorClass: "is-invalid",
        validClass: "is-valid",
        errorElement: "div",
        rules: {
            ServantName: "required",
            Email: {
                required: true,
                email: true, // Built-in email format validation
                remote: {   // API validation for existing email
                    url: `${apiBaseUrl}/Servants/CheckEmail`,
                    type: "get",
                    xhrFields: { withCredentials: true },
                    data: {
                        email: function () { return $("#Email").val(); }
                    }
                }
            },
            Password: {
                required: true,
                passwordComplexity: true // Our custom rule
            },
            ConfirmPassword: {
                required: true,
                equalTo: "#Password" // Built-in rule to match password
            },
            Mobile: "required",
            drpRoles: "required"
        },
        messages: {
            ServantName: "Servant name is required.",
            Email: {
                required: "E-mail is required.",
                email: "Please enter a valid e-mail address.",
                remote: "This email address is already in use." // Message for API check
            },
            Password: {
                required: "Password is required."
                // The complexity message is defined in addMethod
            },
            ConfirmPassword: {
                required: "Please confirm your password.",
                equalTo: "Passwords do not match."
            },
            Mobile: "Mobile number is required.",
            drpRoles: "Please select at least one role."
        },
        // =========================================
        // STYLING
        // =========================================
        highlight: function (element) {
            $(element).addClass('is-invalid');
            if ($(element).hasClass('form-select')) {
                $(element)
                    .next('.select2-container')
                    .find('.select2-selection')
                    .addClass('is-invalid');
            }
        },
        unhighlight: function (element) {
            $(element).removeClass('is-invalid');
            if ($(element).hasClass('form-select')) {
                $(element)
                    .next('.select2-container')
                    .find('.select2-selection')
                    .removeClass('is-invalid');
            }
        },
        errorPlacement: function (error, element) {
            error.addClass("invalid-feedback");
            if (element.next('.select2-container').length) {
                error.insertAfter(
                    element.next('.select2-container')
                );
            }
            else {
                error.insertAfter(element);
            }
        },
        // =========================================
        // SUBMIT
        // =========================================
        submitHandler: async function (form, event) {
            event.preventDefault();
            await saveServant();
        }
    });


    $("#servantEditForm").validate({
        ignore: [],
        errorClass: "is-invalid",
        validClass: "is-valid",
        errorElement: "div",
        rules: {
            ServantName: "required",
            Mobile: "required",
            drpEditRoles: "required"
        },
        messages: {
            ServantName: "Servant name is required.",
            Mobile: "Mobile number is required.",
            drpEditRoles: "Please select at least one role."
        },
        // =========================================
        // STYLING
        // =========================================
        highlight: function (element) {
            $(element).addClass('is-invalid');
            if ($(element).hasClass('form-select')) {
                $(element)
                    .next('.select2-container')
                    .find('.select2-selection')
                    .addClass('is-invalid');
            }
        },
        unhighlight: function (element) {
            $(element).removeClass('is-invalid');
            if ($(element).hasClass('form-select')) {
                $(element)
                    .next('.select2-container')
                    .find('.select2-selection')
                    .removeClass('is-invalid');
            }
        },
        errorPlacement: function (error, element) {
            error.addClass("invalid-feedback");
            if (element.next('.select2-container').length) {
                error.insertAfter(
                    element.next('.select2-container')
                );
            }
            else {
                error.insertAfter(element);
            }
        },
        // =========================================
        // SUBMIT
        // =========================================
        submitHandler: async function (form, event) {
            event.preventDefault();
            await editServant();
        }
    });
});
// ============================================
// LOAD ROLES
// ============================================
async function loadRoles() {
    try {
        const request =
            `${apiBaseUrl}/Servants/GetSystemRoles`;
        const response = await fetch(request, {
            method: "GET",
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            }
        });
        if (!response.ok) {
            throw new Error("Failed to fetch roles.");
        }
        const roles = await response.json();
        const dropdown = document.getElementById("drpRoles");
        dropdown.innerHTML = "";
        roles.forEach(role => {
            const option = document.createElement("option");
            option.value = role.id;
            option.textContent = role.name;
            dropdown.appendChild(option);
        });
        $('#drpRoles').select2({
            placeholder: "Select roles",
            allowClear: true,
            width: '100%',
            dropdownParent: $('#servantModal')
        });

        const dropdownEdit = document.getElementById("drpEditRoles");
        dropdownEdit.innerHTML = "";
        roles.forEach(role => {
            const option = document.createElement("option");
            option.value = role.id;
            option.textContent = role.name;
            dropdownEdit.appendChild(option);
        });
        $('#drpEditRoles').select2({
            placeholder: "Select roles",
            allowClear: true,
            width: '100%',
            dropdownParent: $('#servantEditModal')
        });
    }
    catch (err) {
        console.error("Error loading roles:", err);
    }
}
// ============================================
// LOAD SERVANTS
// ============================================
async function loadServants() {
    showLoading();
    try {
        const request =
            `${apiBaseUrl}/Servants/get-servants`;
        const response = await fetch(request, {
            method: "GET",
            credentials: "include"
        });
        if (!response.ok) {
            throw new Error("Failed to fetch servants.");
        }
        allServants = await response.json();
        filterServants();
    }
    catch (err) {
        console.error(err);
    }
    finally {
        hideLoading();
    }
}
// ============================================
// FILTER
// ============================================
function filterServants() {
    const name =
        $('#filterServantName')
            .val()
            .toLowerCase();
    const status =
        $('#filterStatus').val();
    const filtered = allServants.filter(s => {
        const matchesName = s.servantName.toLowerCase().includes(name);
        const matchesStatus = status === 'all' || s.isActive.toString() === status;
        return matchesName && matchesStatus;
    });
    renderServants(filtered);
    $('#servant-counter')
        .text(`(${filtered.length})`);
}
// ============================================
// RENDER GRID
// ============================================
function renderServants(servants) {
    const grid =
        document.getElementById("servant-grid");
    grid.innerHTML = '';
    if (servants.length === 0) {
        grid.innerHTML =
            '<div class="col-12 text-center py-5 text-muted">No servants found.</div>';
        return;
    }
    servants.forEach(servant => {
        const col =
            document.createElement("div");
        col.className =
            "col-12 col-md-6 col-lg-4";
        col.innerHTML = `
            <div class="card h-100 shadow-sm border-0">
                <div class="card-body">
                    <div class="d-flex justify-content-between align-items-start mb-3">
                        <h5 class="card-title fw-bold text-primary mb-0">
                            ${servant.servantName}
                        </h5>
                        <div class="d-flex align-items-center gap-2">
                            <span class="badge ${servant.isActive ? 'bg-success-subtle text-success border border-success-subtle' : 'bg-secondary-subtle text-secondary border border-secondary-subtle'} px-3">${servant.isActive ? 'Active' : 'Inactive'}
                            </span>
                            <div class="dropdown">
                                <button class="btn btn-link btn-sm text-dark p-0"
                                        data-bs-toggle="dropdown">
                                    <i class="bi bi-three-dots-vertical"></i>
                                </button>
                                <ul class="dropdown-menu dropdown-menu-end shadow-sm">
                                    <li>
                                        <button class="dropdown-item" onclick="openEditModal(${servant.servantID})">
                                            <i class="bi bi-pencil me-2"></i>
                                            Edit
                                        </button>
                                    </li>
                                    <li>
                                        <button class="dropdown-item text-danger" onclick="deleteServant(${servant.servantID}, '${servant.servantName}')">
                                            <i class="bi bi-trash me-2"></i>
                                            Delete
                                        </button>
                                    </li>
                                </ul>
                            </div>
                        </div>
                    </div>
                    <div class="small text-muted mb-2"><i class="bi bi-envelope me-2"></i>${servant.email ?? '-'}
                    </div>
                    <div class="small text-muted"><i class="bi bi-telephone me-2"></i>${servant.mobile1 ?? '-'}
                    </div>
                </div>
            </div>
        `;
        grid.appendChild(col);
    });
}
// ============================================
// CREATE MODAL
// ============================================
function openCreateModal() {
    $('#servantForm')[0].reset();
    $('#ServantID').val('');
    $('#drpServantClasses').val(null).trigger('change');
    $('#drpRoles').val(null).trigger('change');
    $('#Email').prop('disabled', false);
    $('#isActive').prop('checked', true);

    servantModal.show();
}
// ============================================
// EDIT MODAL
// ============================================
async function openEditModal(servantID) {
    try {
        showLoading();
        const request = `${apiBaseUrl}/Servants/GetServant?servantID=${servantID}`;
        const response = await fetch(request, {
            method: "GET",
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            }
        });
        if (!response.ok) {
            throw new Error("Failed loading servant");
        }
        const servant = await response.json();
        $('#ServantID').val(servant.servantID);
        $('#EditServantName').val(servant.servantName);
        $('#EditEmail').val(servant.email);
        $('#EditMobile').val(servant.mobile1);
        $('#EditMobile2').val(servant.mobile2);
        $('#EditIsActive').prop('checked', servant.isActive);
        if (servant.servantClassesIDs) {
            $('#drpEditServantClasses').val(servant.servantClassesIDs).trigger('change');
        }
        if (servant.servantRoles) {
            $('#drpEditRoles').val(servant.servantRoles).trigger('change');
        }

        servantEditModal.show();
    }
    catch (err) {
        console.error(err);
        alert("Failed to load servant.");
    }
    finally {
        hideLoading();
    }
}
// ============================================
// SAVE
// ============================================
async function saveServant() {
    const rolesDropdown = document.getElementById("drpRoles");
    const selectedRoles = Array.from(rolesDropdown.selectedOptions).map(opt => opt.textContent);

    const classesDropdown = document.getElementById("drpServantClasses");
    const selectedClasses = Array.from(classesDropdown.selectedOptions).map(opt => parseInt(opt.value));
    let payload ={
            email: $("#Email").val(),
            password: $("#Password").val(),
            roles: selectedRoles,
            name: $("#ServantName").val(),
            mobile: $("#Mobile").val(),
            mobile2: $("#Mobile2").val(),
            classes: selectedClasses
        };
    try {
        showLoading();
        const url = `${apiBaseUrl}/Servants/Register` ;
        const response = await fetch(url, {
            method: "POST",
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(payload)
        });
        if (!response.ok) {
            throw new Error("Save failed");
        }
        showSuccessToast("Servant created successfully");
        await loadServants();
    }
    catch (err) {
        console.error(err);
        showFailedToast("Failed to save servant.");
    }
    finally {
        servantModal.hide();
        hideLoading();
    }
}

async function editServant() {
    const servantID = parseInt($('#ServantID').val()) || 0;
    const rolesDropdown = document.getElementById("drpEditRoles");
    const selectedRoles = Array.from(rolesDropdown.selectedOptions).map(opt => opt.value);

    const classesDropdown = document.getElementById("drpEditServantClasses");
    const selectedClasses = Array.from(classesDropdown.selectedOptions).map(opt => parseInt(opt.value));
    let payload = {
        servantID: servantID,
        roles: selectedRoles,
        name: $("#EditServantName").val(),
        mobile: $("#EditMobile").val(),
        mobile2: $("#EditMobile2").val(),
        isActive: $("#EditIsActive").prop("checked"),
        classes: selectedClasses
    };

    try {
        showLoading();
        const url = `${apiBaseUrl}/Servants/Update`;
        const response = await fetch(url, {
            method: "POST",
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(payload)
        });
        if (!response.ok) {
            throw new Error("Save failed");
        }
        showSuccessToast("Servant updated successfully");
        await loadServants();
    }
    catch (err) {
        console.error(err);
        showFailedToast("Failed to save servant.");
    }
    finally {
        servantEditModal.hide();
        hideLoading();
    }
}
// ============================================
// DELETE
// ============================================
function deleteServant(servantID, servantName) {
    showDeleteModal('Delete Servant', `Are you sure you want to delete "${servantName}"?`, servantID);
}
async function btnDelete_click() {
    try {
        const servantID =
            document.getElementById("ID").value;
        const request =
            `${apiBaseUrl}/Servants/Delete`;
        const response = await fetch(request, {
            method: "POST",
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(servantID)
        });
        if (!response.ok) {
            throw new Error("Delete failed");
        }
        closeDeleteModal();
        showSuccessToast("Servant deleted successfully");
        await loadServants();
    }
    catch (err) {
        console.error(err);
        alert("Failed to delete servant.");
    }
}