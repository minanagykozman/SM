async function loadMemberFundsPartial(memberID, fundID) {
    try {
        const response = await fetch(`${apiBaseUrl}/Member/GetMemberFunds?memberID=${memberID}`, { credentials: 'include' });
        if (response.ok) {
            const funds = await response.json();
            if (fundID) {
                const filteredFunds = funds.filter(fund => parseInt(fund.fundID) !== parseInt(this.fundId));
                populateFundsData(filteredFunds);
            } else
                populateFundsData(funds);
        }
    } catch (error) {
        console.error('Error loading funds history:', error);
        throw error;
    }
}

function populateFundsData(funds) {
    const fundsList = document.getElementById('fundsHistoryList');
    if (!funds || funds.length === 0) {
        fundsList.innerHTML = '<div class="list-group-item text-center text-muted">No funds history available</div>';
        return;
    }

    //const self = this;
    funds.sort((a, b) => new Date(b.requestDate) - new Date(a.requestDate));
    fundsList.innerHTML = funds.map(function (fund) {
        return `
                            <div class="list-group-item list-group-item-action flex-column align-items-start">
                                <div class="d-flex w-100 justify-content-between">
                                    <h5 class="mb-1">${fund.fundCategory || 'N/A'}</h5>
                                    <span class="badge ${getFundStatusBadgeClass(fund.status)} fs-6">${fund.statusName}</span>
                                </div>
                                <p class="mb-1">${fund.requestDescription || 'No description'}</p>
                                <div class="d-flex w-100 justify-content-between align-items-center mt-2">
                                    <small class="text-muted"><i class="bi bi-calendar-event me-1"></i> ${formatDate(fund.requestDate)}</small>
                                    <small><b>Requested:</b> ${formatCurrency(fund.requestedAmount)} | <b>Approved:</b> ${formatCurrency(fund.approvedAmount)}</small>
                                </div>
                            </div>`;
    }).join('');
}