let allMembers = [];

document.addEventListener("DOMContentLoaded", function () {
    fetchEventMembers();

    document.getElementById('search-filter').addEventListener('input', applyFiltersAndSort);
    document.getElementById('active-filter').addEventListener('change', applyFiltersAndSort);
    document.getElementById('baptised-filter').addEventListener('change', applyFiltersAndSort);
    document.getElementById('sort-by').addEventListener('change', applyFiltersAndSort);
    document.getElementById('sort-direction').addEventListener('click', toggleSortDirection);
    document.getElementById('download-excel').addEventListener('click', downloadEventData); 
    document.getElementById('update-attendance').addEventListener('click', updateAttendance);
    document.getElementById('download-cards').addEventListener('click', downloadCards);
});

async function fetchEventMembers() {
    const eventID = document.getElementById('eventID').value;
    if (!eventID) {
        console.error("Event ID is missing");
        showErrorMessage("Event ID is missing. Cannot fetch data.");
        return;
    }

    const url = `${apiBaseUrl}/Events/GetEventMembers?eventID=${eventID}&registered=true`;

    try {
        showLoading();
        const response = await fetch(url, {
            method: "GET",
            credentials: "include",
            headers: { "Content-Type": "application/json" }
        });

        if (!response.ok) {
            handleHTTPError(response);
            return;
        }

        allMembers = await response.json();
        applyFiltersAndSort();
    } catch (error) {
        console.error("Network error:", error);
        showErrorMessage("Failed to fetch data. Please try again later.");
    } finally {
        hideLoading();
    }
}

function applyFiltersAndSort() {
    let filteredMembers = [...allMembers];

    // Text Search
    const searchTerm = document.getElementById('search-filter').value.toLowerCase();
    if (searchTerm) {
        filteredMembers = filteredMembers.filter(member =>
            member.code.toLowerCase().includes(searchTerm) ||
            member.fullName.toLowerCase().includes(searchTerm) ||
            (member.team && member.team.toLowerCase().includes(searchTerm)) ||
            (member.bus && member.bus.toLowerCase().includes(searchTerm)) ||
            (member.room && member.room.toLowerCase() == searchTerm.toLowerCase()) ||
            member.age.toString().includes(searchTerm)
        );
    }

    // Baptism Status Filter
    const baptisedFilterValue = document.getElementById('baptised-filter').value;
    if (baptisedFilterValue !== 'all') {
        const mustBeBaptised = baptisedFilterValue === 'true';
        filteredMembers = filteredMembers.filter(member => member.baptised === mustBeBaptised);
    }
    // Active Status Filter
    const activeFilterValue = document.getElementById('active-filter').value;
    if (activeFilterValue !== 'all') {
        const filterValue = activeFilterValue === 'null' ? null : activeFilterValue === 'true';
        filteredMembers = filteredMembers.filter(member => member.attended === filterValue);
    }
    // Sorting
    const sortBy = document.getElementById('sort-by').value;
    const sortDirection = document.getElementById('sort-direction').querySelector('i').classList.contains('bi-arrow-down') ? 'desc' : 'asc';
    
    filteredMembers.sort((a, b) => {
        let valA = a[sortBy];
        let valB = b[sortBy];

        // Handle nulls by treating them as the lowest possible value
        if (valA === null || valA === undefined) valA = -Infinity;
        if (valB === null || valB === undefined) valB = -Infinity;

        if (valA < valB) return sortDirection === 'asc' ? -1 : 1;
        if (valA > valB) return sortDirection === 'asc' ? 1 : -1;
        return 0;
    });

    populateMembersGrid(filteredMembers);
}

function populateMembersGrid(members) {
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
                                <a href="#" onclick="showMemberDetailsModal(${member.memberID},null)">${member.fullName}</a>
                            </h5>
                        </div>
                        <div class="card-text">
                            <p class="mb-1"><small class="text-muted"><strong>Code:</strong> ${member.code || '-'}</small></p>
                            <p class="mb-1"><small class="text-muted">
                                <strong>Team:</strong> ${member.team || '-'}</small> | <small class="text-muted"><strong>Bus:</strong> ${member.bus || '-'}</small> | <small class="text-muted"><strong>Room:</strong> ${member.room || '-'}</small> | <small class="text-muted"><strong>Paid:</strong> ${member.paid || '-'} EGP</small>
                            </p>
                            <span class="badge bg-primary me-2">${member.gender === 'M' ? 'Male' : 'Female'}</span>
                            <span class="badge bg-secondary me-2">Baptised: ${member.baptised ? 'Yes' : 'No'}</span>
                            ${member.attended === null ? `<span class="badge bg-warning text-dark">Registered</span>` : (member.attended ? `<span class="badge bg-success">Present</span>` : `<span class="badge bg-danger">Absent</span>`)}
                            ${member.mobile ? `<div class="mt-3 d-grid"><a href="tel:${member.mobile}" class="btn btn-outline-success btn-sm">
                            <i class="bi bi-telephone-fill me-2"></i> ${member.mobile}</a></div>` : ''}
                         </div>
                    </div>
                </div>
            </div>
        `;
        membersGrid.appendChild(memberCardCol);
    });
}

function toggleSortDirection() {
    const icon = document.getElementById('sort-direction').querySelector('i');
    icon.classList.toggle('bi-arrow-down');
    icon.classList.toggle('bi-arrow-up');
    applyFiltersAndSort();
}

function handleAction(action, memberId) {
    alert(`Functionality for '${action}' on member ID ${memberId} is not implemented yet.`);
}

function downloadEventData() {
    const eventID = document.getElementById('eventID').value;
    if (eventID) {
        const url = `${apiBaseUrl}/Events/DownloadEventMembers?eventID=${eventID}`;
        window.location.href = url;
    } else {
        alert("Cannot download data: Event ID is not available.");
    }
}

async function updateAttendance() {
    const eventID = document.getElementById('eventID').value;
    if (eventID) {
        showLoading();
        const url = `${apiBaseUrl}/Events/UpdateAttendance?eventID=${eventID}`;
        const response = await fetch(url, {
            method: "GET",
            credentials: "include",
            headers: { "Content-Type": "application/json" }
        });
        fetchEventMembers();
        hideLoading();
        if (!response.ok) {
            handleHTTPError(response);
            return;
        }

    } else {
        alert("Cannot update data: Event ID is not available.");
    }
}

async function downloadCards() {
    if (!allMembers || allMembers.length === 0) {
        alert("No member data available to download cards.");
        return;
    }
    const memberIDs = allMembers.map(member => member.memberID);
    bulkPrintCards(memberIDs,"Trip");
}