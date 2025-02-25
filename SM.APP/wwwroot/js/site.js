// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", function () {
    // Desktop Table Filtering
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

    // Mobile List Filtering
    document.querySelector(".mobile-filter").addEventListener("keyup", function () {
        let searchTerm = this.value.toLowerCase();
        let items = document.querySelectorAll("#mobileList .list-group-item");

        items.forEach(item => {
            let text = item.innerText.toLowerCase();
            item.style.display = text.includes(searchTerm) ? "" : "none";
        });
    });
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