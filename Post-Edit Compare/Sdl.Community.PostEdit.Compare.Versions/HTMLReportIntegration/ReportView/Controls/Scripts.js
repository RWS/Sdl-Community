﻿let currentSegmentData = {};

let currentDropdown = null;

document.addEventListener('contextmenu', function (e) {
   
    // Only trigger if right-clicking on an element with class 'status-cell' (or inside one)
    let cell = e.target.closest('.status-cell');

    if (currentDropdown && currentDropdown !== cell) {
        closeDropdown();
    }

    if (cell) {
        e.preventDefault();

        // Get the parent row (assumed to have segment data attributes)
        let row = cell.closest('tr');
        if (row) {
            currentSegmentData = {
                segmentId: row.getAttribute('data-segment-id'),
                fileId: row.getAttribute('data-file-id'),
                projectId: row.getAttribute('data-project-id')
            };
        }

        // Find the dropdown within this status cell
        const dropdown = cell.querySelector('.custom-dropdown');
        if (dropdown) {
            // First, hide any open dropdown in this cell
            dropdown.style.display = 'block'; // Temporarily show to calculate if needed
            // For a cell-local menu, we'll simply position it relative to the cell:
            // For example, place it at the bottom-right of the cell:
            dropdown.style.top = cell.offsetHeight + 'px';
            dropdown.style.right = '0px';

            // Show the dropdown
            dropdown.style.opacity = '1';
            dropdown.style.visibility = 'visible';

            currentDropdown = dropdown;
        }
    }
});

function closeDropdown() {
    if (currentDropdown) {
        currentDropdown.style.display = 'none';
        currentDropdown.style.opacity = 0;
        currentDropdown.style.visibility = 'hidden';
    }
    currentDropdown = null;  // Reset the current dropdown
}

// Hide the dropdown when clicking outside of any status cell
document.addEventListener('click', function (e) {
    // Look for all dropdowns and hide those that are open
    document.querySelectorAll('.custom-dropdown').forEach(function (dropdown) {
        if (!dropdown.contains(e.target)) {
            dropdown.style.opacity = '0';
            dropdown.style.visibility = 'hidden';
            // Use a timeout to hide display after transition if desired:
            setTimeout(() => { dropdown.style.display = 'none'; }, 200);
        }
    });
});

// Called when an option in the dropdown is clicked
function updateStatus(option) {
    const status = option.getAttribute('data-value');

    const payload = {
        action: 'updateStatus',
        segmentId: currentSegmentData.segmentId,
        fileId: currentSegmentData.fileId,
        projectId: currentSegmentData.projectId,
        status: status
    };

    console.log('Payload:', payload);
    window.chrome.webview.postMessage(payload);

    // Hide the dropdown in the current status cell
    const dropdown = option.closest('.custom-dropdown');
    if (dropdown) {
        dropdown.style.opacity = '0';
        dropdown.style.visibility = 'hidden';
        setTimeout(() => { dropdown.style.display = 'none'; }, 200);
    }
}








function getComments() {
    const segments = [];

    const rows = document.querySelectorAll('table tr[data-file-id]');

    rows.forEach(row => {
        const segmentId = row.querySelector('td:first-child')?.textContent.trim();
        const fileId = row.getAttribute('data-file-id');
        const projectId = row.getAttribute('data-project-id');

        const comments = [];

        const commentCell = row.querySelector('td:nth-last-child(1)');
        if (commentCell) {
            const commentDivs = commentCell.querySelectorAll('div.comments > div');
            commentDivs.forEach(div => {
                const infoDiv = div.querySelector('div');
                const spans = infoDiv?.querySelectorAll('span') || [];

                const severity = spans[0]?.textContent.trim() || '';
                const date = spans[1]?.textContent.trim() || '';
                const author = spans[2]?.textContent.trim() || '';
                const text = div.querySelector('p')?.textContent.trim() || '';

                if (severity || date || author || text) {
                    comments.push({
                        severity,
                        date,
                        author,
                        text
                    });
                }
            });
        }

        if (segmentId) {
            segments.push({
                segmentId,
                fileId,
                projectId,
                comments
            });
        }
    });

    return segments;
}




function getProjectId() {
    const row = document.querySelector('table tr[data-project-id]');

    if (row) {
        const projectId = row.getAttribute('data-project-id');
        console.info('ProjectId: ' + projectId + '\n');
        return projectId;
    }

    return null;
}

function showSegments(segmentList) {
    segmentList.forEach(seg => {
        console.info('Segment: ' + seg.segmentId + ', ' + seg.fileId + '\n');
    });
    document.querySelectorAll('table tr[data-file-id]').forEach(row => {
        const segmentCell = row.querySelector('td:first-child'); // Assuming segment ID is in the first column
        const fileId = row.getAttribute('data-file-id');

        console.info('Current file ID: ' + fileId + '\n');

        if (segmentCell) {
            const segmentId = segmentCell.textContent.trim();
            const match = segmentList.some(item => item.segmentId === segmentId && item.fileId === fileId);

            row.style.display = match ? '' : 'none';
        }
    });
}

function showAllSegments() {
    document.querySelectorAll('table tr[data-file-id]').forEach(row => {
        row.style.display = '';
    });
}

function getCellFromRow(row, cellIndex) {
    const cell = row.cells[cellIndex];
    if (cell) {
        return cell;
    } else {
        console.error('Cell not found at index: ' + cellIndex);
        return null;
    }
}

//method for extracting a column index from a table
function getColumnIndexFromTable(columnName) {
    const table = document.querySelector('table tr[data-file-id]').closest('table');
    const headers = table.querySelectorAll('th');

    let columnIndex = -1;

    headers.forEach((header, index) => {
        if (header.textContent.trim() === columnName) {
            columnIndex = index;
        }
    });

    if (columnIndex !== -1) {
        console.info('Status column found at index: ' + columnIndex);
    } else {
        console.error('Status column not found');
    }

    return columnIndex;
}

function collectSegmentsDataFromHTML() {
    const segments = [];

    const statusColumnIndex = getColumnIndexFromTable('Status');
    const matchTypeColumnIndex = getColumnIndexFromTable('Match');

    document.querySelectorAll('table tr[data-file-id]').forEach(row => {
        const segmentId = row.querySelector('td:first-child')?.textContent.trim();
        const fileId = row.getAttribute('data-file-id');
        const statusColumn = getCellFromRow(row, statusColumnIndex);
        const matchType = getCellFromRow(row, matchTypeColumnIndex)?.textContent.trim();

        const statuses = statusColumn.querySelectorAll('span');
        const status = statuses[statuses.length - 1]?.textContent.trim();

        if (segmentId) {
            segments.push({
                segmentId,
                fileId,
                status,
                matchType
            });
        }
    });

    return segments;
}

function getAllSegmentsCurrentlyVisible() {
    const segments = [];

    const visibleSegments = Array.from(document.querySelectorAll('table tr[data-file-id]'))
        .filter(row => row.style.display !== 'none');

    visibleSegments.forEach(row => {

        const segmentId = row.querySelector('td:first-child')?.textContent.trim();
        const fileId = row.getAttribute('data-file-id');
        const projectId = row.getAttribute('data-project-id');

        if (segmentId) {
            segments.push({
                segmentId,
                fileId,
                projectId
            });
        }
    });

    return segments;
}

function getCleanedHTMLForExport() {
    console.log("Cleaning exportable HTML...");

    // Clone the document to avoid modifying the live page
    let clonedDocument = document.documentElement.cloneNode(true);

    // Remove dropdowns for selecting status
    clonedDocument.querySelectorAll('.status-dropdown').forEach(el => el.remove());

    // Remove comment input fields
    clonedDocument.querySelectorAll('input[name="commentInput"]').forEach(el => el.remove());

    // Remove severity dropdowns
    clonedDocument.querySelectorAll('.severity-dropdown').forEach(el => el.remove());

    console.log("HTML cleaned. Returning exported HTML.");
    return clonedDocument.outerHTML;
}

function navigateToSegment(segmentId, fileId, projectId) {
    const payload = {
        action: "navigate",
        segmentId: segmentId,
        fileId: fileId,
        projectId: projectId
    };
    window.chrome.webview.postMessage(payload);
}

function submitComment(input, severity, segmentId, fileId, projectId) {
    const commentText = input.value.trim();
    if (!commentText) return;

    const payload = {
        action: "addComment",
        severity: severity,
        segmentId: segmentId,
        fileId: fileId,
        projectId: projectId,
        comment: commentText
    };

    window.chrome.webview.postMessage(payload);

    // Reset input field
    input.value = "";
    input.placeholder = "Add comment";
}

//function updateStatus(option, segmentId, fileId, projectId) {
//    const status = option.getAttribute("data-value");
//    const payload = {
//        action: "updateStatus",
//        segmentId: segmentId,
//        fileId: fileId,
//        projectId: projectId,
//        status: status
//    };
//    window.chrome.webview.postMessage(payload);
//}


function updateSegmentStatus(segmentId, fileId, newStatus) {
    console.info('UpdateSegmentStatus');
    const rows = document.querySelectorAll('table tr');

    rows.forEach(function (row) {
        const fileIdRow = row.getAttribute('data-file-id'); // Get the fileId attribute
        const segmentCell = row.querySelector('td:first-child'); // Get the first cell in the row

        if (segmentCell) {
            const cellContent = segmentCell.textContent.trim();
            if (cellContent === segmentId) {
                console.info('SegmentId found');
                if (fileIdRow === fileId) {
                    console.info('FileId found');

                    // Locate the status cell
                    const statusColumnIndex = getColumnIndexFromTable('Status');
                    const statusCell = getCellFromRow(row, statusColumnIndex);
                    if (statusCell) {
                        const spans = statusCell.querySelectorAll('span');
                        const lastSpan = spans.length ? spans[spans.length - 1] : null;

                        if (lastSpan) {
                            if (spans.length === 1) {
                                if (lastSpan.innerText === newStatus) return;

                                statusCell.appendChild(document.createElement('br'));

                                const newSpan = document.createElement('span');
                                newSpan.innerText = newStatus;
                                newSpan.classList.add('textNew');

                                lastSpan.style.cssText = '';
                                lastSpan.classList.add('textRemoved');

                                statusCell.appendChild(newSpan);
                            } else {
                                if (spans[0].innerText === newStatus) {
                                    const previousSibling = lastSpan.previousSibling;
                                    if (previousSibling && previousSibling.nodeName === "BR") {
                                        previousSibling.remove();
                                    }
                                    spans[0].classList.remove('textRemoved');
                                    lastSpan.remove();
                                    return;
                                }
                                lastSpan.innerText = newStatus;
                            }
                        }

                    } else {
                        console.error('Status column not found in segment row: ' + segmentId);
                    }
                }
            }
        }
    });
}



function addCommentsForSegment(segmentId, comments, fileId) {
    const rows = document.querySelectorAll('table tr');
    let found = false;

    rows.forEach(function (row) {
        const firstCell = row.querySelector('td:first-child');
        if (firstCell) {
            const cellText = firstCell.textContent.trim();
            const rowDataFileId = row.getAttribute('data-file-id');

            if (cellText === segmentId)
                if (rowDataFileId === fileId) {
                    found = true;
                    const commentsCell = row.querySelector('td:last-child');
                    const commentsDiv = commentsCell.querySelector('.comments');
                    if (commentsDiv) {
                        // Append new comments without replacing existing ones
                        comments.forEach(function (comment) {
                            let severityHtml;
                            if (comment.severity === 'High') {
                                severityHtml = '<span style="padding: 3px; color: red; font-weight: bold;">' + comment.severity + '</span>';
                            } else {
                                severityHtml = '<span style="padding: 3px; font-weight: bold;">' + comment.severity + '</span>';
                            }

                            const commentHtml = `
          <div style="border-style: solid solid dashed solid; border-width: thin; border-color: #C0C0C0 #C0C0C0 #000000 #C0C0C0; margin: 1px 0px 0px 1px; padding: 0; text-align: left;">
            <div style="white-space: nowrap; background-color: #DFDFFF; text-align: left; color: Black; margin-bottom: 1px;">
              ${severityHtml}
              <span style="padding: 3px; font-style: italic;">${comment.date}</span>
              <br/>
              <span style="padding: 3px;">${comment.author}</span>
            </div>
            <p style="margin: 0px; padding: 3;">${comment.text}</p>
          </div>
          `;
                            commentsDiv.innerHTML += commentHtml;
                        });
                    } else {
                        console.error('Last column not found in segment row: ' + segmentId);
                    }
                }
        }
    });

    if (!found) {
        console.error('Segment not found: ' + segmentId);
    }
}

function replaceCommentsForSegment(segmentId, comments, fileId) {
    const rows = document.querySelectorAll('table tr');
    let found = false;

    rows.forEach(function (row) {
        const firstCell = row.querySelector('td:first-child');
        if (firstCell) {
            const cellText = firstCell.textContent.trim();
            const rowDataFileId = row.getAttribute('data-file-id');

            if (cellText === segmentId)
                if (rowDataFileId === fileId) {
                    found = true;
                    const commentsCell = row.querySelector('td:last-child');
                    const commentsDiv = commentsCell.querySelector('.comments');
                    if (commentsDiv) {
                        commentsDiv.innerHTML = '';

                        comments.forEach(function (comment) {
                            let severityHtml;
                            if (comment.severity === 'High') {
                                severityHtml = '<span style="padding: 3px; color: red; font-weight: bold;">' + comment.severity + '</span>';
                            } else {
                                severityHtml = '<span style="padding: 3px; font-weight: bold;">' + comment.severity + '</span>';
                            }

                            const commentHtml = `
          <div style="border-style: solid solid dashed solid; border-width: thin; border-color: #C0C0C0 #C0C0C0 #000000 #C0C0C0; margin: 1px 0px 0px 1px; padding: 0; text-align: left;">
            <div style="white-space: nowrap; background-color: #DFDFFF; text-align: left; color: Black; margin-bottom: 1px;">
              ${severityHtml}
              <span style="padding: 3px; font-style: italic;">${comment.date}</span>
              <br/>
              <span style="padding: 3px;">${comment.author}</span>
            </div>
            <p style="margin: 0px; padding: 3;">${comment.text}</p>
          </div>
          `;
                            commentsDiv.innerHTML += commentHtml;
                        });
                    } else {
                        console.error('Last column not found in segment row: ' + segmentId);
                    }
                }
        }
    });

    if (!found) {
        console.error('Segment not found: ' + segmentId);
    }
}