window.ASC.Files.Filter = (function() {
    var isInit = false;

    var advansedFilter = null;

    var init = function() {
        if (isInit === false) {
            isInit = true;
        }
    };

    var disableFilter = function() {
        var isMy = ASC.Files.Folders.folderContainer == "my";
        var isForMe = ASC.Files.Folders.folderContainer == "forme";

        ASC.Files.Filter.advansedFilter.advansedfilter("sort", !isForMe);
        if (isForMe) {
            ASC.Files.Filter.advansedFilter.advansedFilter({
                sorters: [{ id: "DateAndTime", def: true, dsc: true }]
            });
        }

        ASC.Files.Filter.advansedFilter.advansedFilter({
            filters: [
                { id: "selected-person", visible: !isMy },
                { id: "selected-group", visible: !isMy }
            ],
            sorters: [
                { id: "Author", visible: !isMy }
            ]
        });
    };

    var setFilter = function(evt, $container, filter, params, selectedfilters) {
        if (!isInit) {
            ASC.Files.Filter.init();
            return;
        }

        ASC.Files.Folders.navigationSet(ASC.Files.Folders.currentFolderId);
    };

    var resetFilter = function(evt, $container, filter) {
        if (!isInit) {
            ASC.Files.Filter.init();
            return;
        }

        ASC.Files.Folders.navigationSet(ASC.Files.Folders.currentFolderId);
    };

    var clearFilter = function() {
        ASC.Files.Filter.advansedFilter.advansedFilter(null);
        ASC.Files.Folders.navigationSet(ASC.Files.Folders.currentFolderId);
    };

    var getFilterSettings = function() {
        var settings =
            {
                sorter: ASC.Files.Filter.getOrderByDateAndTime(false),
                text: "",
                filter: 0,
                subject: ""
            };

        if (ASC.Files.Filter.advansedFilter == null) return settings;

        var param = ASC.Files.Filter.advansedFilter.advansedFilter();
        jq(param).each(function(i, item) {
            switch (item.id) {
            case "sorter":
                settings.sorter = getOrderBy(item.params.id, !item.params.dsc);
                break;
            case "text":
                settings.text = item.params.value;
                break;
            case "selected-person":
                settings.filter = 8;
                settings.subject = item.params.id;
                break;
            case "selected-group":
                settings.filter = 9;
                settings.subject = item.params.id;
                break;
            case "selected-type":
                settings.filter = parseInt(item.params.value || 0);
                break;
            }
        });

        if (ASC.Files.Folders.folderContainer == "forme")
            settings.sorter = ASC.Files.Filter.getOrderByDateAndTime(false);

        return settings;
    };

    var getOrderByDateAndTime = function(asc) {
        return getOrderBy("DateAndTime", asc);
    };

    var getOrderByAZ = function(asc) {
        return getOrderBy("AZ", asc);
    };

    var getOrderBy = function(name, asc) {
        name = (typeof name != "undefined" && name != "" ? name : "DateAndTime");
        asc = asc === true;
        return {
            is_asc: asc,
            property: name
        };
    };

    return {
        init: init,
        advansedFilter: advansedFilter,
        disableFilter: disableFilter,

        getFilterSettings: getFilterSettings,

        getOrderByDateAndTime: getOrderByDateAndTime,
        getOrderByAZ: getOrderByAZ,

        setFilter: setFilter,
        resetFilter: resetFilter,
        clearFilter: clearFilter
    };
})();

(function($) {
    $(function() {

        jq("#files_clearFilter").click(function() {
            ASC.Files.Filter.clearFilter();
            return false;
        });

    });
})(jQuery);