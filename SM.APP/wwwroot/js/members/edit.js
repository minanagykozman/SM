const modal = new bootstrap.Modal(document.getElementById("editMemberModal"));
async function showEditMemberModal(memberID) {
    $('#ID').val(memberID);
    modal.show();
    showLoading('#edit-container');
    await initailizeForm(memberID);
    await initializePage(memberID);
    $('#drpClasses').on('change', function () {
        $(this).valid();
    });
    hideLoading('#edit-container');
}
async function initailizeForm(memberID) {
    $("#frmEdit").validate({
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
                        unFileNumber: function () { return $("#UNPersonalNumberEdit").val(); },
                        memberID: function () { return memberID; }
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
        highlight: function (element) { $(element).addClass('is-invalid'); if ($(element).hasClass('form-select')) { $(element).next('.select2-container').find('.select2-selection').addClass('is-invalid'); } },
        unhighlight: function (element) { $(element).removeClass('is-invalid'); if ($(element).hasClass('form-select')) { $(element).next('.select2-container').find('.select2-selection').removeClass('is-invalid'); } },
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
            showLoading('#edit-container');
            await updateMember();
            hideLoading('#edit-container');
            modal.hide();
        }
    });
}
async function loadMemberData(memberID) {
    try {
        const response = await fetch(`${apiBaseUrl}/Member/GetMember?memberID=${memberID}`, {
            method: "GET",
            credentials: "include",
            headers: { "Content-Type": "application/json" }
        });
        if (!response.ok) throw new Error("Failed to load event data");
        const memberData = await response.json();
        console.info(memberData);
        const birthdate = new Date(memberData.birthdate);
        const formattedBirthdate = birthdate.toISOString().split('T')[0];
        document.getElementById("CodeEdit").value = memberData.code;
        document.getElementById("MemberIDEdit").value = memberData.memberID;
        document.getElementById("UNPersonalNumberEdit").value = memberData.unPersonalNumber;
        document.getElementById("UNFileNumberEdit").value = memberData.unFileNumber;
        document.getElementById("UNFirstNameEdit").value = memberData.unFirstName;
        document.getElementById("UNLastNameEdit").value = memberData.unLastName;
        document.getElementById("GenderEdit").value = memberData.gender;
        document.getElementById("BirthdateEdit").value = formattedBirthdate;
        document.getElementById("BaptisedEdit").checked = memberData.baptised;
        document.getElementById("MobileEdit").value = memberData.mobile;
        document.getElementById("NicknameEdit").value = memberData.nickname;
        document.getElementById("BaptismNameEdit").value = memberData.baptismName;
        document.getElementById("SchoolEdit").value = memberData.school;
        document.getElementById("WorkEdit").value = memberData.work;
        document.getElementById("NotesEdit").value = memberData.notes;
        document.getElementById("ImageURLEdit").value = memberData.imageURL;
        document.getElementById("S3KeyEdit").value = memberData.s3ImageKey;
        document.getElementById("CardStatusEdit").value = memberData.cardStatus;


        
        // Set selected classes
        const classIDs = memberData.classesIDs;

        $('#drpClasses').val(classIDs).trigger('change');

    } catch (err) {
        console.error("Error loading member details:", err);
    }
}
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
async function initializePage(memberID) {
    await loadClasses(apiBaseUrl); 
    await loadMemberData(memberID);
    // Set up other page logic after data is loaded
    const baptisedCheckbox = document.getElementById("BaptisedEdit");
    const baptismNameInput = document.getElementById("BaptismNameEdit");
    function toggleBaptismName() { baptismNameInput.disabled = !baptisedCheckbox.checked; }
    toggleBaptismName();
    baptisedCheckbox.addEventListener("change", toggleBaptismName);
}