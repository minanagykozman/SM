// MODIFICATION: allMembers is no longer needed to store the full list
let allClasses = [];
let selectedMemberIDs = new Set();

// DOM Elements
const elements = {
    mainContainer: document.getElementById('main-container'),
    codeSearchInput: document.getElementById('code-search-input'),
    // MODIFICATION: Added button for code search
    codeSearchBtn: document.getElementById('code-search-btn'),
    // Advanced filter elements
    advancedFilterForm: document.getElementById('advanced-filter-form'),
    firstNameFilter: document.getElementById('first-name-filter'),
    lastNameFilter: document.getElementById('last-name-filter'),
    activeFilter: document.getElementById('active-filter'),
    baptisedFilter: document.getElementById('baptised-filter'),
    cardStatusFilter: document.getElementById('card-status-filter'),
    classFilter: document.getElementById('class-filter'),
    birthdateStartFilter: document.getElementById('birthdate-start-filter'),
    birthdateEndFilter: document.getElementById('birthdate-end-filter'),
    resetFiltersBtn: document.getElementById('reset-filters-btn'),
    applyFiltersBtn: document.getElementById('apply-filters-btn'),
    notInClassFilter: document.getElementById('not-in-class-filter'),
    classFilterOperator: document.getElementById('class-filter-operator'),
    // Bulk actions elements
    gridControls: document.getElementById('grid-controls'),
    bulkActionsDropdown: document.getElementById('bulk-actions-dropdown'),
    selectAllCheckbox: document.getElementById('select-all-checkbox'),
    selectedCount: document.getElementById('selected-count'),
    // Grid and main UI
    membersGrid: document.getElementById('members-grid'),
    counter: document.getElementById('counter'),
    searchTabs: null // Initialized later
};

/**
 * Initializes the page: fetches data, sets up event listeners.
 */
document.addEventListener("DOMContentLoaded", async () => {
    try {
        const codeSearchTabEl = document.getElementById('code-search-tab');
        showLoading();
        if (codeSearchTabEl) {
            elements.searchTabs = new bootstrap.Tab(codeSearchTabEl);
        }

        initializeScanning();
        // MODIFICATION: Removed fetchMembers() from initial load
        await fetchClasses();
        setupEventListeners();

        // Start with an empty grid, prompting user to search
        populateMembersGrid([], true);
        toggleFormInputs(document.getElementById('filter-search-pane'), false);
        elements.codeSearchInput.focus();
    } catch (error) {
        console.error("Initialization failed:", error);
        showErrorMessage("Failed to load initial data. Please refresh the page.");
    } finally {
        hideLoading();
    }
});

/**
 * Fetches the list of classes and defensively initializes the selectpicker.
 */
async function fetchClasses() {
    const response = await fetch(`${apiBaseUrl}/Meeting/GetServantClasses`, {
        method: "GET",
        credentials: "include",
        headers: { "Content-Type": "application/json" }
    });
    if (!response.ok) throw new Error(`Failed to fetch classes: ${response.statusText}`);
    allClasses = await response.json();

    elements.classFilter.innerHTML = '';
    
    allClasses.forEach(cls => {
        const option = document.createElement("option");
        option.value = cls.classID;
        option.textContent = cls.className;
        elements.classFilter.appendChild(option);
    });

    $('#class-filter').select2({
        placeholder: "Select classes",
        allowClear: true,
        width: '100%' // Ensures proper styling with Bootstrap
    });

    
}

/**
 * Sets up all event listeners for the page.
 */
function setupEventListeners() {
    // Toggle search modes
    const tabs = document.querySelectorAll('#searchTabs button');
    tabs.forEach(tab => tab.addEventListener('shown.bs.tab', handleTabChange));

    // MODIFICATION: Changed code search to trigger on button click and 'Enter' key
    elements.codeSearchBtn.addEventListener('click', handleCodeSearch);
    elements.codeSearchInput.addEventListener('keyup', (event) => {
        if (event.key === 'Enter') {
            handleCodeSearch();
        }
    });

    // Advanced Filters (triggers on button click)
    elements.applyFiltersBtn.addEventListener('click', applyAdvancedFilters);
    elements.resetFiltersBtn.addEventListener('click', resetAdvancedFilters);

    // Class filter UI logic
    elements.classFilterOperator.addEventListener('change', () => {
        const label = document.getElementById('class-filter-operator-label');
        label.textContent = elements.classFilterOperator.checked ? 'Attending Any Class' : 'Attending All Claasses';
    });
    elements.notInClassFilter.addEventListener('change', () => {
        const isChecked = elements.notInClassFilter.checked;
        if (isChecked) {
            $('#class-filter').selectpicker('val', ''); // Clear selection
            $('#class-filter').prop('disabled', true).selectpicker('refresh');
            elements.classFilterOperator.disabled = true;
        } else {
            $('#class-filter').prop('disabled', false).selectpicker('refresh');
            elements.classFilterOperator.disabled = false;
        }
    });
    $('#class-filter').on('change.bs.select', function () {
        const selectedClasses = $(this).val();
        if (selectedClasses && selectedClasses.length > 0) {
            elements.notInClassFilter.checked = false;
            elements.notInClassFilter.dispatchEvent(new Event('change'));
        }
    });

    // Delegated listener for member selection checkboxes
    elements.membersGrid.addEventListener('change', (event) => {
        if (event.target.classList.contains('member-select-checkbox')) {
            const memberId = parseInt(event.target.value, 10);
            if (event.target.checked) {
                selectedMemberIDs.add(memberId);
            } else {
                selectedMemberIDs.delete(memberId);
            }
            updateBulkActionsUI();
        }
    });

    // Listener for "Select All" checkbox
    elements.selectAllCheckbox.addEventListener('change', (event) => {
        const isChecked = event.target.checked;
        const visibleCheckboxes = elements.membersGrid.querySelectorAll('.member-select-checkbox');

        visibleCheckboxes.forEach(checkbox => {
            const memberId = parseInt(checkbox.value, 10);
            checkbox.checked = isChecked;
            if (isChecked) {
                selectedMemberIDs.add(memberId);
            } else {
                selectedMemberIDs.delete(memberId);
            }
        });
        updateBulkActionsUI();
    });

    // Corrected bulk action listener
    elements.gridControls.addEventListener('click', (event) => {
        const actionLink = event.target.closest('a[data-action]');
        if (!actionLink) {
            return;
        }

        event.preventDefault();
        const action = actionLink.dataset.action;
        const memberIds = Array.from(selectedMemberIDs);

        if (memberIds.length === 0) {
            alert("Please select at least one member.");
            return;
        }

        switch (action) {
            case 'print-card':
                bulkPrintCards(memberIds,"Standard");
                break;
            case 'update-baptism':
                bulkUpdateBaptism(memberIds);
                break;
            case 'delete-members':
                bulkDeleteMembers(memberIds);
                break;
        }
    });
}

/**
 * Manages UI state when switching between search tabs.
 */
function handleTabChange(event) {
    const isCodeSearch = event.target.id === 'code-search-tab';
    toggleFormInputs(document.getElementById('code-search-pane'), isCodeSearch);
    toggleFormInputs(document.getElementById('filter-search-pane'), !isCodeSearch);
    populateMembersGrid([], true); // Clear grid on tab switch
    elements.codeSearchInput.value = '';
    resetAdvancedFilters(false);
    if (isCodeSearch) {
        setTimeout(() => elements.codeSearchInput.focus(), 150);
    }
}

/**
 * Enables or disables all form inputs within a given container.
 */
function toggleFormInputs(container, enable) {
    const inputs = container.querySelectorAll('input, select, button');
    inputs.forEach(input => {
        if (input.id === 'class-filter') {
            $(input).prop('disabled', !enable);
            setTimeout(() => $(input).selectpicker('refresh'), 50);
        } else {
            input.disabled = !enable;
        }
    });
}

async function handleCodeSearch() {
    clearSelection();
    const searchTerm = elements.codeSearchInput.value.trim();
    if (!searchTerm) {
        populateMembersGrid([], true);
        return;
    }

    showLoading();
    try {
        const params = new URLSearchParams({ term: searchTerm });
        const response = await fetch(`${apiBaseUrl}/Member/SearchByCode?${params}`, {
            method: "GET",
            credentials: "include",
            headers: { "Content-Type": "application/json" }
        });

        if (!response.ok) {
            throw new Error(`Server error: ${response.statusText}`);
        }

        const members = await response.json();
        populateMembersGrid(members);
    } catch (error) {
        console.error("Failed to search by code:", error);
        showErrorMessage("An error occurred while searching. Please try again.");
        populateMembersGrid([]); // Show empty grid on error
    } finally {
        hideLoading();
    }
}

async function applyAdvancedFilters() {
    clearSelection();

    // Build the search criteria object to send to the server
    const searchCriteria = {
        firstName: elements.firstNameFilter.value.trim() || null,
        lastName: elements.lastNameFilter.value.trim() || null,
        isActive: elements.activeFilter.value === 'all' ? null : (elements.activeFilter.value === 'true'),
        isBaptised: elements.baptisedFilter.value === 'all' ? null : (elements.baptisedFilter.value === 'true'),
        cardStatus: elements.cardStatusFilter.value === 'all' ? null : elements.cardStatusFilter.value,
        classIDs: $('#class-filter').val() || [],
        birthdateStart: elements.birthdateStartFilter.value || null,
        birthdateEnd: elements.birthdateEndFilter.value || null,
        isNotInAnyClass: elements.notInClassFilter.checked,
        classOperatorIsOr: elements.classFilterOperator.checked
    };

    showLoading();
    try {
        const response = await fetch(`${apiBaseUrl}/Member/Search`, {
            method: "POST",
            credentials: "include",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(searchCriteria)
        });

        if (!response.ok) {
            throw new Error(`Server error: ${response.statusText}`);
        }

        const members = await response.json();
        populateMembersGrid(members);
    } catch (error) {
        console.error("Failed to apply advanced filters:", error);
        showErrorMessage("An error occurred while searching. Please try again.");
        populateMembersGrid([]);
    } finally {
        hideLoading();
    }
}

/**
 * Intialize scanning component
 */
function initializeScanning() {
    const scanButton = document.getElementById("scanMemberCodeBtnSearch");
    const qrScannerContainer = document.getElementById("qrScannerContainerSearch");
    const stopScanButton = document.getElementById("btnStopScanSearch");
    const codeInput = document.getElementById("code-search-input");
    if (scanButton) {
        let html5QrCode;
        scanButton.addEventListener("click", function () {
            qrScannerContainer.classList.remove("d-none");
            html5QrCode = new Html5Qrcode("reader");
            html5QrCode.start({
                facingMode: "environment"
            }, {
                fps: 10,
                qrbox: { width: 250, height: 250 }
            }, (decodedText) => {
                codeInput.value = decodedText;
                html5QrCode.stop().then(() => {
                    qrScannerContainer.classList.add("d-none");
                    handleCodeSearch();
                });
            }, (errorMessage) => {
                 console.warn("QR Code scan error: ", errorMessage);
            }).catch((err) => {
                console.error("QR Code scanning failed: ", err);
            });
        });
        stopScanButton.addEventListener("click", function () {
            if (html5QrCode && html5QrCode.isScanning) {
                html5QrCode.stop().then(() => {
                    qrScannerContainer.classList.add("d-none");
                });
            }
        });
    }
}

/**
 * Resets all advanced filters to their default state.
 */
function resetAdvancedFilters(triggerSearch = true) {
    clearSelection();
    elements.advancedFilterForm.reset();
    $('#class-filter').selectpicker('val', '');
    elements.notInClassFilter.disabled = false;
    $('#class-filter').prop('disabled', false).selectpicker('refresh');
    elements.classFilterOperator.disabled = false;
    document.getElementById('class-filter-operator-label').textContent = 'Match ANY (OR)';
    if (triggerSearch) {
        populateMembersGrid([], true);
    }
}

/**
 * Renders the member cards in the grid.
 */
function populateMembersGrid(members, showInitialMessage = false) {
    elements.membersGrid.innerHTML = "";
    elements.counter.textContent = `(${members.length} Members)`;
    if (showInitialMessage) {
        elements.membersGrid.innerHTML = `<div class="col-12"><div class="alert alert-info text-center"><i class="bi bi-search me-2"></i>Please use a search method to find members.</div></div>`;
        elements.counter.textContent = '';
        elements.gridControls.classList.add('d-none');
        return;
    }
    if (members.length === 0) {
        elements.membersGrid.innerHTML = '<div class="col-12"><div class="alert alert-warning text-center">No members found matching your search criteria.</div></div>';
        elements.gridControls.classList.add('d-none');
        return;
    }
    elements.gridControls.classList.remove('d-none');
    members.forEach(member => {
        const isSelected = selectedMemberIDs.has(member.memberID);
        let editButtonHtml = '';
        let deleteButtonHtml = '';
        if (member.permissions["canEdit"]) {
            editButtonHtml = `<li><a href="#" class="dropdown-item" onclick="showEditMemberModal(${member.memberID})">Edit</a></li>`;
        }
        if (member.permissions["canDelete"]) {
            deleteButtonHtml = `<li>
                                  <hr class="dropdown-divider">
                                </li>
                                <li><a href="#" class="dropdown-item text-danger" onclick="showMemberDeleteModal(${member.memberID}, null)">Delete</a></li>`;
        }
        const memberCard = `<div class="col-xl-4 col-lg-6 col-12">
   <div class="card h-100 shadow-sm position-relative">
      <div class="position-absolute top-0 start-0 p-2" style="z-index: 10;">
         <input class="form-check-input member-select-checkbox" type="checkbox" value="${member.memberID}" ${isSelected ? 'checked' : ''}>
      </div>
      <div class="card-body">
         <div class="d-flex justify-content-between align-items-start">
            <h5 class="card-title mb-1 ms-4">${member.fullName || 'N/A'}</h5>
            <div class="dropdown">
               <button class="btn btn-sm btn-light" type="button" data-bs-toggle="dropdown" aria-expanded="false"><i class="bi bi-three-dots-vertical"></i></button>
               <ul class="dropdown-menu dropdown-menu-end">
                  <li><a href="#" class="dropdown-item" onclick="showMemberDetailsModal(${member.memberID}, null)">View</a></li>
                  ${editButtonHtml}
                  ${deleteButtonHtml}
               </ul>
            </div>
         </div>
         <div class="card-text">
            <p class="mb-1"><small class="text-muted"><strong>Code:</strong> ${member.code || 'N/A'}</small></p>
            <p class="mb-1"><strong>UN File #:</strong> ${member.unFileNumber || 'N/A'}</p>
            <p class="mb-2"><strong>UN Personal #:</strong> ${member.unPersonalNumber || 'N/A'}</p>
            <p class="mb-2"><strong>Card Prints #:</strong> ${member.cardDeliveryCount || 'N/A'}</p>
            <span class="badge bg-primary me-2">${member.gender === 'M' ? 'Male' : 'Female'}</span><span class="badge ${member.baptised ? 'bg-success' : 'bg-secondary'} me-2">Baptised: ${member.baptised ? 'Yes' : 'No'}</span><span class="badge bg-info text-dark">${member.cardStatus || 'N/A'}</span>${member.mobile ? `
            <div class="mt-3 d-grid"><a href="tel:${member.mobile}" class="btn btn-outline-success btn-sm"><i class="bi bi-telephone-fill me-2"></i> ${member.mobile}</a></div>
            ` : ''}
         </div>
      </div>
   </div>
</div>`;
        elements.membersGrid.insertAdjacentHTML('beforeend', memberCard);
    });
    updateBulkActionsUI();
}

/**
 * Shows or hides the bulk actions panel and updates counts.
 */
function updateBulkActionsUI() {
    const count = selectedMemberIDs.size;
    if (count > 0) {
        elements.bulkActionsDropdown.classList.remove('d-none');
        elements.selectedCount.textContent = count;
    } else {
        elements.bulkActionsDropdown.classList.add('d-none');
    }
    const visibleCheckboxes = elements.membersGrid.querySelectorAll('.member-select-checkbox');
    if (visibleCheckboxes.length > 0) {
        elements.selectAllCheckbox.checked = count > 0 && count === visibleCheckboxes.length;
        elements.selectAllCheckbox.indeterminate = count > 0 && count < visibleCheckboxes.length;
    } else {
        elements.selectAllCheckbox.checked = false;
        elements.selectAllCheckbox.indeterminate = false;
    }
}

/**
 * Clears all selected members and updates the UI.
 */
function clearSelection() {
    selectedMemberIDs.clear();
    updateBulkActionsUI();
}



function bulkUpdateBaptism(memberIds) {
    console.log("Updating baptism status for member IDs:", memberIds);
    alert(`Updating baptism status for ${memberIds.length} members.`);
}

function bulkDeleteMembers(memberIds) {
    console.log("Deleting member IDs:", memberIds);
    if (confirm(`Are you sure you want to delete ${memberIds.length} members?`)) {
        alert('Deletion logic goes here.');
    }
}

function showErrorMessage(message) { alert(`ERROR: ${message}`); }
function showMemberDeleteModal(memberID, _) { alert(`Showing delete confirmation for member ID: ${memberID}`); }