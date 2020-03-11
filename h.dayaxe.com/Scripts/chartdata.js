$(function () {
    if ($('#container').length > 0) {
        $('#container').highcharts({
            chart: {
                type: 'column'
            },
            title: {
                text: 'New vs Returning',
                style: { "color": "#666666", "fontSize": "36px" }
            },
            xAxis: {
                categories: window.newAndReturningDataCategory,
                crosshair: true
            },
            yAxis: {
                min: 0,
                title: {
                    text: ''
                }
            },
            tooltip: {
                headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
                pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
                    '<td style="padding:0"><b>{point.y:.0f}</b></td></tr>',
                footerFormat: '</table>',
                shared: true,
                useHTML: true
            },
            plotOptions: {
                column: {
                    pointPadding: 0.1,
                    borderWidth: 0
                }
            },
            series: window.newAndReturningData
        });
    }
    if ($('#redemptionFrequency').length > 0) {
        $('#redemptionFrequency').highcharts({
            chart: {
                type: 'column'
            },
            title: {
                text: 'Redemption Frequency',
                style: { "color": "#666666", "fontSize": "36px" }
            },
            xAxis: {
                categories: window.frequencyDataCategory,
                crosshair: true
            },
            yAxis: {
                min: 0,
                title: {
                    text: ''
                }
            },
            tooltip: {
                headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
                pointFormat: '<tr><td style="color:{series.color};padding:0"></td>' +
                    '<td style="padding:0"><b>{point.y:.0f}%</b></td></tr>',
                footerFormat: '</table>',
                shared: true,
                useHTML: true
            },
            colors: [
                '#000000'
            ],
            plotOptions: {
                column: {
                    pointPadding: 0.1,
                    borderWidth: 0,
                    colorByPoint: true
                },
                series: {
                    borderWidth: 0,
                    dataLabels: {
                        enabled: true,
                        format: '{point.y:.0f}%'
                    }
                }
            },
            series: [{
                name: 'Frequency',
                showInLegend: false,
                data: window.frequencyData
            }]
        });
    }
    if ($('#mostRedeemDay').length > 0) {
        $('#mostRedeemDay').highcharts({
            chart: {
                type: 'column'
            },
            title: {
                text: 'Most Redeemed Days',
                style: { "color": "#666666", "fontSize": "36px" }
            },
            xAxis: {
                categories: [
                    'Mon',
                    'Tue',
                    'Web',
                    'Thu',
                    'Fri',
                    'Sat',
                    'Sun'
                ],
                crosshair: true
            },
            yAxis: {
                min: 0,
                title: {
                    text: ''
                }
            },
            tooltip: {
                headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
                pointFormat: '<tr><td style="color:{series.color};padding:0"></td>' +
                    '<td style="padding:0"><b>{point.y:.0f}%</b></td></tr>',
                footerFormat: '</table>',
                shared: true,
                useHTML: true
            },
            colors: [
                '#000000'
            ],
            plotOptions: {
                column: {
                    pointPadding: 0.1,
                    borderWidth: 0,
                    colorByPoint: true
                },
                series: {
                    borderWidth: 0,
                    dataLabels: {
                        enabled: true,
                        format: '{point.y:.0f}%'
                    }
                }
            },
            series: window.redeeemDayData
        });
    }
    if ($('#otherHotelvisited').length > 0) {
        $('#otherHotelvisited').highcharts({
            chart: {
                type: 'bar'
            },
            title: {
                text: 'Other Hotels Visited',
                style: { "color": "#666666", "fontSize": "36px" }
            },
            subtitle: {
                text: ''
            },
            xAxis: {
                categories: window.hotelVisitedCategory,
                title: {
                    text: null
                }
            },
            yAxis: {
                min: 0,
                title: {
                    text: null,
                    align: 'high'
                },
                labels: {
                    overflow: 'justify'
                }
            },
            tooltip: {
                valueSuffix: ' %'
            },
            colors: [
                '#000000'
            ],
            plotOptions: {
                bar: {
                    dataLabels: {
                        enabled: true,
                        format: '{point.y:.0f}%'
                    }
                }
            },
            legend: {
                layout: 'vertical',
                align: 'right',
                verticalAlign: 'top',
                x: -40,
                y: 80,
                floating: true,
                borderWidth: 1,
                backgroundColor: ((Highcharts.theme && Highcharts.theme.legendBackgroundColor) || '#FFFFFF'),
                shadow: true
            },
            credits: {
                enabled: false
            },
            series: window.hotelVisitedData
        });
    }
    if ($('#customerSatisfaction').length > 0) {
        function getAvgCustomerSatisfaction() {
            var total = 0;
            var totalRate = 0;
            var rate = 5;
            for (var i = 0; i < window.customerSatisfactionData[0].data.length; i++) {
                total += window.customerSatisfactionData[0].data[i] * rate;
                totalRate += window.customerSatisfactionData[0].data[i];
                rate--;
            }

            return (total / totalRate).toFixed(1);
        }
        $('#customerSatisfaction').highcharts({
            chart: {
                type: 'bar'
            },
            title: {
                text: 'Customer Satisfaction',
                style: { "color": "#666666", "fontSize": "36px" }
            },
            subtitle: {
                text: 'AVG. ' + getAvgCustomerSatisfaction()
            },
            xAxis: {
                categories: ['5 star', '4 star', '3 star', '2 star', '1 star'],
                title: {
                    text: null
                }
            },
            yAxis: {
                min: 0,
                title: {
                    text: null,
                    align: 'high'
                },
                labels: {
                    overflow: 'justify'
                }
            },
            tooltip: {
                valueSuffix: ' millions'
            },
            colors: [
                '#000000'
            ],
            plotOptions: {
                bar: {
                    dataLabels: {
                        enabled: true,
                        format: '{point.y:.0f}%'
                    }
                }
            },
            legend: {
                layout: 'vertical',
                align: 'right',
                verticalAlign: 'top',
                x: -40,
                y: 80,
                floating: true,
                borderWidth: 1,
                backgroundColor: ((Highcharts.theme && Highcharts.theme.legendBackgroundColor) || '#FFFFFF'),
                shadow: true
            },
            credits: {
                enabled: false
            },
            series: window.customerSatisfactionData
        });
    }

    if ($('#daysRedeemed').length > 0) {
        $('#daysRedeemed').highcharts({
            chart: {
                type: 'column'
            },
            title: {
                text: 'Redeemed Days From Purchase',
                style: { "color": "#666666", "fontSize": "36px" }
            },
            xAxis: {
                categories: window.daysRedeemedCategories,
                crosshair: true
            },
            yAxis: {
                min: 0,
                title: {
                    text: ''
                }
            },
            tooltip: {
                headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
                pointFormat: '<tr><td style="color:{series.color};padding:0"></td>' +
                    '<td style="padding:0"><b>{point.y:.0f}%</b></td></tr>',
                footerFormat: '</table>',
                shared: true,
                useHTML: true
            },
            colors: [
                '#000000'
            ],
            plotOptions: {
                column: {
                    pointPadding: 0.1,
                    borderWidth: 0,
                    colorByPoint: true
                },
                series: {
                    borderWidth: 0,
                    dataLabels: {
                        enabled: true,
                        format: '{point.y:.0f}%'
                    }
                }
            },
            series: window.daysRedeemedData
        });
    }

    if ($('#mostRedeemedHour').length > 0) {
        $('#mostRedeemedHour').highcharts({
            chart: {
                type: 'column'
            },
            title: {
                text: 'Most Redeemed Hours',
                style: { "color": "#666666", "fontSize": "36px" }
            },
            xAxis: {
                categories: window.mostRedeemedHourCategories,
                crosshair: true
            },
            yAxis: {
                min: 0,
                title: {
                    text: ''
                }
            },
            tooltip: {
                headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
                pointFormat: '<tr><td style="color:{series.color};padding:0"></td>' +
                    '<td style="padding:0"><b>{point.y:.0f}%</b></td></tr>',
                footerFormat: '</table>',
                shared: true,
                useHTML: true
            },
            colors: [
                '#000000'
            ],
            plotOptions: {
                column: {
                    pointPadding: 0.1,
                    borderWidth: 0,
                    colorByPoint: true
                },
                series: {
                    borderWidth: 0,
                    dataLabels: {
                        enabled: true,
                        format: '{point.y:.0f}%'
                    }
                }
            },
            series: window.mostRedeemedHourData
        });
    }
});