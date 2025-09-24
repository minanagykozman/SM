document.addEventListener("DOMContentLoaded", function () {
    fetchEventAttendedMembers();
});
async function btnDelete_click() {
    let eventID = document.querySelector('input[name="eventID"]').value;
    let memberID = document.getElementById("ID").value;
    try {
        const request = `${apiBaseUrl}/Events/RemoveMemberAttendance?memberID=${memberID}&eventID=${eventID}`;
        const response = await fetch(request, {
            method: "POST",
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ memberID: memberID, eventID: eventID })
        });
        if (!response.ok) throw new Error("Couldn't remove member");
        closeDeleteModal();
        //showSuccessToast("Member removed successfully");
        fetchEventAttendedMembers();
        var searchInput = document.getElementById("SearchString");
        searchInput.value = "";
        searchInput.focus();
    }
    catch (err) {
        console.error("Error removing member:", err);
    }

}
// Function to fetch attended members
async function fetchEventAttendedMembers() {
    const url = `${apiBaseUrl}/Events/GetEventMembers`;

    const eventID = document.querySelector('input[name="eventID"]').value;
    if (!eventID) {
        console.error("Event ID is missing");
        showErrorMessage("Event ID is missing. Cannot fetch data.");
        return;
    }

    try {
        const response = await fetch(`${url}?eventID=${eventID}&registered=true&attended=true`, {
            method: "GET",
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            }
        });

        if (!response.ok) {
            handleHTTPError(response);
            return;
        }

        const members = await response.json();

        if (!Array.isArray(members) || members.length === 0) {
            showWarningMessage("No members found for this event.");
            return;
        }

        document.getElementById("counter").textContent = `Attended: ${members.length}`;
        populateTable(members);
    } catch (error) {
        console.error("Network error:", error);
        showErrorMessage("Failed to fetch data. Please try again later.");
    }
}


function populateTable(members) {
    const membersGrid = document.getElementById("members-grid");
    membersGrid.innerHTML = "";

    document.getElementById("counter").textContent = `(${members.length} Members)`;

    if (members.length === 0) {
        membersGrid.innerHTML = '<div class="col-12"><div class="alert alert-info text-center">No members match the current filters.</div></div>';
        return;
    }

    members.forEach(member => {
        const memberCardCol = document.createElement("div");
        memberCardCol.className = "col-xl-4 col-lg-6 col-md-6 col-sm-12";

        memberCardCol.innerHTML = `
                <div class="card h-100">
                    <div class="card-body d-flex flex-column">
                        <div>
                            <div class="d-flex justify-content-between align-items-start">
                                <h5 class="card-title mb-2">
                                    ${member.fullName}
                                </h5>
                                <div>
                                     <button class="btn btn-sm btn-outline-danger me-1" onclick="removeMember(${member.memberID},'${member.fullName}')" title="Remove Member">
                                        <i class="bi bi-trash"></i>
                                    </button>
                                </div>
                            </div>
                            <div class="card-text">
                                <p class="mb-1"><small class="text-muted"><strong>Code:</strong> ${member.code || '-'}</small></p>
                             </div>
                        </div>
                    </div>
                </div>
            `;
        membersGrid.appendChild(memberCardCol);
    });
}



document.getElementById("btnCheck").addEventListener("click", function () {
    const searchValue = document.getElementById("SearchString").value.trim();

    if (!searchValue) {
        alert("Please enter a search value.");
        return;
    }


    const eventID = document.querySelector('input[name="eventID"]').value;

    const apiUrl = `${apiBaseUrl}/Events/CheckAttendanceStatus?memberCode=${encodeURIComponent(searchValue)}&eventID=${eventID}`;
    fetch(apiUrl, {
        method: "GET",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        }
    })
        .then(response => response.json())
        .then(data => {
            populateModal(data); // Function to fill modal with API response
            showModal(); // Function to display the modal
        })
        .catch(error => {
            console.error("Error fetching data:", error);
            alert("Failed to fetch data. Please try again.");
        });

});

// Function to fill modal with API response
function populateModal(data) {
    switch (data.status) {
        case 0:
            document.getElementById("MemberStatusAlert").value = "Member not found or not registered";
            $("#divStatus").hide();
            $("#divStatusAlert").show();
            $("#MemberDataBody").hide();
            $("#submitBtn").hide();
            break;
        case 1:
            document.getElementById("MemberStatusAlert").value = "Event not found";
            $("#divStatus").hide();
            $("#divStatusAlert").show();
            $("#MemberDataBody").hide();
            $("#submitBtn").hide();
            break;
        case 5:
            document.getElementById("MemberStatus").value = "Can attend";
            $("#divStatus").show();
            $("#divStatusAlert").hide();
            document.getElementById("MemberCode").value = data.member.code;
            document.getElementById("MemberName").value = data.member.fullName;
            document.getElementById("Mobile").value = data.member.mobile;
            document.getElementById("Team").value = data.member.team;
            document.getElementById("Bus").value = data.member.bus;
            document.querySelector('input[name="MemberCode"]').value = data.member.code;
            $("#divTeam").show();
            $("#divBus").show();
            $("#MemberDataBody").show();
            $("#submitBtn").show();
            break;
        case 7:
            document.getElementById("MemberStatusAlert").value = "Member not registered in this event!";
            $("#divStatus").hide();
            $("#divStatusAlert").show();
            $("#MemberDataBody").hide();
            $("#submitBtn").hide();

            $("#divTeam").hide();
            $("#divBus").hide();
            break;
        case 8:
            document.getElementById("MemberStatusAlert").value = "Member already attedned!";
            $("#divStatus").hide();
            $("#divStatusAlert").show();
            $("#MemberDataBody").hide();
            $("#submitBtn").hide();
            break;
    }
}

function removeMember(memberID, memberName) {
    let message = 'Are you sure you want to remove member from attendace ' + memberName + '?';

    showDeleteModal('Remove Member', message, memberID);
}

document.getElementById("submitBtn").addEventListener("click", function () {

    const eventID = document.querySelector('input[name="eventID"]').value; // Get eventID from hidden input
    const memberCode = document.querySelector('input[name="MemberCode"]').value;

    const apiUrl = `${apiBaseUrl}/Events/TakeAttendance?memberCode=${encodeURIComponent(memberCode)}&eventID=${eventID}`;


    fetch(apiUrl, {
        method: "POST",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ memberCode: memberCode, eventID: eventID })
    })
        .then(response => response.json())
        .then(data => {
            console.log("Success:", data);
            fetchEventAttendedMembers();
            closeModal(); // Close the modal after successful API call
            var searchInput = document.getElementById("SearchString");
            searchInput.value = "";
            searchInput.focus();
        })
        .catch(error => {
            console.error("Error:", error);
            alert("Failed to add member. Please try again.");
        });
});