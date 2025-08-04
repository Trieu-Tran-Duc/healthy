$(document).ready(function () {
    $.getJSON(window.chartDataUrl, function (data) {
        new Chart(document.getElementById('body-weight-chart'), {
            type: 'line',
            data: {
                labels: data.labels,
                datasets: [
                    {
                        label: 'Line 1',
                        data: data.line1,
                        borderColor: '#4dd0e1',
                        backgroundColor: 'transparent',
                        borderWidth: 2,
                        tension: 0.3
                    },
                    {
                        label: 'Line 2',
                        data: data.line2,
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
                layout: {
                    padding: {
                        left: 20,
                        right: 20,
                        top: 10,
                        bottom: 10
                    }
                }
            }
        });
    });
});
