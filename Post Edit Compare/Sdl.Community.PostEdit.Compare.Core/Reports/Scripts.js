function updateSegmentStatus(segmentId, fileId, newStatus) {
    // Find all rows in the table
    const rows = document.querySelectorAll('table tr');


    rows.forEach(row => {
        const fileIdRow = row.getAttribute('data-file-id'); // Get the fileId attribute
        const segmentCell = row.querySelector('td:first-child'); // Get the first cell in the row

        // Check if the row matches the segmentId
        if (segmentCell) {
            const cellContent = segmentCell.textContent.trim();
            if (cellContent === segmentId)
                if (fileIdRow === fileId) {


                        
                    // Locate the status cell
                    const statusCell = row.querySelector('td:nth-child(6)');
                    if (statusCell) {
                        const statusHtml = `
                    <div style="border: 1px solid black; background-color: yellow; padding: 3px; margin-bottom: 2px;">
                        <strong>}</strong>
                    </div>
                `;
                        statusCell.innerHTML = statusHtml + statusCell.innerHTML;
                    } else {
                        console.error('Status column not found in segment row: ' + segmentId);
                    }

                }
        }
    });


}