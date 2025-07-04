function getFundStatusBadgeClass(status) {
    switch (status) {
        case 0: return 'bg-warning text-dark';
        case 1: return 'bg-success';
        case 2: return 'bg-danger';
        case 3: return 'bg-info';
        default: return 'bg-secondary';
    }
}
function formatCurrency(amount) {
    if (amount === null || amount === undefined) return 'N/A';
    return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'EGP' }).format(amount);
}