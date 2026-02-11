let allVisitations = [];
let cachedServants = [];
let cachedClasses = [];

document.addEventListener("DOMContentLoaded", async function () {
    // 1. Fetch Lookups (Servants & Classes) in parallel
    await Promise.all([fetchServants(), fetchClasses()]);

    // 2. Fetch Main Data
    await fetchVisitations();

    // 3. Attach Event Listeners to all filters
    const filters = ['filterMember', 'filterStatus', 'filterClass', 'filterAssignedTo', 'filterCreatedBy'];
    filters.forEach(id => {
        const el = document.getElementById(id);
        if (el) {
            el.addEventListener('input', applyFilters);
            el.addEventListener('change', applyFilters);
        }
    });

    // 4. Attach Modal Listeners
    const btnUpdate = document.getElementById('btnPerformUpdate');
    if (btnUpdate) btnUpdate.addEventListener('click', submitUpdateVisitation);

    const statusSelect = document.getElementById('updateVisitationStatus');
    if (statusSelect) statusSelect.addEventListener('change', handleStatusChange);

    const typeSelect = document.getElementById('updateVisitationType');
    if (typeSelect) typeSelect.addEventListener('change', handleStatusChange);
});

// --- Helper: Date Formatter (dd/MM/yyyy) ---
function formatDate(isoString) {
    if (!isoString) return '';
    const d = new Date(isoString);
    if (isNaN(d.getTime())) return '';
    const day = String(d.getDate()).padStart(2, '0');
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const year = d.getFullYear();
    return `${day}/${month}/${year}`;
}

// --- Helper: Note Toggler for Read More ---
window.toggleNote = function (contentId, btnId) {
    const content = document.getElementById(contentId);
    const btn = document.getElementById(btnId);
    if (content.style.display !== 'block') {
        content.style.display = 'block';
        content.style.webkitLineClamp = 'unset';
        btn.innerHTML = 'Show less';
    } else {
        content.style.display = '-webkit-box';
        content.style.webkitLineClamp = '2';
        content.style.webkitBoxOrient = 'vertical';
        btn.innerHTML = 'Read more';
    }
}

// --- API Calls ---

async function fetchServants() {
    const url = `${apiBaseUrl}/Servants/get-servants?isActive=true`;
    try {
        const response = await fetch(url, { method: "GET", credentials: 'include', headers: { "Content-Type": "application/json" } });
        if (response.ok) {
            cachedServants = await response.json();
            populateServantDropdowns(cachedServants);
        }
    } catch (error) { console.error("Error fetching servants:", error); }
}

async function fetchClasses() {
    const url = `${apiBaseUrl}/Meeting/GetServantClasses/`;
    try {
        const response = await fetch(url, { method: "GET", credentials: 'include', headers: { "Content-Type": "application/json" } });
        if (response.ok) {
            cachedClasses = await response.json();
            populateClassDropdown(cachedClasses);
        }
    } catch (error) { console.error("Error fetching classes:", error); }
}

async function fetchVisitations() {
    const url = `${apiBaseUrl}/api/Visitation/get-visitations`;
    if (typeof showLoading === 'function') showLoading();
    try {
        const response = await fetch(url, { method: "GET", credentials: 'include', headers: { "Content-Type": "application/json" } });
        if (response.ok) {
            allVisitations = await response.json();
            applyFilters();
        } else {
            const grid = document.getElementById('visitations-grid');
            if (grid) grid.innerHTML = `<div class="col-12"><div class="alert alert-danger">Failed to load visitations.</div></div>`;
        }
    } catch (error) { console.error("Error fetching visitations:", error); }
    finally { if (typeof hideLoading === 'function') hideLoading(); }
}

// --- Population Logic ---

function populateServantDropdowns(servants) {
    const assignedSelect = document.getElementById('filterAssignedTo');
    const createdSelect = document.getElementById('filterCreatedBy');
    const updateSelect = document.getElementById('updateAssignedServant');
    const currentServantID = document.getElementById('currentServantID') ? document.getElementById('currentServantID').value : null;

    let optionsHtml = '';
    servants.forEach(s => { optionsHtml += `<option value="${s.servantID}">${s.servantName}</option>`; });

    if (assignedSelect) assignedSelect.insertAdjacentHTML('beforeend', optionsHtml);
    if (createdSelect) createdSelect.insertAdjacentHTML('beforeend', optionsHtml);
    if (updateSelect) {
        updateSelect.innerHTML = '';
        updateSelect.insertAdjacentHTML('beforeend', optionsHtml);
    }

    if (currentServantID && assignedSelect) assignedSelect.value = currentServantID;
}

function populateClassDropdown(classes) {
    const select = document.getElementById('filterClass');
    if (!select) return;
    classes.forEach(c => {
        const option = document.createElement('option');
        option.value = c.classID || c.ClassID;
        option.textContent = c.className || c.ClassName;
        select.appendChild(option);
    });
}

// --- Filtering & Rendering ---

function applyFilters() {
    const memberTxt = document.getElementById('filterMember').value.toLowerCase();
    const status = document.getElementById('filterStatus').value;
    const classId = document.getElementById('filterClass').value;
    const assignedId = document.getElementById('filterAssignedTo').value;
    const createdId = document.getElementById('filterCreatedBy').value;

    const filtered = allVisitations.filter(v => {
        const matchMember = !memberTxt ||
            (v.member.fullName && v.member.fullName.toLowerCase().includes(memberTxt)) ||
            (v.member.code && v.member.code.toString().includes(memberTxt));
        const matchStatus = !status || v.status === status;
        const matchClass = !classId || (v.classID != null && v.classID == classId);
        const matchAssigned = !assignedId || (v.assignedServantID != null && v.assignedServantID == assignedId);
        const matchCreated = !createdId || (v.servantID != null && v.servantID == createdId);
        return matchMember && matchStatus && matchClass && matchAssigned && matchCreated;
    });
    renderGrid(filtered);
}

function renderGrid(visitations) {
    const grid = document.getElementById('visitations-grid');
    const counter = document.getElementById('counter');
    if (!grid) return;

    grid.innerHTML = "";
    if (counter) counter.textContent = `(${visitations.length})`;

    if (visitations.length === 0) {
        grid.innerHTML = '<div class="col-12"><div class="alert alert-light text-center border">No visitations match your criteria.</div></div>';
        return;
    }

    visitations.forEach((v, index) => {
        let badgeClass = "bg-secondary";
        if (v.status === "Assigned") badgeClass = "bg-warning text-dark";
        else if (v.status === "Done") badgeClass = "bg-success";
        else if (v.status === "Cancelled") badgeClass = "bg-danger";
        else if (v.status === "Follow up needed") badgeClass = "bg-info text-dark";

        let createdByName = "System";
        if (v.servantID) {
            const creator = cachedServants.find(s => s.servantID === v.servantID);
            if (creator) createdByName = creator.servantName;
        }

        // Notes Logic: Prioritize AssignedServantFeedback, fall back to VisitationNotes if needed or empty string
        // Note: The UI label says "Notes", usually showing the initial request notes or the feedback?
        // Based on typical flows: The card usually shows the result (Feedback) if done, or request notes if open.
        // For now, I'll concatenate them or just show Feedback if available.
        // Let's assume we show Feedback if present (Done), otherwise Notes (Request).

        let displayNote = v.assignedServantFeedback;
        if (!displayNote) displayNote = v.visitationNotes; // Fallback to request notes if no feedback yet

        const noteText = displayNote || '';
        const uniqueId = `note-${v.visitationID}`; // Use ID instead of index to avoid collisions
        const isLong = noteText.length > 80;
        const clampedStyle = "display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden;";

        const card = `
        <div class="col-xl-4 col-lg-6 col-md-6 col-sm-12">
            <div class="card h-100 shadow-sm border-0 position-relative">
                
                <div class="position-absolute top-0 end-0 m-2 d-flex align-items-center gap-1">
                    <span class="badge rounded-pill ${badgeClass}">
                        ${v.status}
                    </span>
                    
                    <div class="dropdown">
                        <button class="btn btn-sm btn-light rounded-circle p-0" type="button" data-bs-toggle="dropdown" aria-expanded="false" style="width: 24px; height: 24px;">
                            <i class="bi bi-three-dots-vertical"></i>
                        </button>
                        <ul class="dropdown-menu dropdown-menu-end shadow">
                            <li><a class="dropdown-item" href="#" onclick="openUpdateModal(${v.visitationID})"><i class="bi bi-pencil me-2"></i>Update Request</a></li>
                            <li><a class="dropdown-item text-danger" href="#" onclick="deleteVisitation(${v.visitationID})"><i class="bi bi-trash me-2"></i>Delete Request</a></li>
                        </ul>
                    </div>
                </div>

                <div class="card-body pt-4">
                    <div class="mb-2">
                         <h5 class="card-title fw-bold text-primary mb-1 text-truncate" title="${v.member.fullName}">
                            ${v.member.fullName}
                        </h5>
                        <span class="badge bg-light text-dark border">Code: ${v.member.code || 'N/A'}</span>
                    </div>
                    
                    <hr class="my-2">

                    <div class="row g-2 small mb-2">
                        <div class="col-6">
                             <span class="text-muted d-block">Assigned To:</span>
                             <span class="fw-medium">${v.assignedServant ? v.assignedServant.servantName : 'Unassigned'}</span>
                        </div>
                        <div class="col-6">
                             <span class="text-muted d-block">Created By:</span>
                             <span class="fw-medium">${createdByName}</span>
                        </div>
                        
                        <div class="col-6">
                            <span class="text-muted d-block">Created At:</span>
                            <span>${formatDate(v.createdAt)}</span>
                        </div>
                        <div class="col-6">
                            <span class="text-muted d-block">Visitation Date:</span>
                            <span>${formatDate(v.visitationDate)}</span>
                        </div>
                    </div>
                    
                    ${noteText ? `
                    <div class="mb-3 bg-light p-2 rounded border-start border-3 border-info">
                        <small class="text-muted d-block fw-bold">Notes:</small>
                        <div id="content-${uniqueId}" class="small text-break" style="${isLong ? clampedStyle : ''}">
                            ${noteText}
                        </div>
                        ${isLong ? `
                        <a href="javascript:void(0)" id="btn-${uniqueId}" onclick="toggleNote('content-${uniqueId}', 'btn-${uniqueId}')" class="small text-primary text-decoration-none fw-bold mt-1 d-inline-block">Read more</a>
                        ` : ''}
                    </div>` : ''}

                    <div class="mt-auto d-flex gap-2">
                        ${v.member.mobile ? `
                        <a href="tel:${v.member.mobile}" class="btn btn-outline-primary flex-grow-1">
                            <i class="bi bi-telephone-fill me-2"></i> Call
                        </a>
                        <a href="https://wa.me/${v.member.mobile.replace(/\D/g, '')}" target="_blank" class="btn btn-success" title="WhatsApp">
                             <i class="bi bi-whatsapp"></i>
                        </a>
                        ` : `
                        <button class="btn btn-outline-secondary w-100" disabled>
                            <i class="bi bi-telephone-x me-2"></i> No Mobile
                        </button>
                        `}
                    </div>
                </div>
                
                <div class="card-footer bg-white border-top pt-2 pb-2">
                    <div class="d-flex justify-content-between align-items-center">
                        <small class="text-muted text-truncate" style="max-width: 60%;">
                            <i class="bi bi-people me-1"></i> ${v.class ? v.class.className : 'Unknown Class'}
                        </small>
                        <button class="btn btn-link btn-sm text-decoration-none p-0" 
                                onclick="toggleFamilyAttendance(this, ${v.member.memberID})"
                                aria-expanded="false">
                            Show Attendance <i class="bi bi-chevron-down ms-1"></i>
                        </button>
                    </div>
                    <div class="family-attendance-container mt-2 d-none border-top pt-2" id="family-container-${v.member.memberID}">
                        <div class="text-center py-2"><div class="spinner-border spinner-border-sm text-primary" role="status"></div> Loading...</div>
                    </div>
                </div>
            </div>
        </div>
        `;
        grid.insertAdjacentHTML('beforeend', card);
    });
}

// --- Update Modal Logic ---

// Opens the modal and sets initial state using ID (not index)
function openUpdateModal(id) {
    // Find the exact object from the full list
    const visitation = allVisitations.find(v => v.visitationID === id);
    if (!visitation) {
        console.error("Visitation not found for ID:", id);
        return;
    }

    // 1. Set Hidden IDs
    document.getElementById('updateVisitationID').value = visitation.visitationID;
    document.getElementById('updateMainMemberID').value = visitation.member.memberID;

    // 2. Set Assigned Servant
    const servantSelect = document.getElementById('updateAssignedServant');
    if (visitation.assignedServantID) {
        servantSelect.value = visitation.assignedServantID;
    }

    // 3. Set Visitation Type
    const typeSelect = document.getElementById('updateVisitationType');
    typeSelect.value = visitation.visitationType || "Phone";

    // 4. Set Status & Configure Options
    const statusSelect = document.getElementById('updateVisitationStatus');
    statusSelect.innerHTML = '';
    statusSelect.disabled = false;

    const currentStatus = visitation.status;
    let options = [];

    if (currentStatus === "Assigned" || currentStatus === "Open") {
        options = ["Assigned", "Cancelled", "Done", "Follow up needed"];
    } else if (currentStatus === "Cancelled") {
        options = ["Cancelled"];
        statusSelect.disabled = true;
    } else if (currentStatus === "Done") {
        options = ["Done"];
        statusSelect.disabled = true;
    } else if (currentStatus === "Follow up needed") {
        options = ["Follow up needed", "Done"];
    } else {
        options = [currentStatus, "Done"];
    }

    options.forEach(opt => {
        const optionEl = document.createElement('option');
        optionEl.value = opt;
        optionEl.textContent = opt;
        if (opt === currentStatus) optionEl.selected = true;
        statusSelect.appendChild(optionEl);
    });

    // 5. Pre-fill Feedback (using assignedServantFeedback) and Date
    document.getElementById('updateFeedback').value = visitation.assignedServantFeedback || "";

    if (visitation.visitationDate) {
        document.getElementById('updateVisitationDate').value = new Date(visitation.visitationDate).toISOString().split('T')[0];
    } else {
        document.getElementById('updateVisitationDate').value = new Date().toISOString().split('T')[0];
    }

    // 6. Trigger UI logic for hidden fields
    handleStatusChange();

    // 7. Show Modal
    const modal = new bootstrap.Modal(document.getElementById('updateVisitationModal'));
    modal.show();
}

async function handleStatusChange() {
    const status = document.getElementById('updateVisitationStatus').value;
    const type = document.getElementById('updateVisitationType').value;
    const container = document.getElementById('updateDetailsContainer');
    const checklistContainer = document.getElementById('familyChecklistContainer');

    // Show details if Done or Follow up needed
    if (status === "Done" || status === "Follow up needed") {
        container.classList.remove('d-none');

        // Show Checklist if Home
        if (type === "Home") {
            checklistContainer.classList.remove('d-none');
            // Fetch checklist if empty
            const itemsContainer = document.getElementById('familyChecklistItems');
            if (itemsContainer.innerHTML.includes('Loading')) {
                await loadFamilyChecklist();
            }
        } else {
            checklistContainer.classList.add('d-none');
        }
    } else {
        container.classList.add('d-none');
    }
}

async function loadFamilyChecklist() {
    const mainMemberID = document.getElementById('updateMainMemberID').value;
    const container = document.getElementById('familyChecklistItems');

    try {
        const url = `${apiBaseUrl}/api/Visitation/get-family-attendance?memberID=${mainMemberID}`;
        const response = await fetch(url, { method: "GET", credentials: 'include', headers: { "Content-Type": "application/json" } });

        if (response.ok) {
            const data = await response.json();
            container.innerHTML = '';

            // Flatten list: Main + Family
            let members = [];
            if (data.mainMember) members.push(data.mainMember);
            if (data.familyMembers) members = members.concat(data.familyMembers);

            if (members.length === 0) {
                container.innerHTML = '<span class="text-muted small">No family members found.</span>';
                return;
            }

            members.forEach(m => {
                const id = m.memberID || m.MemberID;
                const name = m.memberName || m.MemberName;

                const div = document.createElement('div');
                div.className = "form-check";
                div.innerHTML = `
                    <input class="form-check-input family-member-check" type="checkbox" value="${id}" id="chk_fam_${id}" checked>
                    <label class="form-check-label" for="chk_fam_${id}">
                        ${name}
                    </label>
                `;
                container.appendChild(div);
            });
        }
    } catch (e) {
        console.error(e);
        container.innerHTML = '<span class="text-danger small">Error loading family.</span>';
    }
}

async function submitUpdateVisitation() {
    const id = parseInt(document.getElementById('updateVisitationID').value);
    const feedback = document.getElementById('updateFeedback').value;
    const status = document.getElementById('updateVisitationStatus').value;
    const dateVal = document.getElementById('updateVisitationDate').value;
    const type = document.getElementById('updateVisitationType').value;

    let memberIDs = [];
    const checklistContainer = document.getElementById('familyChecklistContainer');

    if (!checklistContainer.classList.contains('d-none')) {
        document.querySelectorAll('.family-member-check:checked').forEach(cb => {
            memberIDs.push(parseInt(cb.value));
        });
    } else {
        memberIDs.push(parseInt(document.getElementById('updateMainMemberID').value));
    }

    const payload = {
        VisitationID: id,
        MemberIDs: memberIDs,
        Feedback: feedback,
        VisitationType:type,
        Status: status,
        VisitationDate: dateVal ? new Date(dateVal) : null
    };

    const url = `${apiBaseUrl}/api/visitation/update-visitation`;

    try {
        if (typeof showLoading === 'function') showLoading();

        const response = await fetch(url, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            body: JSON.stringify(payload)
        });

        if (response.ok) {
            const modalEl = document.getElementById('updateVisitationModal');
            const modal = bootstrap.Modal.getInstance(modalEl);
            modal.hide();
            await fetchVisitations();
            alert("Visitation updated successfully.");
        } else {
            const err = await response.text();
            alert("Failed to update: " + err);
        }
    } catch (e) {
        console.error(e);
        alert("Error updating visitation.");
    } finally {
        if (typeof hideLoading === 'function') hideLoading();
    }
}

function deleteVisitation(id) {
    if (confirm("Are you sure you want to delete this visitation request?")) {
        console.log("Delete triggered for ID:", id);
        alert("Delete functionality will be implemented later.");
    }
}

// --- Family Expand Logic ---

async function toggleFamilyAttendance(btn, memberID) {
    const container = document.getElementById(`family-container-${memberID}`);
    const isExpanded = btn.getAttribute('aria-expanded') === 'true';

    if (isExpanded) {
        container.classList.add('d-none');
        btn.innerHTML = 'Show Attendance <i class="bi bi-chevron-down ms-1"></i>';
        btn.setAttribute('aria-expanded', 'false');
    } else {
        container.classList.remove('d-none');
        btn.innerHTML = 'Hide Attendance <i class="bi bi-chevron-up ms-1"></i>';
        btn.setAttribute('aria-expanded', 'true');

        if (!container.querySelector('.list-group')) {
            await fetchFamilyAttendance(memberID, container);
        }
    }
}

async function fetchFamilyAttendance(memberID, container) {
    const url = `${apiBaseUrl}/api/Visitation/get-family-attendance?memberID=${memberID}`;
    try {
        const response = await fetch(url, { method: "GET", credentials: 'include', headers: { "Content-Type": "application/json" } });
        if (response.ok) {
            const data = await response.json();
            renderFamilyAttendance(data, container);
        } else {
            container.innerHTML = `<div class="alert alert-danger p-1 small mb-0">Failed to load attendance.</div>`;
        }
    } catch (error) {
        console.error("Error fetching family attendance:", error);
        container.innerHTML = `<div class="alert alert-danger p-1 small mb-0">Error loading data.</div>`;
    }
}

function renderFamilyAttendance(data, container) {
    if (!data) { container.innerHTML = `<div class="text-muted small text-center fst-italic">No data found.</div>`; return; }

    let html = `<ul class="list-group list-group-flush small">`;

    const renderMember = (member, isMain) => {
        if (!member) return '';
        const nameClass = isMain ? 'text-primary fw-bold' : 'text-dark fw-bold';
        let memberHtml = `
            <li class="list-group-item px-0 py-2">
                <div class="d-flex justify-content-between align-items-center mb-1">
                    <span class="${nameClass}">${member.memberName}</span>
                    <span class="text-muted small">Code: ${member.memberCode || 'N/A'}</span>
                </div>`;

        if (!member.attendance || member.attendance.length === 0) {
            memberHtml += `<div class="text-muted small fst-italic ms-1">There is no attendance data</div>`;
        } else {
            memberHtml += `<div class="ms-1 ps-2 border-start">`;
            member.attendance.forEach(att => {
                let percent = 0;
                if (typeof att.attendancePercentage === 'number') percent = att.attendancePercentage;
                else if (att.totalOccurrencesToDate > 0) percent = (att.attendedTimes / att.totalOccurrencesToDate) * 100;

                let badgeColor = 'bg-secondary';
                if (percent >= 75) badgeColor = 'bg-success';
                else if (percent >= 50) badgeColor = 'bg-warning text-dark';
                else badgeColor = 'bg-danger';
                const lastDate = formatDate(att.lastAttendanceDate) || 'Never';

                memberHtml += `
                    <div class="mb-2">
                        <div class="d-flex justify-content-between align-items-center">
                             <span class="small fw-medium">${att.className || 'No Class'}</span>
                             <span class="badge ${badgeColor}" style="font-size: 0.75em;">${percent.toFixed(0)}%</span>
                        </div>
                        <div class="d-flex justify-content-between text-muted" style="font-size: 0.85em;">
                            <span>${att.attendedTimes}/${att.totalOccurrencesToDate} Attended</span>
                            <span>Last: ${lastDate}</span>
                        </div>
                    </div>`;
            });
            memberHtml += `</div>`;
        }
        memberHtml += `</li>`;
        return memberHtml;
    };

    if (data.mainMember) html += renderMember(data.mainMember, true);
    if (data.familyMembers && data.familyMembers.length > 0) {
        if (data.mainMember) html += `<li class="list-group-item bg-light text-center py-1 text-muted fw-bold mt-2 mb-1" style="font-size:0.75rem;">FAMILY</li>`;
        data.familyMembers.forEach(fm => { html += renderMember(fm, false); });
    }
    html += `</ul>`;
    container.innerHTML = html;
}