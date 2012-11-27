/*sharing data sample
    {
        actions : [{name: 'actionName',
                    id  : 'id',
                    defaultAction : 'true/false'
                  }],
                  
        items : [{
            id : 'itemId',
            data : 'customData',
            isGroup : true,
            name : 'name',
            canEdit : true/false,
            selectedAction : {
                                name: 'actionName',
                                id  : 'id'
                             }
            
        }]
    }
*/

var SharingSettingsManager = function(elementId, sharingData) {

    var clone = function(o) {
        if (!o || 'object' !== typeof o)
            return o;

        var c = 'function' === typeof o.pop ? [] : { };
        var p, v;
        for (p in o) {
            if (o.hasOwnProperty(p)) {
                v = o[p];
                if (v && 'object' === typeof v) {
                    c[p] = clone(v);
                }
                else {
                    c[p] = v;
                }
            }
        }
        return c;
    };

    this.OnSave = null;

    var _data = sharingData;
    var _workData = clone(sharingData);

    var _manager = this;
    jq(function() {
        if (elementId != undefined)
            jq('#' + elementId).click(function() {
                _manager.ShowDialog();
            });

        jq('#sharingSettingsSaveButton').click(_manager.SaveAndCloseDialog);
        jq('#sharingSettingsCancelButton').click(_manager.CloseDialog);

        jq('#studio_sharingSettingsDialog .removeItem').live('click', function() { RemoveItem(jq(this)); });
        jq('#sharingSettingsItems .action select').live('change', function() { ChangeItemAction(jq(this)); });

        jq("#sharingSettingsItems .combobox-title").live("click", function() { jq("#sharingSettingsItems").scrollTo(jq(".combobox-container:visible")); });

    });

    var ChangeItemAction = function(el) {
        var itemId = jq(el).attr('data');
        var actId = jq(el).val();

        var act = null;
        for (var i = 0; i < _workData.actions.length; i++) {
            if (_workData.actions[i].id == actId) {
                act = _workData.actions[i];
                break;
            }
        }

        for (var i = 0; i < _workData.items.length; i++) {
            if (_workData.items[i].id == itemId) {

                _workData.items[i].selectedAction = act;
                break;
            }
        }
    };

    var RemoveItem = function(el) {
        var itemId = jq(el).attr('data');

        for (var i = 0; i < _workData.items.length; i++) {
            if (_workData.items[i].id == itemId) {
                _workData.items.splice(i, 1);
                break;
            }
        }

        jq("#sharing_item_" + itemId).remove();
        jq("#sharingSettingsItems div.sharingItem.tintMedium").removeClass("tintMedium");
        jq("#sharingSettingsItems div.sharingItem:even").addClass("tintMedium");

        shareUserSelector.HideUser(itemId, false);
        shareGroupSelector.HideGroup(itemId, false);
    };

    var AddUserItem = function(userId, userName) {

        var defAct = null;
        for (var i = 0; i < _workData.actions.length; i++) {
            if (_workData.actions[i].defaultAction) {
                defAct = _workData.actions[i];
                break;
            }
        }
        var newItem = { id: userId, name: userName, selectedAction: defAct, isGroup: false, canEdit: true };
        _workData.items.push(newItem);

        jq('#sharingSettingsItems').append(jq("#sharingListTemplate").tmpl({ items: [newItem], actions: _workData.actions }));
        jq('#studio_sharingSettingsDialog .action select:last').tlcombobox();

        jq("#sharingSettingsItems div.sharingItem.tintMedium").removeClass("tintMedium");
        jq("#sharingSettingsItems div.sharingItem:even").addClass("tintMedium");

        shareUserSelector.HideUser(userId, true);
    };

    var AddGroupItem = function(group) {
        var defAct = null;
        for (var i = 0; i < _workData.actions.length; i++) {
            if (_workData.actions[i].defaultAction) {
                defAct = _workData.actions[i];
                break;
            }
        }
        var newItem = { id: group.Id, name: group.Name, selectedAction: defAct, isGroup: true, canEdit: true };
        _workData.items.push(newItem);

        jq('#sharingSettingsItems').append(jq("#sharingListTemplate").tmpl({ items: [newItem], actions: _workData.actions }));
        jq('#studio_sharingSettingsDialog .action select:last').tlcombobox();

        jq("#sharingSettingsItems div.sharingItem.tintMedium").removeClass("tintMedium");
        jq("#sharingSettingsItems div.sharingItem:even").addClass("tintMedium");

        shareGroupSelector.HideGroup(group.Id, true);
    };

    var ReDrawItems = function() {

        jq('#sharingSettingsItems').html(jq("#sharingListTemplate").tmpl(_workData));

        if (jq.browser.mobile) {
            jq("#sharingSettingsItems").addClass("isMobileAgent");
        }

        jq('#studio_sharingSettingsDialog .action select').each(function() {
            jq(this).tlcombobox();
        });

        shareUserSelector.AdditionalFunction = AddUserItem;
        shareUserSelector.DisplayAll();

        shareGroupSelector.AdditionalFunction = AddGroupItem;
        shareGroupSelector.DisplayAll();

        for (var i = 0; i < _workData.items.length; i++) {
            var item = _workData.items[i];
            if (item.isGroup) {
                shareGroupSelector.HideGroup(item.id, true);
            }
            else {
                shareUserSelector.HideUser(item.id, true);
            }
        }
    };

    this.UpdateSharingData = function(data) {
        _data = data;
        _workData = clone(data);
    };

    this.GetSharingData = function() {
        return _data;
    };

    this.ShowDialog = function() {

        ReDrawItems();

        jq.blockUI({
                message: jq("#studio_sharingSettingsDialog"),
                css: {
                    opacity: '1',
                    border: 'none',
                    padding: '0px',
                    width: '600px',
                    height: 'auto',
                    "min-height": '600px',
                    cursor: 'default',
                    textAlign: 'left',
                    backgroundColor: 'transparent',
                    marginLeft: '-300px',
                    left: '50%',
                    top: '50%',
                    position: "absolute",
                    overflow: "visible",
                    "overflow-x": (jq.browser.msie && jq.browser.version <= 8) ? "auto" : "visible",
                    "overflow-y": (jq.browser.msie && jq.browser.version <= 8) ? "auto" : "visible",
                    "margin-top": (jq(window).scrollTop() - 300) + "px"
                },
                overlayCSS: {
                    backgroundColor: '#AAA',
                    cursor: 'default',
                    opacity: '0.3'
                },
                focusInput: true,
                fadeIn: 0,
                fadeOut: 0
            });

        PopupKeyUpActionProvider.EnterAction = "jq('#sharingSettingsSaveButton').click();";
    };

    this.SaveAndCloseDialog = function() {
        data = _workData;

        if (_manager.OnSave != null)
            _manager.OnSave(data);

        PopupKeyUpActionProvider.CloseDialog();
        return false;
    };

    this.CloseDialog = function() {
        PopupKeyUpActionProvider.CloseDialog();
        return false;
    };
};