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

    let chart;
    function createChart(ctx, data) {
        return new Chart(ctx, {
            type: 'line',
            data: {
                labels: data.timeLine || [],
                datasets: [
                    {
                        label: '重さ',
                        data: data.weight || [],
                        borderColor: '#4dd0e1',
                        backgroundColor: 'transparent',
                        borderWidth: 2,
                        tension: 0.3
                    },
                    {
                        label: '体',
                        data: data.body || [],
                        borderColor: '#fdd835',
                        backgroundColor: 'transparent',
                        borderWidth: 2,
                        tension: 0.3
                    }
                ]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: { display: false },
                    title: { display: false }
                },
                maintainAspectRatio: false,
                scales: {
                    x: { grid: { color: '#444' } },
                    y: {
                        grid: { drawTicks: true, drawBorder: true, display: false },
                        ticks: { display: false }
                    }
                },
                layout: { padding: { left: 20, right: 20, top: 10, bottom: 10 } }
            }
        });
    }

    const loadChart = (type) => {
        $.getJSON(`/challenge/generation-chart?timeMetrics=${type}`, function (data) {
            const ctx = document.getElementById('body-weight-chart').getContext('2d');

            if (chart) {
                chart.destroy();
            }

            chart = createChart(ctx, data);
        });
    };

    $(document).on('click', '.btn-filter-chart', function () {
        $('.btn-filter-chart').removeClass('btn-active');
        $(this).addClass('btn-active');
        const type = parseInt($(this).attr('data-type')) || 0;
        loadChart(type);
    });

    loadChart(2);
});
