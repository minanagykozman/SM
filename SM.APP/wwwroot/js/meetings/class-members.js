let allMembers = [];
let cachedServants = [];
let selectedMemberIDs = new Set();

document.addEventListener("DOMContentLoaded", function () {
    fetchClassMembers();

    document.getElementById('search-filter').addEventListener('input', applyFiltersAndSort);
    document.getElementById('active-filter').addEventListener('change', applyFiltersAndSort);
    document.getElementById('baptised-filter').addEventListener('change', applyFiltersAndSort);
    document.getElementById('sort-by').addEventListener('change', applyFiltersAndSort);
    document.getElementById('sort-direction').addEventListener('click', toggleSortDirection);
    document.getElementById('download-excel').addEventListener('click', downloadClassData);

    document.getElementById('selectAllMembers').addEventListener('change', toggleSelectAll);

    document.querySelectorAll('.dropdown-item[data-action]').forEach(item => {
        item.addEventListener('click', function (e) {
            e.preventDefault();
            handleBulkAction(this.getAttribute('data-action'));
        });
    });

    const saveBtn = document.getElementById('btnSaveVisitation');
    if (saveBtn)
        saveBtn.addEventListener('click', submitVisitationRequest);
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

    // Update Select All checkbox state based on current visible list vs selected set
    updateSelectAllCheckboxState(members);

    if (members.length === 0) {
        membersGrid.innerHTML = '<div class="col-12"><div class="alert alert-info text-center">No members match the current filters.</div></div>';
        return;
    }

    members.forEach(member => {
        const memberCardCol = document.createElement("div");
        memberCardCol.className = "col-xl-4 col-lg-6 col-md-6 col-sm-12";

        // Check if this member is currently selected
        const isSelected = selectedMemberIDs.has(member.memberID);

        memberCardCol.innerHTML = `
            <div class="card h-100 position-relative">
                <input type="hidden" class="baptised-hidden" value="${member.isBaptised}">
                <input type="hidden" class="isActive-hidden" value="${member.isActive}">
                
                <div class="position-absolute top-0 start-0 m-2" style="z-index: 10;">
                    <input type="checkbox" class="form-check-input member-checkbox p-2" 
                           style="cursor:pointer;"
                           value="${member.memberID}" 
                           ${isSelected ? 'checked' : ''} 
                           onchange="toggleMemberSelection(${member.memberID}, this)">
                </div>

                <div class="card-body d-flex flex-column pt-4"> <div>
                        <div class="d-flex justify-content-between align-items-start">
                            <h5 class="card-title mb-2 ms-3"> <a href="#" onclick="showMemberDetailsModal(${member.memberID},null)">${member.fullName}</a>
                            </h5>
                            <div class="dropdown">
                                <button class="btn btn-sm btn-light" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                    <i class="bi bi-three-dots-vertical"></i>
                                </button>
                                <ul class="dropdown-menu">
                                    <li><a class="dropdown-item" href="#" onclick="handleAction('assign', ${member.memberID})">Assign/Unassign</a></li>
                                    <li><a class="dropdown-item" href="#" onclick="handleAction('assignToAnother', ${member.memberID})">Assign to another</a></li>
                                    <li><a class="dropdown-item" href="#" onclick="handleAction('editUser', ${member.memberID})">Edit user</a></li>
                                    <li><a class="dropdown-item" href="#" onclick="handleAction('addVisitationRequest', ${member.memberID})">Visitation Request</a></li>
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
    if (action === 'addVisitationRequest') {
        openVisitationModal(memberId);
    } else {
        // Keep existing placeholder for other actions
        alert(`Functionality for '${action}' on member ID ${memberId} is not implemented yet.`);
    }
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

async function openVisitationModal(memberId) {
    // Reset Fields
    document.getElementById('servantSelect').value = "";
    document.getElementById('visitationNotes').value = '';
    document.getElementById('visitationType').value = 'Phone';

    if (memberId) {
        // Single Mode
        document.getElementById('visitationMemberID').value = memberId;
        // Update modal title for single user
        const modalTitle = document.querySelector('#addVisitationModal .modal-title');
        if (modalTitle) modalTitle.textContent = "Add Visitation Request";
    } else {
        // Bulk Mode
        document.getElementById('visitationMemberID').value = ""; // Clear ID
        // Update modal title for bulk
        const modalTitle = document.querySelector('#addVisitationModal .modal-title');
        if (modalTitle) modalTitle.textContent = `Add Visitation for ${selectedMemberIDs.size} Members`;
    }

    if (cachedServants.length === 0) {
        await fetchActiveServants();
    }

    const modal = new bootstrap.Modal(document.getElementById('addVisitationModal'));
    modal.show();
}

async function fetchActiveServants() {
    // Correcting typo from prompt: Servabts -> Servants, isAcive -> isActive
    const url = `${apiBaseUrl}/Servants/get-servants?isActive=true`;

    try {
        const response = await fetch(url, {
            method: "GET",
            credentials: "include",
            headers: { "Content-Type": "application/json" } // Adjust headers as per your auth requirements
        });

        if (response.ok) {
            cachedServants = await response.json();
            populateServantDropdown(cachedServants);
        } else {
            console.error("Failed to fetch servants");
        }
    } catch (error) {
        console.error("Error fetching servants:", error);
    }
}

function populateServantDropdown(servants) {
    const select = document.getElementById('servantSelect');

    // Reset and add default option
    select.innerHTML = '<option value="" selected disabled>Select a Servant...</option>';

    servants.forEach(servant => {
        const option = document.createElement('option');
        option.value = servant.servantID; // Value is the ID
        option.textContent = servant.servantName; // Display Text is the Name
        select.appendChild(option);
    });
}

async function submitVisitationRequest() {
    const singleMemberID = document.getElementById('visitationMemberID').value;
    const servantID = parseInt(document.getElementById('servantSelect').value);
    const type = document.getElementById('visitationType').value;
    const notes = document.getElementById('visitationNotes').value;
    const classID = parseInt(document.getElementById('classID').value);

    // Validation
    if (!servantID || isNaN(servantID)) {
        alert("Please select a servant.");
        return;
    }

    // Determine target IDs
    let targetIDs = [];
    if (singleMemberID) {
        // Single Mode: just the one ID
        targetIDs.push(parseInt(singleMemberID));
    } else {
        // Bulk Mode: get all from the Set
        targetIDs = Array.from(selectedMemberIDs);
    }

    if (targetIDs.length === 0) {
        alert("No members selected.");
        return;
    }

    // Construct Payload for the new Bulk API
    const payload = {
        MemberIDs: targetIDs,
        AssignedServant: servantID,
        ClassID: classID,
        VisitationType: type,
        VisitationNotes: notes
    };

    // Point to the new PLURAL endpoint
    const url = `${apiBaseUrl}/api/Visitation/create-visitations`;

    try {
        if (typeof showLoading === 'function') showLoading();

        const response = await fetch(url, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            credentials: "include",
            body: JSON.stringify(payload)
        });

        if (response.ok) {
            // Close Modal
            const modalEl = document.getElementById('addVisitationModal');
            const modal = bootstrap.Modal.getInstance(modalEl);
            modal.hide();

            showSuccessToast("Successfully added visitation request(s)");

            // If this was a bulk action, clear the UI selection
            if (!singleMemberID) {
                document.getElementById('selectAllMembers').checked = false;
                selectedMemberIDs.clear();
                updateBulkUI();

                // Uncheck all visible boxes
                document.querySelectorAll('.member-checkbox').forEach(cb => cb.checked = false);
            }
        } else {
            const errorText = await response.text();
            alert("Failed to add visitations: " + errorText);
        }
    } catch (error) {
        console.error("Error creating visitations:", error);
        alert("An error occurred while creating the requests.");
    } finally {
        if (typeof hideLoading === 'function') hideLoading();
    }
}

function toggleMemberSelection(memberID, checkbox) {
    if (checkbox.checked) {
        selectedMemberIDs.add(memberID);
    } else {
        selectedMemberIDs.delete(memberID);
    }
    updateBulkUI();
}

function toggleSelectAll() {
    const isChecked = document.getElementById('selectAllMembers').checked;

    // We only select/deselect the *visible* members (filtered list)
    // To do this correctly, we grab the checkboxes currently in the grid
    const visibleCheckboxes = document.querySelectorAll('.member-checkbox');

    visibleCheckboxes.forEach(cb => {
        cb.checked = isChecked;
        const id = parseInt(cb.value);
        if (isChecked) {
            selectedMemberIDs.add(id);
        } else {
            selectedMemberIDs.delete(id);
        }
    });

    updateBulkUI();
}

function updateSelectAllCheckboxState(visibleMembers) {
    // If we have visible members and ALL of them are in the selected Set, check "Select All"
    const selectAllCb = document.getElementById('selectAllMembers');
    if (visibleMembers.length > 0 && visibleMembers.every(m => selectedMemberIDs.has(m.memberID))) {
        selectAllCb.checked = true;
        selectAllCb.indeterminate = false;
    } else if (visibleMembers.some(m => selectedMemberIDs.has(m.memberID))) {
        selectAllCb.checked = false;
        selectAllCb.indeterminate = true;
    } else {
        selectAllCb.checked = false;
        selectAllCb.indeterminate = false;
    }
}

function updateBulkUI() {
    const count = selectedMemberIDs.size;
    const countSpan = document.getElementById('selectedCount');
    if (countSpan) countSpan.textContent = count;

    const actionsContainer = document.getElementById('bulkActionsContainer');
    if (actionsContainer) {
        if (count > 0) {
            actionsContainer.classList.remove('d-none');
        } else {
            actionsContainer.classList.add('d-none');
        }
    }
}

function handleBulkAction(action) {
    if (selectedMemberIDs.size === 0) return;

    if (action === 'bulk-visitation') {
        // Pass null to indicate bulk mode
        openVisitationModal(null);
    } else if (action === 'bulk-baptism') {
        alert(`Update Baptism functionality for ${selectedMemberIDs.size} members is not implemented yet.`);
    } else if (action === 'bulk-remove') {
        if (confirm(`Are you sure you want to remove ${selectedMemberIDs.size} members from this class?`)) {
            alert("Remove functionality is not implemented yet.");
        }
    }
}

