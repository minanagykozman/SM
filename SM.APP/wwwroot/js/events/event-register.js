document.addEventListener("DOMContentLoaded", function () {

    $("#submitBtn").html('<i class="bi bi-check-circle"></i> Register');

    //populate registered members data.
    fetchEventMembers();
});
var myModal = document.getElementById('myModal');

myModal.addEventListener('shown.bs.modal', function () {
    const paid = document.getElementById("Paid");
    paid.focus();
    paid.select();
});
var myPaymentModal = document.getElementById('updatePaymentModal');

myPaymentModal.addEventListener('shown.bs.modal', function () {
    const editPaidInput = document.getElementById("editPaid");
    editPaidInput.focus();
    editPaidInput.select();
});
// Function to fetch attended members
async function fetchEventMembers() {
    const url = `${apiBaseUrl}/Events/GetEventRegisteredMembers`;
    const eventID = document.querySelector('input[name="eventID"]').value;
    if (!eventID) {
        console.error("Event ID is missing");
        showErrorMessage("Event ID is missing. Cannot fetch data.");
        return;
    }

    try {

        showLoading();
        const response = await fetch(`${url}?eventID=${eventID}`, {
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

        document.getElementById("counter").textContent = `Members: ${members.length}`;
        populateTable(members);
    } catch (error) {
        console.error("Network error:", error);
        showErrorMessage("Failed to fetch data. Please try again later.");
    }
    finally {
        hideLoading();
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
                    <input type="hidden" class="baptised-hidden" value="${member.baptised}">
                    <div class="card-body d-flex flex-column">
                        <div>
                            <div class="d-flex justify-content-between align-items-start">
                                <h5 class="card-title mb-2">
                                    ${member.fullName}
                                </h5>
                                <div>
                                    <button class="btn btn-sm btn-outline-primary" onclick="updatePayment(${member.memberID},${member.paid})" title="Update Payment">
                                        <i class="bi bi-currency-dollar"></i>
                                    </button>
                                     <button class="btn btn-sm btn-outline-danger me-1" onclick="removeMember(${member.memberID},'${member.fullName}')" title="Remove Member">
                                        <i class="bi bi-trash"></i>
                                    </button>
                                </div>
                            </div>
                            <div class="card-text">
                                <p class="mb-1"><small class="text-muted"><strong>Code:</strong> ${member.code || '-'}</small></p>
                                <p class="mb-1"><small class="text-muted"><strong>Paid:</strong> ${member.paid || '-'}</small></p>
                             </div>
                        </div>
                    </div>
                </div>
            `;
        membersGrid.appendChild(memberCardCol);
    });
}
function removeMember(memberID, memberName) {
    let message = 'Are you sure you want to remove member ' + memberName + '?';

    showDeleteModal('Remove Member', message, memberID);
}
function updatePayment(memberID, oldAmount) {
    document.getElementById("memberEditPaymentID").value = memberID;
    document.getElementById("editPaid").value = oldAmount;

    const modal = new bootstrap.Modal(document.getElementById("updatePaymentModal"));
    modal.show();

}
async function btnDelete_click() {
    let eventID = document.querySelector('input[name="eventID"]').value;
    let memberID = document.getElementById("ID").value;
    try {
        const request = `${apiBaseUrl}/Events/RemoveMember?memberID=${memberID}&eventID=${eventID}`;
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
        fetchEventMembers();
        var searchInput = document.getElementById("SearchString");
        searchInput.value = "";
        searchInput.focus();
    }
    catch (err) {
        console.error("Error removing member:", err);
    }

}
async function btnUpdatePayment_click() {
    let eventID = document.querySelector('input[name="eventID"]').value;
    let paidAmount = document.getElementById("editPaid").value;
    let memberID = document.getElementById("memberEditPaymentID").value;
    try {
        const request = `${apiBaseUrl}/Events/UpdatePayment?memberID=${memberID}&eventID=${eventID}&paid=${paidAmount}`;
        const response = await fetch(request, {
            method: "POST",
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ memberID: memberID, eventID: eventID, paid: paidAmount })
        });
        if (!response.ok) throw new Error("Couldn't remove member");
        if (!response.ok) throw new Error("Couldn't remove member");
        const modal = bootstrap.Modal.getInstance(document.getElementById("updatePaymentModal"));
        if (modal) modal.hide();
        //showSuccessToast("Member removed successfully");
        fetchEventMembers();
        var searchInput = document.getElementById("SearchString");
        searchInput.value = "";
        searchInput.focus();
    }
    catch (err) {
        console.error("Error removing member:", err);
    }

}


document.getElementById("btnCheck").addEventListener("click", async function () {
    const searchValue = document.getElementById("SearchString").value.trim();
    if (!searchValue) {
        alert("Please enter a search value.");
        return;
    }

    const eventID = document.querySelector('input[name="eventID"]').value;
    const apiUrl = `${apiBaseUrl}/Events/CheckRegistrationStatus?memberCode=${encodeURIComponent(searchValue)}&eventID=${eventID}`;

    try {
        const response = await fetch(apiUrl, {
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

        const data = await response.json();
        await populateModal(data);
        showModal();

    } catch (error) {
        console.error("Error fetching data:", error);
        alert("Failed to fetch data. Please try again.");
    }
});

// Function to fill modal with API response
async function populateModal(data) {

    document.querySelector('input[name="MemberStatusCode"]').value = data.status;

    switch (data.status) {
        case 0:
            document.getElementById("MemberStatusAlert").value = "Member not found";
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
        case 2:
            document.getElementById("MemberStatusAlert").value = "Member not elligible";
            $("#divStatus").hide();
            $("#divStatusAlert").show();
            document.getElementById("MemberCode").value = data.member.code;
            document.getElementById("MemberName").value = data.member.fullName;
            document.querySelector('input[name="MemberCode"]').value = data.member.code;
            document.getElementById("Paid").value = 0;


            await loadMemberAttendancePartial(data.member.memberID);
            $("#MemberDataBody").show();
            $("#submitBtn").show();
            break;
        case 3:
            document.getElementById("MemberStatusAlert").value = "Member already registered!";
            $("#divStatus").hide();
            $("#divStatusAlert").show();
            $("#MemberDataBody").hide();
            $("#submitBtn").hide();
            document.getElementById("MemberName").value = data.member.fullName;
            break;
        case 4:
            document.getElementById("MemberStatus").value = "Can register";
            $("#divStatus").show();
            $("#divStatusAlert").hide();
            document.getElementById("MemberCode").value = data.member.code;
            document.getElementById("MemberName").value = data.member.fullName;
            document.querySelector('input[name="MemberCode"]').value = data.member.code;
            document.getElementById("Paid").value = 0;
            await loadMemberAttendancePartial(data.member.memberID);
            $("#MemberDataBody").show();
            $("#submitBtn").show();
            break;

    }
}

document.getElementById("submitBtn").addEventListener("click", function () {

    const eventID = document.querySelector('input[name="eventID"]').value; // Get eventID from hidden input
    const memberCode = document.querySelector('input[name="MemberCode"]').value;
    const paid = document.querySelector('input[name="Paid"]').value;
    const notes = document.querySelector('input[name="Notes"]').value;
    const memberStatusCode = document.querySelector('input[name="MemberStatusCode"]').value;
    let isException = memberStatusCode === 2;
    const apiUrl = `${apiBaseUrl}/Events/Register?memberCode=${encodeURIComponent(memberCode)}&paid=${paid}&eventID=${eventID}&isException=${isException}&notes=''`;


    fetch(apiUrl, {
        method: "POST",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ memberCode: memberCode, paid: paid, eventID: eventID, isException: isException, notes: notes })
    })
        .then(response => response.json())
        .then(data => {
            console.log("Success:", data);
            fetchEventMembers();
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