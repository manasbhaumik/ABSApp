$(function () {

      //create a class
    $.APSUserMessage = function (urls) {
        this.url = urls;
    }

    //extend the class network
    $.APSUserMessage.prototype = {
        Init: function () {
            var $that = this;
            debugger
            $(document).ready(function () {
   
                $("#btnMessages").on("click", function () {
                    window.location = $that.url.APSMessagePartialView
                });
                $("#btnDeleted").on("click", function () {
                    window.location = $that.url.APSDeletedPartialView
                });
            });

        },
    }

}(jQuery))