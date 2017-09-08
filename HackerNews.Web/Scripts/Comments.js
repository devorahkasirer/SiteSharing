$(function () {
    $("#SubmitComment").on('click', function () {
        var commentText = $("#commentText").val();
        var id = $("#commentText").data("id");
        $.post("/home/addComment", { comment: commentText, postId: id }, function (result) {
            $("#commentsDiv").append(`<div class="well">
            <p>
                ${result.commentText}
                <br />
                posted by ${result.firstName} ${result.lastName}
            </p>
        </div>`);
            $("#commentText").val("");
        });
    });
});