// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", function () {
    let myModal = document.getElementById("myModal");
    if (myModal) {
        document.getElementById("myModal").addEventListener("keypress", function (event) {
            if (event.key === "Enter") {
                event.preventDefault(); // Prevent default form submission

                const submitBtn = document.getElementById("submitBtn");
                if (submitBtn && window.getComputedStyle(submitBtn).display !== "none") {
                    submitBtn.click(); // Trigger the submit button click
                } else {
                    document.getElementById("btnClose").click(); // Close the modal
                }
            }
        });
    }
    let searchInput = document.getElementById("SearchString");
    if (searchInput) {
        searchInput.focus();
        document.getElementById("SearchString").addEventListener("keypress", function (event) {
            if (event.key === "Enter") {
                event.preventDefault();
                document.getElementById("btnCheck").click();
            }
        });
        var cancelBtn = document.getElementById("btnCancel");
        if (cancelBtn) {
            cancelBtn.addEventListener('click', function () {
                searchInput.value = "";
                searchInput.focus(); // Focus on search input when Cancel is clicked
            });
        }
        var btnClose = document.getElementById("btnClose");
        if (btnClose) {
            btnClose.addEventListener('click', function () {
                searchInput.value = "";
                searchInput.focus(); // Focus on search input when Cancel is clicked
            });
        }
        // Handle Esc key press to close the modal and clear input
        document.addEventListener('keydown', function (event) {
            if (event.key === 'Escape') {
                searchInput.value = "";
                searchInput.focus();
            }
        });


        const scanButton = document.getElementById("btnScanQR");
        const qrScannerContainer = document.getElementById("qrScannerContainer");
        const stopScanButton = document.getElementById("btnStopScan");
        if (scanButton) {

            let html5QrCode;
            scanButton.addEventListener("click", function () {
                qrScannerContainer.classList.remove("d-none");

                html5QrCode = new Html5Qrcode("reader");
                html5QrCode.start(
                    { facingMode: "environment" }, // Use back camera
                    {
                        fps: 10,
                        qrbox: { width: 250, height: 250 }
                    },
                    (decodedText) => {
                        searchInput.value = decodedText;
                        document.getElementById('btnCheck').click();

                        html5QrCode.stop();
                        qrScannerContainer.classList.add("d-none");
                    },
                    (errorMessage) => {
                        console.warn("QR Code scan error: ", errorMessage);
                    }
                ).catch((err) => {
                    console.error("QR Code scanning failed: ", err);
                });
            });
            // Stop scanning
            stopScanButton.addEventListener("click", function () {
                if (html5QrCode) {
                    html5QrCode.stop();
                }
                qrScannerContainer.classList.add("d-none");
            });
            // Show QR scanner on mobile only
            if (window.innerWidth <= 768) {

            }
        }
    }


    // Desktop Table Filtering
    let filter = document.querySelectorAll(".filter-input");
    if (filter) {
        document.querySelectorAll(".filter-input").forEach(input => {
            input.addEventListener("keyup", function () {
                let colIndex = input.getAttribute("data-column");
                let searchTerm = input.value.toLowerCase();
                let rows = document.querySelectorAll("#dataTable tbody tr");

                rows.forEach(row => {
                    let cell = row.cells[colIndex];
                    if (cell) {
                        let text = cell.innerText.toLowerCase();
                        row.style.display = text.includes(searchTerm) ? "" : "none";
                    }
                });
            });
        });
    }
    // Mobile List Filtering
    let mobileFilter = document.querySelector(".mobile-filter");
    if (mobileFilter) {
        document.querySelector(".mobile-filter").addEventListener("keyup", function () {
            let searchTerm = this.value.toLowerCase();
            let items = document.querySelectorAll("#mobileList .list-group-item");

            items.forEach(item => {
                let text = item.innerText.toLowerCase();
                item.style.display = text.includes(searchTerm) ? "" : "none";
            });
        });
    }
});

// Function to Sort Table Columns
function sortTable(columnIndex, thElement) {
    let table = document.getElementById("dataTable");
    let rows = Array.from(table.rows).slice(1); // Exclude first  row (header + filters)
    let ascending = thElement.getAttribute("data-sort-dir") !== "asc";

    rows.sort((rowA, rowB) => {
        let cellA = rowA.cells[columnIndex].innerText.trim();
        let cellB = rowB.cells[columnIndex].innerText.trim();

        // Check if values are numeric for proper sorting
        let isNumeric = !isNaN(parseFloat(cellA)) && !isNaN(parseFloat(cellB));
        if (isNumeric) {
            return ascending ? cellA - cellB : cellB - cellA;
        }

        return ascending ? cellA.localeCompare(cellB) : cellB.localeCompare(cellA);
    });

    // Reorder rows in the table
    rows.forEach(row => table.querySelector("tbody").appendChild(row));

    // Toggle sort direction
    thElement.setAttribute("data-sort-dir", ascending ? "asc" : "desc");

    // Update Font Awesome Icons
    let icon = thElement.querySelector("i");
    if (ascending) {
        icon.classList.remove("fa-sort", "fa-sort-down");
        icon.classList.add("fa-sort-up");
    } else {
        icon.classList.remove("fa-sort", "fa-sort-up");
        icon.classList.add("fa-sort-down");
    }

    // Reset other columns' icons
    document.querySelectorAll("th i").forEach(otherIcon => {
        if (otherIcon !== icon) {
            otherIcon.classList.remove("fa-sort-up", "fa-sort-down");
            otherIcon.classList.add("fa-sort");
        }
    });
}

function sortBySequence(thElement) {
    let table = document.getElementById("dataTable");
    let rows = Array.from(table.rows).slice(1); // Exclude header and filter row
    let ascending = thElement.getAttribute("data-sort-dir") !== "asc";

    rows.sort((rowA, rowB) => {
        let seqA = parseInt(rowA.querySelector(".sequence").innerText.trim(), 10);
        let seqB = parseInt(rowB.querySelector(".sequence").innerText.trim(), 10);

        return ascending ? seqA - seqB : seqB - seqA;
    });

    rows.forEach(row => table.querySelector("tbody").appendChild(row));

    // Toggle sort direction
    thElement.setAttribute("data-sort-dir", ascending ? "asc" : "desc");

    // Update sorting icon
    let icon = thElement.querySelector("i");
    if (ascending) {
        icon.classList.remove("fa-sort", "fa-sort-down");
        icon.classList.add("fa-sort-up");
    } else {
        icon.classList.remove("fa-sort", "fa-sort-up");
        icon.classList.add("fa-sort-down");
    }

    // Reset icons for other columns
    document.querySelectorAll("th i").forEach(otherIcon => {
        if (otherIcon !== icon) {
            otherIcon.classList.remove("fa-sort-up", "fa-sort-down");
            otherIcon.classList.add("fa-sort");
        }
    });
}
// Function to show the modal using Bootstrap
function showModal() {
    const modal = new bootstrap.Modal(document.getElementById("myModal"));
    modal.show();
}
// Function to close the modal using Bootstrap
function closeModal() {
    const modal = bootstrap.Modal.getInstance(document.getElementById("myModal"));
    if (modal) modal.hide();
}
function handleHTTPError(response) {
    let errorMessage = `Error: ${response.status} ${response.statusText}`;

    switch (response.status) {
        case 400:
            errorMessage = "Bad request. Please check the parameters.";
            break;
        case 401:
            errorMessage = "Unauthorized. Please check your authentication token.";
            break;
        case 403:
            errorMessage = "Forbidden. You do not have permission to access this resource.";
            break;
        case 404:
            errorMessage = "Data not found. Please check the event ID.";
            break;
        case 500:
            errorMessage = "Server error. Please try again later.";
            break;
    }

    console.error(errorMessage);
    showErrorMessage(errorMessage);
}

function showErrorMessage(message) {
    const tableBody = document.querySelector("#dataTable tbody");
    tableBody.innerHTML = `
        <tr>
            <td colspan="7" class="text-center">
                <div class="alert alert-danger d-flex align-items-center justify-content-center p-2" role="alert">
                    <i class="bi bi-exclamation-triangle-fill me-2"></i> <!-- Bootstrap Icon -->
                    <span>${message}</span>
                </div>
            </td>
        </tr>
        `;
    const mobileList = document.getElementById("mobileList");
    mobileList.innerHTML = ""; // Clear existing list items
    const listItem = document.createElement("div");
    listItem.classList.add("list-group-item", "list-group-item-action", "flex-column", "align-items-start");
    listItem.innerHTML = `<div class="alert alert-danger d-flex align-items-center justify-content-center p-2" role="alert">
                    <i class="bi bi-exclamation-triangle-fill me-2"></i> <!-- Bootstrap Icon -->
                    <span>${message}</span>
                </div>`;
    mobileList.appendChild(listItem);
}