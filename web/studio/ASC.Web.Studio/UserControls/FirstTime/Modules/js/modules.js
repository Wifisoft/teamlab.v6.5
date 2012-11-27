
ModulesContentManager = function() {
    this.SaveModules = function(parentCallback) {

        var settings = new Array();
        jq('div[id^="itm"]').each(function(i, pel) {
            var pid = jq(this).attr('name');
            var iOpt = { SortOrder: i,
                ItemID: pid,
                Disabled: !jq('#studio_poptDisabled_' + pid).is(':checked'),
                CustomName: '',
                ChildItemIDs: new Array()
            };

            settings.push(iOpt);
        });
        ModulesController.SaveModules(settings, function(result) { if (parentCallback != null) parentCallback(result.value); });
    }
}