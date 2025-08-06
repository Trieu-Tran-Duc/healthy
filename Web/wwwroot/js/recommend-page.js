$(document).ready(function () {

    $(document).on('click', '#load-more-btn', function () {
        var button = $(this);
        var page = parseInt(button.data('page')) + 1;

        $.ajax({
            url: '/recommend/load-more-recommend',
            type: 'GET',
            data: { pageIndex: page },
            success: function (result) {
                if (result.trim() !== '') {
                    $('#recommend-container').append(result);
                    button.data('page', page);
                } else {
                    button.hide();
                }
            }
        });
    });

});