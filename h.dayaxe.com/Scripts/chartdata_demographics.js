function getRateForMale() {
    var totalMale = 0;
    var totalFemale = 0;
    for (var i = 0; i < window.demographicsMale.length; i++) {
        totalMale += window.demographicsMale[i];
        totalFemale += window.demographicsFemale[i];
    }
    return Math.round(Math.abs(totalMale) * 100 / (Math.abs(totalMale) + Math.abs(totalFemale)));
}

$(function () {
    if ($('#ageAndGender').length > 0) {
        // Age categories
        //var categories = ['65+', '55-64', '45-54', '35-44', '25-34', '18-24', '13-17'];
        $('#ageAndGender').highcharts({
            chart: {
                type: 'bar'
            },
            title: {
                text: 'Age & Gender',
                style: { "color": "#666666", "fontSize": "36px" }
            },
            subtitle: {
                text: ''
            },
            xAxis: [{
                categories: window.ageAndGenderCategories,
                reversed: false,
                labels: {
                    step: 1
                }
            }, { // mirror axis on right side
                opposite: true,
                reversed: false,
                categories: window.ageAndGenderCategories,
                linkedTo: 0,
                labels: {
                    step: 1
                }
            }],
            yAxis: {
                title: {
                    text: null
                },
                labels: {
                    formatter: function () {
                        return Math.abs(this.value) + '%';
                    }
                }
            },
            colors:[
                '#000000',
                '#14c2f4'
            ],
            plotOptions: {
                series: {
                    stacking: 'normal'
                }
            },

            tooltip: {
                formatter: function () {
                    return '<b>' + this.series.name + ', age ' + this.point.category + '</b><br/>' +
                        'Total: ' + Highcharts.numberFormat(Math.abs(this.point.y), 0);
                }
            },

            series: [{
                name: 'Female',
                data: window.demographicsFemale
            },{
                name: 'Male',
                data: window.demographicsMale
            }]
        });
        var malePercent = getRateForMale();
        var female = 100 - malePercent;
        $('#malePercent').html(malePercent + '%');
        $('#femalePercent').html(female + '%');
    }
    if ($('#proximity').length > 0) {
        $('#proximity').highcharts({
            chart: {
                type: 'column'
            },
            title: {
                text: 'Proximity (miles)',
                style: { "color": "#666666", "fontSize": "36px" }
            },
            xAxis: {
                categories: ['1', '5', '10', '15', '30', '40+'],
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
                    '<td style="padding:0"><b>{point.y:.1f} mm</b></td></tr>',
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
                }
            },
            series: [{
                name: 'miles',
                showInLegend: false,
                data: window.proximityData
            }]
        });
    }
    if ($('#customerType').length > 0) {
        $('#customerType').highcharts({
            chart: {
                type: 'bar'
            },
            title: {
                text: 'Customer Type',
                style: { "color": "#666666", "fontSize": "36px" }
            },
            subtitle: {
                text: ''
            },
            xAxis: {
                categories: ['Savvy Singles',
                    'Working Professionals',
                    'Urban Professionals',
                    'Creatives',
                    'Working Parents',
                    'Twenty Somethings'],
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
                        enabled: true
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
            series: [{
                name: 'Visited',
                showInLegend: false,
                data: window.customerTypeData
            }]
        });
    }
    if ($('#Income').length > 0) {
        $('#Income').highcharts({
            chart: {
                type: 'column'
            },
            title: {
                text: 'Income',
                style: { "color": "#666666", "fontSize": "36px" }
            },
            xAxis: {
                categories: ['50K<', '50-75K', '75-100K', '100-150K', '150-250K', '250+'],
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
                    '<td style="padding:0"><b>{point.y:.1f} mm</b></td></tr>',
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
                }
            },
            series: [{
                name: 'income',
                showInLegend: false,
                data: window.incomeData
            }]
        });
    }
    if ($('#education').length > 0) {
        $('#education').highcharts({
            chart: {
                type: 'column'
            },
            title: {
                text: 'Education',
                style: { "color": "#666666", "fontSize": "36px" }
            },
            xAxis: {
                categories: ['Ph\'D+', 'Bachelor\'s Degree', 'Master\'s Degree', 'Below High School', 'High School Diploma'],
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
                    '<td style="padding:0"><b>{point.y:.1f} mm</b></td></tr>',
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
                }
            },
            series: [{
                name: 'Education',
                showInLegend: false,
                data: window.educationData
            }]
        });
    }
});