// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", function () {
    refreshToken();


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
function formatDate(dateString) { return !dateString ? '' : new Date(dateString).toLocaleDateString('en-GB'); }
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

function sortByDate(columnIndex, thElement) {
    const table = document.getElementById("dataTable");
    const rows = Array.from(table.rows).slice(1); // Exclude header
    const ascending = thElement.getAttribute("data-sort-dir") !== "asc";

    rows.sort((rowA, rowB) => {
        const cellA = rowA.cells[columnIndex].innerText.trim();
        const cellB = rowB.cells[columnIndex].innerText.trim();

        const dateA = parseDate(cellA);
        const dateB = parseDate(cellB);

        return ascending ? dateA - dateB : dateB - dateA;
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

// Helper to parse "dd-mm-yy" to a Date object
function parseDate(dateStr) {
    const [day, month, year] = dateStr.split("-");
    return new Date(`${year}`, month - 1, day);
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
// Function to show the modal using Bootstrap
function showDeleteModal(title, message, ID) {
    document.getElementById("myModalLabel").innerText = title;
    document.getElementById("lblDeleteMessage").value = message;
    document.getElementById("ID").value = ID;

    const modal = new bootstrap.Modal(document.getElementById("deleteModal"));
    modal.show();
}
// Function to close the delete modal using Bootstrap
function closeDeleteModal() {
    const modal = bootstrap.Modal.getInstance(document.getElementById("deleteModal"));
    if (modal) modal.hide();
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
    var columnCount = $("#dataTable tr:first td").length || $("#dataTable tr:first th").length;

    tableBody.innerHTML = `
        <tr>
            <td colspan="${columnCount}" class="text-center">
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

function showWarningMessage(message) {
    const tableBody = document.querySelector("#dataTable tbody");
    var columnCount = $("#dataTable tr:first td").length || $("#dataTable tr:first th").length;
    tableBody.innerHTML = `
        <tr>
            <td colspan="${columnCount}" class="text-center">
                <div class="alert alert-success d-flex align-items-center justify-content-center p-2" role="alert">
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
    listItem.innerHTML = `<div class="alert alert-success d-flex align-items-center justify-content-center p-2" role="alert">
                    <i class="bi bi-exclamation-triangle-fill me-2"></i> <!-- Bootstrap Icon -->
                    <span>${message}</span>
                </div>`;
    mobileList.appendChild(listItem);
}
// In site.js
function showLoading() {
    const loader = document.getElementById("full-page-loader");
    if (loader) {
        loader.classList.remove("d-none");
    }
}

function hideLoading() {
    const loader = document.getElementById("full-page-loader");
    if (loader) {
        loader.classList.add("d-none");
    }
}
function showSuccessToast(message) {
    const toastElement = document.getElementById('successToast');
    const lblSuccessMessage = document.getElementById('lblSuccessMessage');
    lblSuccessMessage.textContent = message;
    const toast = new bootstrap.Toast(toastElement, { delay: 3000 }); // 3 seconds
    toast.show();
}
function showFailedToast(message) {
    const toastElement = document.getElementById('failedToast');
    const lblFailedMessage = document.getElementById('lblFailedMessage');
    lblFailedMessage.textContent = message;
    const toast = new bootstrap.Toast(toastElement, { delay: 3000 }); // 3 seconds
    toast.show();
}

async function loadClasses(apiBaseUrl) {
    try {
        const classesDropdown = document.getElementById("drpClasses");
        const request = `${apiBaseUrl}/Meeting/GetServantClasses`;
        const classResponse = await fetch(request, {
            method: "GET",
            credentials: "include",
            headers: { "Content-Type": "application/json" }
        });

        if (!classResponse.ok) throw new Error("Failed to fetch classes.");
        const classList = await classResponse.json();

        // Populate the dropdown
        classesDropdown.innerHTML = "";
        classList.forEach(cls => {
            const option = document.createElement("option");
            option.value = cls.classID;
            option.textContent = cls.className;
            classesDropdown.appendChild(option);
        });

        $('#drpClasses').select2({
            placeholder: "Select classes",
            allowClear: true,
            width: '100%' // Ensures proper styling with Bootstrap
        });

    } catch (err) {
        console.error("Error loading classes:", err);
    }

}
function dialMobile() {
    const mobile = document.getElementById('Mobile').value;
    if (mobile) {
        window.location.href = 'tel:' + mobile;
    } else {
        alert('No mobile number available.');
    }
}
function refreshToken() {
    const keepAliveInterval = 10 * 60 * 1000;

    const keepSessionAlive = () => {
        fetch('/RefreshToken', { // The path to your new Razor Page
            method: 'GET',
            // This is crucial! It ensures cookies are sent with the request.
            credentials: 'include'
        }).then(response => {
            if (response.ok) {
                console.log('Session keep-alive signal sent successfully.');
            }
        }).catch(error => {
            console.error('Error sending session keep-alive signal:', error);
        });
    };

    // Start the timer
    setInterval(keepSessionAlive, keepAliveInterval);
}

/**
* --- Bulk Action Functions ---
*/
async function bulkPrintCards(memberIds, cardType) {
    if (!memberIds || memberIds.length === 0) {
        alert("No members selected to print.");
        return;
    }
    console.log("Requesting cards for member IDs:", memberIds);
    showLoading();
    try {
        const response = await fetch(`${apiBaseUrl}/MemberImages/generate-cards`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: 'include',
            body: JSON.stringify({
                memberIDs: memberIds,
                cardType: cardType
            })
        });
        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(`Failed to generate cards. Server responded with: ${errorText || response.statusText}`);
        }
        const blob = await response.blob();
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.style.display = 'none';
        a.href = url;
        a.download = 'MemberCards.zip';
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
        document.body.removeChild(a);
    } catch (error) {
        console.error("Error printing member cards:", error);
        alert(`An error occurred: ${error.message}`);
    } finally {
        hideLoading();
    }
}