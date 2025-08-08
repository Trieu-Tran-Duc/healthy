$(document).ready(function () {

    $(document).on('click', '#load-more-btn', function () {
        const $button = $(this);
        const page = ($button.data('page') || 0) + 1;

        $.ajax({
            url: '/challenge/load-more-diary',
            type: 'GET',
            data: { pageIndex: page },
            success: function (result) {
                if ($.trim(result) !== '') {
                    $('#diaries-container').append(result);
                    $button.data('page', page);
                } else {
                    $button.hide();
                }
            }
        });
    });

    $(document).on('click', '.btn-filter-chart', function () {
        $('.btn-filter-chart').removeClass('btn-active');
        $(this).addClass('btn-active');
        const type = parseInt($(this).attr('data-type')) || 0;
        RenderBodyChart(type);
    });

    //default to monthly for body chart
    RenderBodyChart(2);
});
