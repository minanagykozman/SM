function populateFamilyMembers(members) {
    const membersList = document.getElementById('membersFamilyList');
    if (!members || members.length === 0) {
        membersList.innerHTML = '<div class="list-group-item text-center text-muted">No members available</div>';
        return;
    }
    membersList.innerHTML = members.map(function (member) {
        return `
        <div class="list-group-item list-group-item-action flex-column align-items-start">
            <div class="d-flex w-100 justify-content-between align-items-center">
                <h5 class="mb-1">${member.fullName || 'N/A'}</h5>
            </div>
            <small class="text-muted d-block"><strong>Code:</strong> ${member.code || 'N/A'}</small>
            <small class="text-muted d-block"><strong>Age:</strong> ${member.age || 'N/A'}</small>
        </div>`;
    }).join('');
}
async function loadMemberEvents(memberID) {
    if (memberID) {
        try {
            showLoading();
            const response = await fetch(`${apiBaseUrl}/Member/GetMemberEventRegistrations?memberID=${memberID}`, {
                method: "GET",
                credentials: "include",
                headers: { "Content-Type": "application/json" }
            });
            if (!response.ok) throw new Error("Failed to load event data");
            const eventRegistrations = await response.json();
            populateEventRegistrationsData(eventRegistrations);
            hideLoading();
        } catch (err) {
            console.error("Error loading event:", err);
        }
    }
}
function populateEventRegistrationsData(eventRegistrations) {
    const eventRegistrationsList = document.getElementById('eventRegistrationsList');
    if (!eventRegistrations || eventRegistrations.length === 0) {
        eventRegistrationsList.innerHTML = '<div class="list-group-item text-center text-muted">No event registrations available</div>';
        return;
    }

    eventRegistrationsList.innerHTML = eventRegistrations.map(function (registration) {
        const eventName = registration.event?.eventName || 'N/A';
        const eventDate = registration.event?.eventStartDate ? formatDate(registration.event.eventStartDate) : 'Date N/A';

        let statusClass = 'badge bg-warning';
        let statusText = 'Registered';
        if (registration.attended === true) {
            statusClass = 'badge bg-success';
            statusText = 'Attended';
        } else if (registration.attended === false) {
            statusClass = 'badge bg-danger';
            statusText = 'Absent';
        }

        return `
            <div class="list-group-item list-group-item-action flex-column align-items-start">
                <div class="d-flex w-100 justify-content-between align-items-center">
                    <h5 class="mb-1">${eventName}</h5>
                    <span class="${statusClass} fs-6">${statusText}</span>
                </div>
                <small class="text-muted"><i class="bi bi-calendar-event me-1"></i>${eventDate}</small>
            </div>`;
    }).join('');
}
async function loadMemberDetails(memberID) {
    if (memberID) {
        try {

            const response = await fetch(`${apiBaseUrl}/Member/GetMember?memberID=${memberID}&&includeFamilyCount=true`, {
                method: "GET",
                credentials: "include",
                headers: { "Content-Type": "application/json" }
            });

            if (!response.ok) throw new Error("Failed to load event data");
            const memberData = await response.json();
            const birthdate = new Date(memberData.birthdate);
            const formattedBirthdate = birthdate.toISOString().split('T')[0]; // "YYYY-MM-DD"
            // Fill form with data
            $("#Code").val(memberData.code);
            $("#MemberID").val(memberData.memberID);
            $("#UNPersonalNumber").val(memberData.unPersonalNumber);
            $("#UNFileNumber").val(memberData.unFileNumber);
            $("#FullName").val(memberData.fullName);
            $("#Gender").val(memberData.gender);
            $("#Birthdate").val(formattedBirthdate);
            $("#Baptised").prop("checked", memberData.baptised);
            $("#Mobile").val(memberData.mobile);
            $("#Nickname").val(memberData.nickname);
            $("#BaptismName").val(memberData.baptismName);
            $("#School").val(memberData.school);
            $("#Work").val(memberData.work);
            $("#Notes").val(memberData.notes);
            $("#ImageReference").val(memberData.imageReference);
            $("#CardStatus").val(memberData.cardStatus);

            // Member Image
            const memberImage = document.getElementById('memberImage');
            const placeholder = document.getElementById('memberImagePlaceholder');

            if (memberImage && placeholder) {
                if (memberData?.imageURL) {
                    memberImage.src = memberData.imageURL;
                    memberImage.classList.remove('d-none');
                    placeholder.classList.add('d-none');

                    memberImage.onerror = function () {
                        // If the image fails, hide it and show the placeholder instead
                        memberImage.classList.add('d-none');
                        placeholder.classList.remove('d-none');
                    };
                } else {
                    // If there's no URL, show the placeholder and hide the image
                    placeholder.classList.remove('d-none');
                    memberImage.classList.add('d-none');
                }
            }
            populateFamilyMembers(memberData.familyMembers);
            await loadMemberFundsPartial(memberID, null);
            await loadMemberAidsPartial(memberID);
            await loadMemberAttendancePartial(memberID);
            await loadMemberEvents(memberID);
        } catch (err) {
            console.error("Error loading event:", err);
        }
    }
}

async function showMemberDetailsModal(memberID, returnURL) {
    const modal = new bootstrap.Modal(document.getElementById("memberDetailsModal"));
    modal.show();
    await loadMemberDetails(memberID);

    $('#btnEdit').on('click', function (e) {
        e.preventDefault();
        if (memberID) {
            if (returnURL)
                window.location.href = `/Admin/Members/Edit?id=${memberID}&returnURL='${returnURL}'`;
            else
                window.location.href = `/Admin/Members/Edit?id=${memberID}`;
        } else {
            alert('Member ID is missing.');
        }
    });
}