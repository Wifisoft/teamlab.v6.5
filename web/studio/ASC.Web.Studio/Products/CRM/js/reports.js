if (typeof ASC === "undefined")
    ASC = {};

if (typeof ASC.CRM === "undefined")
    ASC.CRM = function() { return {} };

ASC.CRM.Reports = (function() {

    var callbackMethods = {
        getContacts: function() {alert("1"); }
    };

    var bred = function() {
        var allData = [
          { label: "Данные 1", color: 0, data: [["2010/10/01", 0], ["2010/11/01", 1], ["2010/12/01", 7]]},
          { label: "Данные 2", color: 1, data: [["2010/10/01", 13], ["2010/11/01", 23], ["2010/12/01", 32]]}
        ];
        // преобразуем даты в UTC
        for(var j = 0; j < allData.length; ++j) {
            for (var i = 0; i < allData[j].data.length; ++i)
                allData[j].data[i][0] = Date.parse(allData[j].data[i][0]);
        }
        // свойства графика
        var plotConf = {
            series: {
                lines: {
                    show: true,
                    lineWidth: 2
                }
            },
            xaxis: {
                mode: "time",
                timeformat: "%y/%m/%d"
            }
        };

        // выводим график
        jq.plot(jq("#placeholder"), allData, plotConf);
    };

    function bred1() {
        bred2();
    }

    function bred2() {
        alert("3");
    }

    return {
            callbackMethods : callbackMethods,
            bred : bred,
            bred1 : bred1
    };
})();