async function loadMemberAidsPartial(memberID) {
    try {
        const response = await fetch(`${apiBaseUrl}/Member/GetMemberAids?memberID=${memberID}`, { credentials: 'include' });
        if (response.ok) {
            const aids = await response.json();
            populateAidsHistory(aids);
        }
    } catch (error) {
        console.error('Error loading funds history:', error);
        throw error;
    }
}
function populateAidsHistory(aids) {
    const aidsList = document.getElementById('aidsHistoryList');
    if (!aidsList) return;

    if (aids && aids.length > 0) {
        // **FIXED**: The logic was inadvertently removed in the previous update. This is the corrected version.
        aidsList.innerHTML = aids.map(aid => `
            <div class="list-group-item d-flex justify-content-between align-items-center">
                <span>${aid.aid?.aidName || 'N/A'}</span>
                <small class="text-muted fw-bold">${formatDate(aid.timeStamp)}</small>
            </div>
        `).join('');
    } else {
        aidsList.innerHTML = '<div class="list-group-item text-center text-muted"><i class="bi bi-bandaid fa-lg me-2"></i>No aids history available</div>';
    }
}