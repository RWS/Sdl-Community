function collectSegmentsDataFromHTML() {
    const segments = [];

    document.querySelectorAll('table tr[data-file-id]').forEach(row => {
        const segmentId = row.querySelector('td:first-child')?.textContent.trim();
        const fileId = row.getAttribute('data-file-id');
        const status = row.querySelector('td:nth-child(6)')?.textContent.trim(); // Adjust column index if needed
        //const comment = row.querySelector('input[name="commentInput"]')?.value.trim() || '';

        if (segmentId) {
            segments.push({
                segmentId,
                fileId,
                status
                //comment
            });
        }
    });

    window.chrome.webview.postMessage({ type: "SegmentData", data: segments });
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


function updateStatus(dropdown, segmentId, fileId, projectId) {
    const status = dropdown.value;
    const payload = {
        action: "updateStatus",
        segmentId: segmentId,
        fileId: fileId,
        projectId: projectId,
        status: status
    };
    window.chrome.webview.postMessage(payload);
}

function updateSegmentStatus(segmentId, fileId, newStatus) {

    console.info('UpdateSegmentStatus');
    const rows = document.querySelectorAll('table tr');


    rows.forEach(function(row) {
        const fileIdRow = row.getAttribute('data-file-id'); // Get the fileId attribute
        const segmentCell = row.querySelector('td:first-child'); // Get the first cell in the row


        if (segmentCell) {
            const cellContent = segmentCell.textContent.trim();
            if (cellContent === segmentId)
                console.info('SegmentId found');
            if (fileIdRow === fileId) {
                console.info('FileId found');


                // Locate the status cell
                const statusCell = row.querySelector('td:nth-child(6)');
                if (statusCell) {

                    const originalStatusElement = statusCell.querySelector('span');
                    const originalStatus = originalStatusElement ? originalStatusElement.textContent.trim() : null;

                    const isOriginal = newStatus === originalStatus;

                    console.info('newStatus ' + newStatus + '.\n' + 'original status ' + originalStatus + '.\n');
                    console.info('is original' + isOriginal + '.\n');


                    const newStatusElement = statusCell.querySelector('.new-status');

                    if (!isOriginal) {
                        if (newStatusElement) {
                            newStatusElement.innerHTML = newStatus;
                        } else {
                            const statusHtml = '          <div class="new-status">' + newStatus + '</div>';
                            statusCell.innerHTML = statusHtml + statusCell.innerHTML;
                        }
                    } else if (newStatusElement) {
                        statusCell.removeChild(newStatusElement);
                    }
                } else {
                    console.error('Status column not found in segment row: ' + segmentId);
                }
            }
        }
    });
}

function replaceCommentsForSegment(segmentId, comments, fileId) {
    const rows = document.querySelectorAll('table tr');
    let found = false;

    rows.forEach(function (row) {
        const firstCell = row.querySelector('td:first-child');
        if (firstCell) {
            const cellText = firstCell.textContent.trim();
            const rowDataFileId = row.getAttribute('data-file-id');
            console.error('FileId: ' + rowDataFileId);
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