// --- 1. Define Modal Elements and Validator Instance (do this once) ---
const editMemberModalEl = document.getElementById('editMemberModal');
const editMemberModal = new bootstrap.Modal(editMemberModalEl);
let formValidator; // We will store the validator instance here

// --- 2. Setup All Modal Logic (runs once after the page is ready) ---
$(async function () {
    // A) Initialize Form Validation (ONCE)
    await loadClasses(apiBaseUrl);
    formValidator = $("#frmEdit").validate({
        ignore: [],
        errorClass: "text-danger",
        errorElement: "div",
        rules: {
            UNPersonalNumberEdit: {
                required: true,
                remote: {
                    url: `${apiBaseUrl}/Member/ValidateUNNumber`,
                    type: "get",
                    data: {
                        unFileNumber: () => $("#UNPersonalNumberEdit").val(),
                        memberID: () => $("#MemberIDEdit").val()
                    },
                    xhrFields: { withCredentials: true }
                }
            },
            CodeEdit: "required",
            UNFileNumberEdit: "required",
            UNFirstNameEdit: "required",
            UNLastNameEdit: "required",
            GenderEdit: "required",
            BirthdateEdit: "required"
        },
        messages: {
            UNPersonalNumberEdit: { required: "UN Personal Number is required.", remote: "This UN number is used by another member." }
        },
        highlight: function (element) { $(element).addClass('is-invalid'); },
        unhighlight: function (element) { $(element).removeClass('is-invalid'); },
        errorPlacement: function (error, element) {
            if ($(element).parent().hasClass('form-floating')) {
                error.insertAfter(element.parent());
            } else if (element.next('.select2-container').length) {
                error.insertAfter(element.next('.select2-container'));
            }
            else {
                error.insertAfter(element);
            }
        },
        submitHandler: async function (form, event) {
            event.preventDefault();
            showLoading();
            await updateMember();
            hideLoading();
            editMemberModal.hide();
        }
    });

    editMemberModalEl.addEventListener('show.bs.modal', async (event) => {
        const memberID = event.currentTarget.dataset.memberid;
        if (memberID) {
            showLoading();
            // Load all necessary data when the modal opens
            await loadMemberData(memberID);
            hideLoading();
        }
    });

    // C) Attach Event Listener for when the modal CLOSES (ONCE)
    editMemberModalEl.addEventListener('hidden.bs.modal', () => {
        clearEditModal();
    });

    // D) Attach other static event listeners (ONCE)
    $('#drpClasses').on('change', function () {
        $(this).valid(); // Trigger validation for the classes dropdown
    });

    const baptisedCheckbox = document.getElementById("BaptisedEdit");
    baptisedCheckbox.addEventListener("change", function () {
        document.getElementById("BaptismNameEdit").disabled = !this.checked;
    });
});


// --- 3. Define the Functions that will be called by the events ---

/**
 * Public function to be called from other pages to show the modal.
 */
function showEditMemberModal(memberID) {
    editMemberModalEl.dataset.memberid = memberID;
    editMemberModal.show();
}

/**
 * Clears all dynamic data and validation from the edit modal.
 */
function clearEditModal() {
    const form = document.getElementById('frmEdit');
    form.reset();
    $('#drpClasses').val(null).trigger('change');
    if (formValidator) {
        formValidator.resetForm();
    }
    form.querySelectorAll('.is-invalid').forEach(el => el.classList.remove('is-invalid'));
}

/**
 * Fetches and populates the form with a specific member's data.
 */
async function loadMemberData(memberID) {
    try {
        const response = await fetch(`${apiBaseUrl}/Member/GetMember?memberID=${memberID}`, {
            method: "GET",
            credentials: "include",
            headers: { "Content-Type": "application/json" }
        });
        if (!response.ok) throw new Error("Failed to load member data");
        const memberData = await response.json();

        const birthdate = new Date(memberData.birthdate);
        const formattedBirthdate = birthdate.toISOString().split('T')[0];
        document.getElementById("CodeEdit").value = memberData.code || '';
        document.getElementById("MemberIDEdit").value = memberData.memberID || '';
        document.getElementById("UNPersonalNumberEdit").value = memberData.unPersonalNumber || '';
        document.getElementById("UNFileNumberEdit").value = memberData.unFileNumber || '';
        document.getElementById("UNFirstNameEdit").value = memberData.unFirstName || '';
        document.getElementById("UNLastNameEdit").value = memberData.unLastName || '';
        document.getElementById("GenderEdit").value = memberData.gender || '';
        document.getElementById("BirthdateEdit").value = formattedBirthdate;
        document.getElementById("BaptisedEdit").checked = memberData.baptised;
        document.getElementById("MobileEdit").value = memberData.mobile || '';
        document.getElementById("NicknameEdit").value = memberData.nickname || '';
        document.getElementById("BaptismNameEdit").value = memberData.baptismName || '';
        document.getElementById("SchoolEdit").value = memberData.school || '';
        document.getElementById("WorkEdit").value = memberData.work || '';
        document.getElementById("NotesEdit").value = memberData.notes || '';
        document.getElementById("ImageURLEdit").value = memberData.imageURL || '';
        document.getElementById("S3KeyEdit").value = memberData.s3ImageKey || '';
        document.getElementById("CardStatusEdit").value = memberData.cardStatus || '';

        document.getElementById("BaptismNameEdit").disabled = !memberData.baptised;

        if (memberData.classesIDs) {
            $('#drpClasses').val(memberData.classesIDs).trigger('change');
        }
    } catch (err) {
        console.error("Error loading member details:", err);
    }
}

/**
 * Gathers form data and sends it to the server to update the member.
 */
async function updateMember() {
    const id = document.getElementById("MemberIDEdit").value;
    const formData = new FormData();
    console.info('Loading data');
    // Collect form values
    formData.append("MemberID", id);
    formData.append("UNPersonalNumber", document.getElementById("UNPersonalNumberEdit").value);
    formData.append("UNFileNumber", document.getElementById("UNFileNumberEdit").value);
    formData.append("UNFirstName", document.getElementById("UNFirstNameEdit").value);
    formData.append("UNLastName", document.getElementById("UNLastNameEdit").value);
    formData.append("Gender", document.getElementById("GenderEdit").value);
    formData.append("Birthdate", document.getElementById("BirthdateEdit").value);
    formData.append("Baptised", document.getElementById("BaptisedEdit").checked);
    formData.append("Mobile", document.getElementById("MobileEdit").value);
    formData.append("Nickname", document.getElementById("NicknameEdit").value);
    formData.append("BaptismName", document.getElementById("BaptismNameEdit").value);
    formData.append("School", document.getElementById("SchoolEdit").value);
    formData.append("Work", document.getElementById("WorkEdit").value);
    formData.append("Notes", document.getElementById("NotesEdit").value);
    formData.append("Code", document.getElementById("CodeEdit").value);
    formData.append("ImageURL", document.getElementById("ImageURLEdit").value);
    formData.append("S3ImageKey", document.getElementById("S3KeyEdit").value);
    formData.append("CardStatus", document.getElementById("CardStatusEdit").value);

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
    const request = `${apiBaseUrl}/Member/UpdateMemberWImage`;
    try {
        const response = await fetch(request, {
            method: "POST",
            credentials: "include",
            body: formData
        });

        if (!response.ok) {
            const errorData = await response.json();
            console.error("Failed to update member data:", errorData);
            //showFailedToast("Failed to update member: " + errorData.message);
            return;
        }

        //showSuccessToast("Member Updated successfully!");
    } catch (err) {
        console.error("Failed to submit member data:", err);
        //showFailedToast("Failed to save member data!");
    }
}