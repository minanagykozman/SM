function renderGrid(classes) {
    const grid = document.getElementById('classes-grid');
    const counter = document.getElementById('counter');
    if (!grid) return;

    grid.innerHTML = "";
    if (counter) counter.textContent = `(${classes.length})`;

    if (classes.length === 0) {
        grid.innerHTML = '<div class="col-12"><div class="alert alert-light text-center border">No classes match your criteria.</div></div>';
        return;
    }

    classes.forEach(c => {
        const statusBadge = c.isActive
            ? '<span class="badge bg-success">Active</span>'
            : '<span class="badge bg-secondary">Inactive</span>';

        let genderIcon = "bi-gender-ambiguous";
        let genderText = "Any";
        if (c.gender === 'M') { genderIcon = "bi-gender-male text-primary"; genderText = "Male"; }
        else if (c.gender === 'F') { genderIcon = "bi-gender-female text-danger"; genderText = "Female"; }

        const scheduleText = (c.classDay || c.classStartTime || c.classEndTime)
            ? `${c.classDay || 'TBD'} ${c.classStartTime || ''} - ${c.classEndTime || ''}`.trim()
            : 'Schedule Not Set';

        const card = `
        <div class="col-xl-4 col-lg-6 col-md-6 col-sm-12">
            <div class="card h-100 shadow-sm border-0 position-relative">
                
                <div class="position-absolute top-0 end-0 m-2 d-flex align-items-center gap-1">
                    ${statusBadge}
                    
                    <div class="dropdown">
                        <button class="btn btn-sm btn-light rounded-circle p-0" type="button" data-bs-toggle="dropdown" aria-expanded="false" style="width: 24px; height: 24px;">
                            <i class="bi bi-three-dots-vertical"></i>
                        </button>
                        <ul class="dropdown-menu dropdown-menu-end shadow">
                            <li><a class="dropdown-item" href="javascript:void(0)" onclick="openEditModal(${c.classID})"><i class="bi bi-pencil me-2"></i>Edit Class</a></li>
                            <li><a class="dropdown-item" href="javascript:void(0)" onclick="openAutoAssignModal(${c.classID})"><i class="bi bi-people-fill me-2"></i>Auto Assign Members</a></li>
                            <li><hr class="dropdown-divider"></li>
                            <li><a class="dropdown-item text-primary" href="javascript:void(0)" onclick="openCreateMeetingsModal(${c.classID})"><i class="bi bi-calendar-plus me-2"></i>Create Meetings</a></li>
                        </ul>
                    </div>
                </div>

                <div class="card-body pt-4">
                    <div style="width: calc(100% - 85px);">
                        <h5 class="card-title fw-bold text-primary mb-2 text-wrap text-break">
                            ${c.className}
                        </h5>
                    </div>
                    
                    <div class="d-flex justify-content-between align-items-center mb-3">
                        <span class="badge bg-light text-dark border"><i class="bi bi-calendar-event me-1"></i>Year: ${c.year}</span>
                        <span class="badge bg-light text-dark border"><i class="bi ${genderIcon} me-1"></i>${genderText}</span>
                    </div>
                    
                    <hr class="my-2">

                    <div class="row g-2 small mb-2">
                        <div class="col-12">
                            <span class="text-muted"><i class="bi bi-clock me-1"></i></span>
                            <span class="fw-medium">${scheduleText}</span>
                            ${c.classFrequency ? `<span class="badge bg-info text-dark ms-1">${c.classFrequency}</span>` : ''}
                        </div>
                        
                        <div class="col-6">
                            <span class="text-muted d-block">Age Starts:</span>
                            <span class="fw-medium">${formatDate(c.ageStartDate) || 'N/A'}</span>
                        </div>
                        <div class="col-6">
                            <span class="text-muted d-block">Age Ends:</span>
                            <span class="fw-medium">${formatDate(c.ageEndDate) || 'N/A'}</span>
                        </div>
                    </div>
                </div>
            </div>
        </div>`;

        grid.insertAdjacentHTML('beforeend', card);
    });
}

// --- Create & Edit Logic ---

function openCreateModal() {
    document.getElementById('classForm').reset();
    document.getElementById('classId').value = '';

    // Set Default Year to current
    document.getElementById('classYear').value = new Date().getFullYear();
    document.getElementById('classGender').value = 'A';
    document.getElementById('classFrequency').value = 'Weekly';

    document.getElementById('classModalTitle').textContent = 'Create Class';

    const modal = new bootstrap.Modal(document.getElementById('classModal'));
    modal.show();
}

function openEditModal(classID) {
    const cls = allClasses.find(c => c.classID === classID);
    if (!cls) return;

    document.getElementById('classForm').reset();

    document.getElementById('classId').value = cls.classID;
    document.getElementById('className').value = cls.className;
    document.getElementById('classYear').value = cls.year;
    document.getElementById('classGender').value = cls.gender;

    document.getElementById('ageStartDate').value = toInputDate(cls.ageStartDate);
    document.getElementById('ageEndDate').value = toInputDate(cls.ageEndDate);
    document.getElementById('classStartDate').value = toInputDate(cls.classStartDate);
    document.getElementById('classEndDate').value = toInputDate(cls.classEndDate);

    document.getElementById('classDay').value = cls.classDay || "";
    document.getElementById('classStartTime').value = cls.classStartTime || "";
    document.getElementById('classEndTime').value = cls.classEndTime || "";
    document.getElementById('classFrequency').value = cls.classFrequency || "Weekly";
    document.getElementById('classNotes').value = cls.notes || "";

    document.getElementById('classModalTitle').textContent = 'Edit Class';

    const modal = new bootstrap.Modal(document.getElementById('classModal'));
    modal.show();
}

async function saveClass() {
    // Basic HTML5 Validation fallback
    const form = document.getElementById('classForm');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    const classId = document.getElementById('classId').value;
    const isEdit = classId !== '';

    // Build Payload matching ClassDTO + optional ClassID
    const payload = {
        ClassID: isEdit ? parseInt(classId) : 0,
        ClassName: document.getElementById('className').value,
        Year: parseInt(document.getElementById('classYear').value),
        Gender: document.getElementById('classGender').value,
        AgeStartDate: document.getElementById('ageStartDate').value || null,
        AgeEndDate: document.getElementById('ageEndDate').value || null,
        ClassStartDate: document.getElementById('classStartDate').value || null,
        ClassEndDate: document.getElementById('classEndDate').value || null,
        ClassDay: document.getElementById('classDay').value,
        ClassStartTime: document.getElementById('classStartTime').value,
        ClassEndTime: document.getElementById('classEndTime').value,
        ClassFrequency: document.getElementById('classFrequency').value,
        Notes: document.getElementById('classNotes').value
    };

    const endpoint = isEdit ? '/Meeting/EditClass' : '/Meeting/CreateClass';
    const url = `${apiBaseUrl}${endpoint}`;

    try {
        if (typeof showLoading === 'function') showLoading();

        const response = await fetch(url, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            body: JSON.stringify(payload)
        });

        if (response.ok) {
            const modalEl = document.getElementById('classModal');
            bootstrap.Modal.getInstance(modalEl).hide();

            showSuccessToast(`Class ${isEdit ? 'updated' : 'created'} successfully!`);
            await fetchClasses(); // Refresh grid
        } else {
            const err = await response.text();
            showFailedToast(`Failed to save class: ${err}`);
        }
    } catch (e) {
        console.error(e);
        showFailedToast("An error occurred while saving the class.");
    } finally {
        if (typeof hideLoading === 'function') hideLoading();
    }
}

// --- Action Logic ---

function openAutoAssignModal(classID) {
    document.getElementById('autoAssignClassId').value = classID;
    const modal = new bootstrap.Modal(document.getElementById('autoAssignModal'));
    modal.show();
}
function openCreateMeetingsModal(classID) {
    document.getElementById('createMeetingsClassId').value = classID;
    const modal = new bootstrap.Modal(document.getElementById('createMeetingsModal'));
    modal.show();
}

async function confirmAutoAssign() {
    const classID = parseInt(document.getElementById('autoAssignClassId').value);
    const url = `${apiBaseUrl}/Meeting/auto-assign-class-members`;

    try {
        if (typeof showLoading === 'function') showLoading();

        const response = await fetch(url, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            // .NET [FromBody] int classID expects just the raw integer stringified
            body: JSON.stringify(classID)
        });

        const modalEl = document.getElementById('autoAssignModal');
        bootstrap.Modal.getInstance(modalEl).hide();

        if (response.ok) {
            showSuccessToast("Members successfully assigned based on birthdate rules.");
        } else {
            const err = await response.text();
            showFailedToast(`Failed to assign members: ${err}`);
        }
    } catch (e) {
        console.error(e);
        showFailedToast("An error occurred during auto-assignment.");
    } finally {
        if (typeof hideLoading === 'function') hideLoading();
    }
}

async function createMeetings() {
    const classID = parseInt(document.getElementById('createMeetingsClassId').value);
    const url = `${apiBaseUrl}/Meeting/create-class-occurences`;

    try {
        if (typeof showLoading === 'function') showLoading();

        const response = await fetch(url, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            body: JSON.stringify(classID)
        });
        const modalEl = document.getElementById('createMeetingsModal');
        bootstrap.Modal.getInstance(modalEl).hide();
        if (response.ok) {
            showSuccessToast("Class meetings/occurrences generated successfully.");
        } else {
            const err = await response.text();
            showFailedToast(`Failed to generate meetings: ${err}`);
        }
    } catch (e) {
        console.error(e);
        showFailedToast("An error occurred while generating meetings.");
    } finally {
        if (typeof hideLoading === 'function') hideLoading();
    }
}