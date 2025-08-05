$(document).ready(function () {

    $(document).on('click', '#load-more-btn', function () {
        var button = $(this);
        var page = parseInt(button.data('page')) + 1;

        $.ajax({
            url: '/Challenge/LoadMoreDiary',
            type: 'GET',
            data: { pageIndex: page },
            success: function (result) {
                if (result.trim() !== '') {
                    $('#diaries-container').append(result);
                    button.data('page', page);
                } else {
                    button.hide();
                }
            }
        });
    });

});