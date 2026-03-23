async function createMember() {
    const formData = new FormData();

    formData.append("UNPersonalNumber", $("#UNPersonalNumber").val());
    formData.append("UNFileNumber", $("#UNFileNumber").val());
    formData.append("UNFirstName", $("#UNFirstName").val());
    formData.append("UNLastName", $("#UNLastName").val());
    formData.append("Gender", $("#Gender").val());
    formData.append("Birthdate", $("#Birthdate").val());
    formData.append("Baptised", $("#Baptised").is(':checked'));
    formData.append("Mobile", $("#Mobile").val());
    formData.append("Nickname", $("#Nickname").val());
    formData.append("BaptismName", $("#BaptismName").val());
    formData.append("School", $("#School").val());
    formData.append("Work", $("#Work").val());
    formData.append("Notes", $("#Notes").val());

    const selectedClasses = $('#drpClasses').val();
    if (selectedClasses) {
        selectedClasses.forEach(classId => {
            formData.append("Classes", classId);
        });
    }

    const fileInput = document.getElementById("ImageUpload");
    if (fileInput.files.length > 0) {
        formData.append("ImageFile", fileInput.files[0]);
    }
    const request = `${apiBaseUrl}/Member/CreateMemberWImage`;
    try {
        const response = await fetch(request, { method: "POST", credentials: "include", body: formData });
        if (!response.ok) {
            const errorData = await response.json();
            console.error("Failed to save member data:", errorData);
            showFailedToast("Failed to save member: " + errorData.message);
            return;
        }
        else {
            const memberCode = await response.text();
            showSuccessToast("Member created successfully with code: " + memberCode);
        }
    } catch (err) {
        console.error("Failed to submit member data:", err);
        showFailedToast("Failed to save member data!");
    }
}
function openEditModal(memberId) {
    showEditMemberModal(memberId);
}
function refreshData() {
    $("#UNPersonalNumber").val("").focus();
}
$(async function () {
    $("#UNPersonalNumber").on("blur", async function () {
        const unNumber = $(this).val();
        if (!unNumber) return;

        try {
            const response = await fetch(`${apiBaseUrl}/Member/ValidateUNNumber?unFileNumber=${unNumber}`, {
                method: "GET",
                credentials: "include"
            });

            if (response.ok) {
                const data = await response.json(); // Expected: { status: 0, memberID: 123 }

                // 0 = New, 1 = ExistSameChurch, 2 = ExistDifferentChurch
                if (data.status === 1) {
                    showUNModal(
                        "This member already exists, would you like to edit his/her data?",
                        "Edit",
                        () => openEditModal(data.memberID)
                    );
                } else if (data.status === 2) {
                    showUNModal(
                        "This member already exists in another church, would you like to add him to your church?",
                        "Add to church",
                        () => openEditModal(data.memberID)
                    );
                }
            }
        } catch (err) {
            console.error("Validation error:", err);
        }
    });

    function showUNModal(message, actionText, actionCallback) {
        $("#unModalMessage").text(message);
        $("#btnUNAction").text(actionText).off("click").on("click", function () {
            $('#unStatusModal').modal('hide');
            actionCallback();
        });

        $('#unStatusModal').modal('show');
    }

    // Handle Cancel/Escape/Close logic
    $('#unStatusModal').on('hidden.bs.modal', function (e) {
        // If the modal was closed via cancel or backdrop, clear the input
        // We check if the focus is on the action button; if not, it was a cancel action.
        if (document.activeElement.id !== "btnUNAction") {
            $("#UNPersonalNumber").val("").focus();
        }
    });
    showLoading();

    if (typeof loadClasses === "function") { await loadClasses(apiBaseUrl); }
    const baptisedCheckbox = document.getElementById("Baptised");
    const baptismNameInput = document.getElementById("BaptismName");
    function toggleBaptismName() { baptismNameInput.disabled = !baptisedCheckbox.checked; }
    toggleBaptismName();
    baptisedCheckbox.addEventListener("change", toggleBaptismName);
    hideLoading();
    $("#UNPersonalNumber").focus();

    $("#frmCreate").validate({
        ignore: [],
        errorClass: "text-danger",
        errorElement: "div",
        rules: {
            UNPersonalNumber: "required",
            UNFileNumber: "required",
            UNFirstName: "required",
            UNLastName: "required",
            Gender: "required",
            Birthdate: "required"
        },


        highlight: function (element) { $(element).addClass('is-invalid'); if ($(element).hasClass('form-select')) { $(element).next('.select2-container').find('.select2-selection').addClass('is-invalid'); } },
        unhighlight: function (element) { $(element).removeClass('is-invalid'); if ($(element).hasClass('form-select')) { $(element).next('.select2-container').find('.select2-selection').removeClass('is-invalid'); } },

        // The new, simplified errorPlacement function
        errorPlacement: function (error, element) {
            if (element.next('.select2-container').length) {
                error.insertAfter(element.next('.select2-container'));
            } else if (element.parent().hasClass('form-floating')) {
                error.insertAfter(element.parent());
            } else {
                error.insertAfter(element);
            }
        },

        submitHandler: async function (form, event) {
            event.preventDefault();
            showLoading();
            await createMember();
            const unFileNumber = $("#UNFileNumber").val();

            form.reset();
            $('#drpClasses').val(null).trigger('change');

            var validator = $(form).validate();
            validator.resetForm(); // Using the standard reset which should now work.

            setTimeout(function () {
                $("#UNPersonalNumber").focus();
                if (event.originalEvent.submitter?.value === "AddAnother") {
                    $("#UNFileNumber").val(unFileNumber);
                }
            }, 100);

            hideLoading();
        }
    });

    $('#drpClasses').on('change', function () {
        $(this).valid();
    });
});