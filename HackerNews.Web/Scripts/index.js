$(function () {
    $(".voteUp").on('click', function () {
        $.post("/home/upVote", { postId: $(this).data("id") }, function (result) {
            $(`#${result.postId}`).text(`UpVotes: ${result.votesUp} | DownVotes: ${result.votesDown}`);
            $(`#up-${result.postId}`).prop("disabled", true);
            $(`#down-${result.postId}`).prop("disabled", true);
        });
    });
    $(".voteDown").on('click', function () {
        $.post("/home/downVote", { postId: $(this).data("id") }, function (result) {
            $(`#${result.postId}`).text(`UpVotes: ${result.votesUp} | DownVotes: ${result.votesDown}`);
            $(`#down-${result.postId}`).prop("disabled", true);
            $(`#up-${result.postId}`).prop("disabled", true);
        });
    });
});