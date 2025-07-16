let allMembers = [];

document.addEventListener("DOMContentLoaded", function () {
    fetchClassMembers();

    document.getElementById('search-filter').addEventListener('input', applyFiltersAndSort);
    document.getElementById('active-filter').addEventListener('change', applyFiltersAndSort);
    document.getElementById('baptised-filter').addEventListener('change', applyFiltersAndSort);
    document.getElementById('sort-by').addEventListener('change', applyFiltersAndSort);
    document.getElementById('sort-direction').addEventListener('click', toggleSortDirection);
    document.getElementById('download-excel').addEventListener('click', downloadClassData);
});

async function fetchClassMembers() {
    const classID = document.getElementById('classID').value;
    if (!classID) {
        console.error("Class ID is missing");
        showErrorMessage("Class ID is missing. Cannot fetch data.");
        return;
    }

    const url = `${apiBaseUrl}/Meeting/GetClassMembers?classID=${classID}`;

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

        if (!Array.isArray(allMembers) || allMembers.length === 0) {
            showWarningMessage("No members found for this class.");
            document.getElementById("counter").textContent = `(0)`;
        } else {
            applyFiltersAndSort();
        }
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
            member.age.toString().includes(searchTerm) ||
            (member.lastPresentDate && new Date(member.lastPresentDate).toLocaleDateString("en-GB").includes(searchTerm)) ||
            (member.servant && member.servant.toLowerCase().includes(searchTerm))
        );
    }

    // Active Status Filter
    const activeFilterValue = document.getElementById('active-filter').value;
    if (activeFilterValue !== 'all') {
        const mustBeActive = activeFilterValue === 'true';
        filteredMembers = filteredMembers.filter(member => member.isActive === mustBeActive);
    }

    // Baptism Status Filter
    const baptisedFilterValue = document.getElementById('baptised-filter').value;
    if (baptisedFilterValue !== 'all') {
        const mustBeBaptised = baptisedFilterValue === 'true';
        filteredMembers = filteredMembers.filter(member => member.isBaptised === mustBeBaptised);
    }

    // Sorting
    const sortBy = document.getElementById('sort-by').value;
    const sortDirection = document.getElementById('sort-direction').querySelector('i').classList.contains('bi-arrow-down') ? 'desc' : 'asc';

    filteredMembers.sort((a, b) => {
        let valA = a[sortBy];
        let valB = b[sortBy];

        if (sortBy === 'lastPresentDate') {
            valA = a.lastPresentDate ? new Date(a.lastPresentDate) : new Date(0);
            valB = b.lastPresentDate ? new Date(b.lastPresentDate) : new Date(0);
        }

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
                        <input type="hidden" class="baptised-hidden" value="${member.isBaptised}">
                        <input type="hidden" class="isActive-hidden" value="${member.isActive}">
                        <div class="card-body d-flex flex-column">
                            <div>
                                <div class="d-flex justify-content-between align-items-start">
                                    <h5 class="card-title mb-2">
                                        <a href="#" onclick="showMemberDetailsModal(${member.memberID},null)">${member.fullName}</a>
                                    </h5>
                                    <div class="dropdown">
                                        <button class="btn btn-sm btn-light" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                            <i class="bi bi-three-dots-vertical"></i>
                                        </button>
                                        <ul class="dropdown-menu">
                                            <li><a class="dropdown-item" href="#" onclick="handleAction('assign', ${member.memberID})">Assign/Unassign</a></li>
                                            <li><a class="dropdown-item" href="#" onclick="handleAction('assignToAnother', ${member.memberID})">Assign to another</a></li>
                                            <li><a class="dropdown-item" href="#" onclick="handleAction('editUser', ${member.memberID})">Edit user</a></li>
                                            <li><a class="dropdown-item" href="#" onclick="handleAction('updateVisitationNote', ${member.memberID})">Update visitation note</a></li>
                                            <li><a class="dropdown-item" href="#" onclick="handleAction('addVisitation', ${member.memberID})">Add visitation</a></li>
                                        </ul>
                                    </div>
                                </div>
                                <p class="card-text small text-muted"><strong>Code:</strong> ${member.code} | <strong>Age:</strong> ${member.age}</p>
                                <p class="card-text mb-1"><strong>Attendance:</strong> ${member.attendance}</p>
                                <p class="card-text mb-1"><strong>Last Present:</strong> ${member.lastPresentDate ? new Date(member.lastPresentDate).toLocaleDateString("en-GB") : "N/A"}</p>
                                <p class="card-text"><strong>Assigned Servant:</strong> <span data-member-id="${member.memberID}">${member.servant || 'N/A'}</span></p>
                            </div>
                            <div class="mt-auto pt-2">
                                ${member.mobile ? `
                                <p class="card-text mb-0">
                                    <strong>Mobile:</strong> ${member.mobile}
                                    <a href="tel:${member.mobile}" class="ms-2 text-success" title="Call ${member.fullName}">
                                        <i class="bi bi-telephone-fill"></i>
                                    </a>
                                </p>
                                ` : ''}
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

function downloadClassData() {
    const classID = document.getElementById('classID').value;
    if (classID) {
        const url = `${apiBaseUrl}/Meeting/DownloadClassMembers?classID=${classID}`;
        window.location.href = url;
    } else {
        alert("Cannot download data: Class ID is not available.");
    }
}