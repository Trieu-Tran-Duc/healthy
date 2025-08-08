$(document).ready(function () {

    $(document).on('click', '#load-more-btn', function () {
        var button = $(this);
        var pageIndex = parseInt(button.data('page')) + 1;
        var mealType = parseInt(button.data('type'));

        loadDataMeal(button, mealType, pageIndex)
    });

    $('.btn-filter-food').on('click',function () {
        var button = $(this);
        var buttonLoadMore = $('#load-more-btn');
        buttonLoadMore.show();

        var mealType = parseInt(button.data('type'));
        $('#meal-history-container').html('')
        
        loadDataMeal(buttonLoadMore, mealType, 1)
    })

    function loadDataMeal(buttonLoadMore, mealType, pageIndex) {
        $.ajax({
            url: '/home/load-more-meal-history',
            type: 'GET',
            data: { pageIndex: pageIndex, mealType: mealType },
            success: function (result) {
                if (result.trim() !== '') {
                    $('#meal-history-container').append(result);
                    buttonLoadMore.data('type', mealType);
                    buttonLoadMore.data('page', pageIndex);
                } else {
                    buttonLoadMore.hide();
                }
            }
        });
    }

    //default to monthly for body chart
    RenderBodyChart(2);
});