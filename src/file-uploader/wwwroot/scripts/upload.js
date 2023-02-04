// src: http://www.binaryintellect.net/articles/474dc031-087f-4b81-b994-9ca2adb949d6.aspx
$(document).ready
(
    function ()
    {
        $("#progress").hide();
        // in all the event handlers we call preventDefault() and stopPropagation() to prevent the default action and to stop the bubbling-up of the event.
        $("#fileBasket").on("dragenter", function (evt)
        {
            evt.preventDefault();
            evt.stopPropagation();
        });

        $("#fileBasket").on("dragover", function (evt)
        {
            evt.preventDefault();
            evt.stopPropagation();
        });

        $("#fileBasket").on("drop", function (evt)
        {
            evt.preventDefault();
            evt.stopPropagation();
            // grab the files w/ this event
            var files = evt.originalEvent.dataTransfer.files;
            var fileNames = "";
            if (files.length > 0)
            {
                fileNames += "Uploading <br/>"
                for (var i = 0; i < files.length; i++)
                {
                    fileNames += files[i].name + "<br />";
                }
            }
            // write filled array into basket
            $("#fileBasket").html(fileNames)
            var data = new FormData(); // form values (key value pairs) to be submitted to the server
            for (var i = 0; i < files.length; i++)
            {
                data.append(files[i].name, files[i]);
            }
            // AJAX call of the kvp
            $.ajax(
            {
                type: "POST",
                url: "/home/UploadFiles",
                contentType: false,
                processData: false,
                data: data,
                success: function (message)
                {
                    $("#fileBasket").html(message);
                },
                error: function ()
                {
                    $("#fileBasket").html("There was error uploading files!");
                },
                beforeSend: function ()
                {
                    $("#progress").show();
                },
                complete: function ()
                {
                    $("#progress").hide();
                }
            });
        });
    }
);
