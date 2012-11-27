(function($) {
    var 
    colors = {
        hits: '#EDC240',
        hosts: '#AFD8F8',
        filterbyweek: '#EDC240',
        filterbymonth: '#CB4B4B',
        filterby3months: '#AFD8F8',
        filterbyperiod: '#4DA74D'
    },
    displayDates = {},
    chartData = {},
    lastFilter = '';

    function managePeriodFilter(toggle) {
        if (toggle === true) {
            $('#periodSelection').removeClass('disabled');
            $('#startDate input').removeAttr('disabled');
            $('#endDate input').removeAttr('disabled');
        } else {
            $('#periodSelection').addClass('disabled');
            $('#startDate input').attr('disabled', 'disabled');
            $('#endDate input').attr('disabled', 'disabled');
        }
    }

    function changePeriod(evt) {
        var from, to;

        try {
            from = jq.datepick.parseDate(jq.datepick._defaults.dateFormat, jq('input[id$=studio_chart_FromDate]').val());
            to = jq.datepick.parseDate(jq.datepick._defaults.dateFormat, jq('input[id$=studio_chart_ToDate]').val());
            from = new Date(Date.UTC(from.getFullYear(), from.getMonth(), from.getDate()));
            to = new Date(Date.UTC(to.getFullYear(), to.getMonth(), to.getDate()));
        }
        catch (e) { }

        if (from instanceof Date && isFinite(from.getTime()) && to instanceof Date && isFinite(to.getTime()) && from.getTime() < to.getTime()) {
            $('#visitorsChartCanvas').addClass('largeLoading').empty();
            $('#chartDownloadStatistics').addClass('disabled');
            $('#chartLegend').children('li.label:not(.default)').remove();
            VisitorsChart.GetVisitStatistics(from, to, showChart);
        }
        else {
            $('#visitorsChartCanvas').empty();
            $('#chartDownloadStatistics').addClass('disabled');
            $('#chartLegend').children('li.label:not(.default)').remove();
        }
    }

    function showChart(param) {
        if (!param) {
            return undefined;
        }
        $('#visitorsChartCanvas').removeClass('largeLoading');
        if (typeof param === 'string' && (param = param.toLowerCase()).length > 0) {
            var 
        from = new Date(),
        to = new Date();
            from = new Date(Date.UTC(from.getFullYear(), from.getMonth(), from.getDate()));
            to = new Date(Date.UTC(to.getFullYear(), to.getMonth(), to.getDate()));

            switch (param) {
                case 'filterbyweek':
                    $('#visitorsFilter').find('li.filter').removeClass('selected');
                    $('#filterByWeek').addClass('selected');
                    managePeriodFilter(false);
                    from.setDate(to.getDate() - 6);
                    break;
                case 'filterbymonth':
                    $('#visitorsFilter').find('li.filter').removeClass('selected');
                    $('#filterByMonth').addClass('selected');
                    managePeriodFilter(false);
                    from.setMonth(to.getMonth() - 1);
                    break;
                case 'filterby3months':
                    $('#visitorsFilter').find('li.filter').removeClass('selected');
                    $('#filterBy3Months').addClass('selected');
                    managePeriodFilter(false);
                    from.setMonth(to.getMonth() - 3);
                    break;
                case 'filterbyperiod':
                    $('#visitorsFilter').find('li.filter').removeClass('selected');
                    $('#filterByPeriod').addClass('selected');
                    managePeriodFilter(true);

                    from = jq.datepick.parseDate(jq.datepick._defaults.dateFormat, jq('[id$=studio_chart_FromDate]').val());
                    to = jq.datepick.parseDate(jq.datepick._defaults.dateFormat, jq('[id$=studio_chart_ToDate]').val());


                    if (from instanceof Date && to instanceof Date) {
                        from = new Date(Date.UTC(from.getFullYear(), from.getMonth(), from.getDate()));
                        to = new Date(Date.UTC(to.getFullYear(), to.getMonth(), to.getDate()));
                    }

                    break;
                default:
                    return undefined;
            }
            lastFilter = param;
            if (from instanceof Date && isFinite(from.getTime()) && to instanceof Date && isFinite(to.getTime()) && from.getTime() < to.getTime()) {
                $('#visitorsChartCanvas').addClass('largeLoading').empty();
                $('#chartDownloadStatistics').addClass('disabled');
                $('#chartLegend').children('li.label:not(.default)').remove();
                VisitorsChart.GetVisitStatistics(from, to, arguments.callee);
            }
        } else if (typeof param === 'object' && param.hasOwnProperty('value') && param.value) {
            var 
        date = null,
        hits = [],
        hosts = [];
            switch (lastFilter) {
                case 'filterbyweek':
                case 'filterbymonth':
                case 'filterby3months':
                case 'filterbyperiod':
                    break;
                default:
                    return undefined;
            }

            param = param.value;

            for (var i = 0, n = param.length; i < n; i++) {
                param[i].Date.setUTCHours(0, 0, 0);
                displayDates[param[i].Date.getTime()] = param[i].DisplayDate;
                hits.push([param[i].Date, param[i].Hits]);
                // hosts.push([param[i].Date, param[i].Hosts]);
            }

            $('#chartDownloadStatistics').removeClass('disabled');
            $('#chartLegend').children('li.label:not(.default)').remove();

            var $label = $('#chartLegend').children('li.label.default:first').clone().removeClass('default');
            $label.find('div.color:first').css('backgroundColor', colors.hits);
            $label.find('div.title:first').html(ASC.Resources.hitLabel);
            $('#chartLegend').append($label);

            $.plot(
        $('#visitorsChartCanvas'),
        [
          {
              label: ASC.Resources.hitLabel,
              color: colors.hits,
              data: hits
          }
            //          {
            //            label : ASC.Resources.hostLabel,
            //            color : colors.hosts,
            //            data  : hosts
            //          }
        ],
        {
            grid: { hoverable: true, clickable: true },
            legend: { show: false },
            series: { lines: { show: true }, points: { show: true, radius: 2} },
            xaxis: { mode: 'time', timeformat: ASC.Resources.chartDateFormat, monthNames: ASC.Resources.chartMonthNames.split(/\s*,\s*/) },
            yaxis: { min: 0 }
        }
      );
        }
    }

    $(document).ready(function() {
      $('#visitorsChartCanvas')
        .bind("plothover", function(evt, pos, item) {
          if (item) {
            if (!displayDates.hasOwnProperty(item.datapoint[0])) {
              return undefined;
            }
            var content =
              '<h6 class="label">' + item.series.label + ' : ' + displayDates[item.datapoint[0]] + '</h6>' +
              '<div class="info">' + item.datapoint[1] + ' visits' + '</div>';
            ASC.Common.toolTip.show(content, function() {
              var $this = $(this);
              $this.css({
                  left: item.pageX + 5,
                  top: item.pageY - $this.outerHeight(true) - 5
              });
            });
          } else {
            ASC.Common.toolTip.hide();
          }
        });

      $('input[id$=studio_chart_FromDate], input[id$=studio_chart_ToDate]').mask(jq('input[id$=jQueryDateMask]').val());

      var
        defaultFromDate = new Date(),
        defaultToDate = new Date();
      defaultFromDate.setMonth(defaultFromDate.getMonth() - 6);

      $('input[id$=studio_chart_FromDate]')
        .val($.datepick.formatDate(defaultFromDate))
        .datepick({
          onSelect : function (dates) {
            var date = dates[0];
            date.setDate(date.getDate() + 1);
            $('input[id$=studio_chart_ToDate]').datepick('option', 'minDate', date || null);
            changePeriod();
          },
          selectDefaultDate : true,
          maxDate : -1,
          showAnim : ''
        });

      $('input[id$=studio_chart_ToDate]')
        .val($.datepick.formatDate(defaultToDate))
        .datepick({
          onSelect : function (dates) {
            var date = dates[0];
            date.setDate(date.getDate() - 1);
            $('input[id$=studio_chart_FromDate]').datepick('option', 'maxDate', date || null);
            changePeriod();
          },
          selectDefaultDate : true,
          minDate : '-6m +1d',
          showAnim : ''
        });

      $('#chartDownloadStatistics').click(function() {
        return false;
      });

      $('#visitorsFilter')
        .css('visibility', 'visible')
        .click(function(evt) {
          var $target = $(evt.target);
          if ($target.is('li.filter') && !$target.is('li.filter.selected')) {
            showChart($target.attr('id'));
          }
        });

      showChart('filterbyweek');
    });
})(jQuery);
