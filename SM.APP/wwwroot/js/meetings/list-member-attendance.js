async function loadMemberAttendancePartial(memberID) {
    try {
        const response = await fetch(`${apiBaseUrl}/Member/GetMemberClassOverviews?memberID=${memberID}`, { credentials: 'include' });
        if (response.ok) {
            const memberClasses = await response.json();
            populateAttendanceHistory(memberClasses);
        }
    } catch (error) {
        console.error('Error loading funds history:', error);
        throw error;
    }
}
function populateAttendanceHistory(memberClasses) {
    const attendanceList = document.getElementById('attendanceHistoryList');
    if (!memberClasses || memberClasses.length === 0) {
        attendanceList.innerHTML = '<div class="list-group-item text-center text-muted">No class attendance data available</div>';
        return;
    }
    attendanceList.innerHTML = memberClasses.map(function (classData) {
        let percentage = 0, badgeClass = 'bg-secondary', attendanceText = 'N/A';
        if (classData.attendance && classData.attendance.includes('/')) {
            const [attended, total] = classData.attendance.split('/').map(n => parseInt(n));
            if (total > 0) {
                percentage = Math.round((attended / total) * 100);
                if (percentage >= 80) badgeClass = 'bg-success';
                else if (percentage >= 50) badgeClass = 'bg-warning text-dark';
                else badgeClass = 'bg-danger';
            }
            attendanceText = classData.attendance;
        }
        const lastPresent = classData.lastPresentDate ? formatDate(classData.lastPresentDate) : 'Never';
        return `
                        <div class="list-group-item">
                            <div class="d-flex w-100 justify-content-between mb-1">
                                <h6 class="mb-0">${classData.className || ''}</h6>
                                <span class="badge bg-light text-dark align-self-center fs-6 fw-bold">${attendanceText}</span>
                            </div>
                            <div class="progress mb-1" style="height: 20px;">
                                <div class="progress-bar ${badgeClass}" role="progressbar" style="width: ${percentage}%;" aria-valuenow="${percentage}" aria-valuemin="0" aria-valuemax="100">${percentage}%</div>
                            </div>
                            <small class="text-muted">Last Attended: ${lastPresent}</small>
                        </div>`;
    }).join('');
}