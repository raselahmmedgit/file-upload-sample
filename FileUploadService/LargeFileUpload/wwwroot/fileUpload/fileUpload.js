(function ($) {
    var fileUploadCount = 0;
    $.fn.fileUpload = function () {
        return this.each(function () {
            var fileUploadDiv = $(this);
            var name = fileUploadDiv.attr('id');
            var fileUploadId = `fileUpload-${name}`;

            // Creates HTML content for the file upload area.
            var fileDivContent = `
                <label for="${fileUploadId}" class="file-upload">
                    <div>
                        <i class="material-icons-outlined">cloud_upload</i>
                        <p>Drag & Drop Files Here</p>
                        <span>OR</span>
                        <div>Browse Files</div>
                    </div>
                    <input type="file" id="${fileUploadId}" name=[] multiple hidden />
                </label>
            `;

            fileUploadDiv.html(fileDivContent).addClass("file-container");

            var table = null;
            var tableBody = null;
            // Creates a table containing file information.
            function createTable() {
                table = $(`
                    <table class="table">
                        <thead>
                            <tr>
                                <th>#SN</th>
                                <th style="width: 30%;text-align:left">File Name</th>
                                <th style="text-align:center">Preview</th>
                                <th style="width: 20%;text-align:center">Size</th>
                                <th>Type</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                    <div id="divbtnsubmit"></div>
                `);

                tableBody = table.find("tbody");
                fileUploadDiv.append(table);
            }

            // Adds the information of uploaded files to table.
            function handleFiles(files) {
                if (!table) {
                    createTable();
                }

                tableBody.empty();
                if (files.length > 0) {
                    $.each(files, function (index, file) {
                        var fileName = file.name;
                       
                        var fileSize = (file.size / 1024).toFixed(2) + " KB";
                        var fileType = file.type;
                        var preview = fileType.startsWith("image")
                            ? `<img src="${URL.createObjectURL(file)}" alt="${fileName}" height="30">`
                            : `<i class="material-icons-outlined">visibility_off</i>`;

                        tableBody.append(`
                            <tr>
                                <td>${index + 1}</td>
                                <td>${fileName}</td>
                                <td style="text-align:center">${preview}</td>
                                <td style="text-align:center">${fileSize}</td>
                                <td>${fileType}</td>
                                <td><button type="button" data-indx="${index}" class="deleteBtn"><i class="material-icons-outlined">delete</i></button></td>
                            </tr>
                        `);
                    });
                    var divBtnSubmit = $('#divbtnsubmit');
                    tableBody.find(".deleteBtn").click(function () {
                        $(this).closest("tr").remove();
                        var fileIndex = $(this).data("indx");
                        const inputFile = $("#fileUpload-fileUpload");

                        $.each(files, function (index, file) {
                            var fileName = file.type;
                            $(file).val('');
                        });
                        files.splice(index, 1)
                        if (tableBody.find("tr").length === 0) {
                            tableBody.append('<tr><td colspan="6" class="no-file">No files selected!</td></tr>');
                            divBtnSubmit.html('');
                        }
                    });                    
                    divBtnSubmit.html('<input class="btn btn-primary" type="submit" value="Upload" />');                         
                }
            }

            // Events triggered after dragging files.
            fileUploadDiv.on({
                dragover: function (e) {
                    e.preventDefault();
                    fileUploadDiv.toggleClass("dragover", e.type === "dragover");
                },
                drop: function (e) {
                    e.preventDefault();
                    fileUploadDiv.removeClass("dragover");
                    handleFiles(e.originalEvent.dataTransfer.files);
                },
            });

            // Event triggered when file is selected.
            fileUploadDiv.find(`#${fileUploadId}`).change(function () {
                handleFiles(this.files);
            });
        });
    };
})(jQuery);
