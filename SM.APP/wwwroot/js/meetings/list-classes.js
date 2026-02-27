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
                            <li><a class="dropdown-item" href="/Classes/ClassMembers?classID=${c.classID}"><i class="bi bi-people-fill me-2"></i>View Members</a></li>
                            <li><a class="dropdown-item" href="/Classes/ClassOccurances?classID=${c.classID}"><i class="bi bi-calendar me-2"></i>Take Attendance</a></li>
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