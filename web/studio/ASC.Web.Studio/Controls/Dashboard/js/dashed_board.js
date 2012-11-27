
var DashedBoard = function(varName, containerID, defaultContainerID) {

    this.DefaultContainerID = defaultContainerID;
    this.ContainerID = containerID;
    this.VarName = varName;
    this.IsUpdate = false;

    jq(document).ready(function() {
        eval(varName + ".InitSortable();");
    });

    this.InitSortable = function() {
        var varName = this.VarName;

        //sort widgets
        jq('ul[id^="widgetCol_' + this.ContainerID + '_"]').sortable({

            connectWith: 'ul[id^="widgetCol_' + this.ContainerID + '_"]',
            cursor: 'move',

            items: '>li[id^="widgetBox_' + this.ContainerID + '_"]',
            handle: 'td[id^="widgetHandle_' + this.ContainerID + '_"]',

            start: function(event, ui) {
                eval(varName + '.IsUpdate = false;');
            },

            update: function(event, ui) {
                var code = 'if(ui.sender == null) ' + varName + '.IsUpdate = true; ';
                code += 'else if(jq("#widgetDHM_"+' + varName + '.ContainerID' + '+"_' + jq(ui.item).attr('name') + '").val()==0) ' + varName + '.IsUpdate = true; ';
                code += 'else{ ' + varName + '.IsUpdate = false; jq(ui.sender).sortable("cancel"); } ';

                eval(code);
            },

            stop: function(event, ui) {
                eval('if(' + varName + '.IsUpdate) ' + varName + '.SaveWidgetPosition();');
            },

            dropOnEmpty: true,
            placeholder: 'studioWidgetBoxEmpty',
            forcePlaceholderSize: true
        });
    };

    //collapsing widgets
    this.MinimizeMaximize = function(widgetID) {
        if (jq('#widgetContent_' + this.ContainerID + '_' + widgetID).is(':visible')) {
            jq('#widgetContent_' + this.ContainerID + '_' + widgetID).slideUp('slow');
            jq('#widgetDescription_' + this.ContainerID + '_' + widgetID).slideDown('slow');
            jq('#widgetState_' + this.ContainerID + '_' + widgetID).attr('src', SkinManager.GetImage('widget_expand.png'));

            AjaxPro.onLoading = function(b) { };
            WidgetControl.ToggleWidget(widgetID, this.ContainerID, this.DefaultContainerID, true, function(result) { })
        }
        else {
            jq('#widgetContent_' + this.ContainerID + '_' + widgetID).slideDown('slow');
            jq('#widgetDescription_' + this.ContainerID + '_' + widgetID).slideUp('slow');
            jq('#widgetState_' + this.ContainerID + '_' + widgetID).attr('src', SkinManager.GetImage('widget_collapse.png'));

            AjaxPro.onLoading = function(b) { };
            WidgetControl.ToggleWidget(widgetID, this.ContainerID, this.DefaultContainerID, false, function(result) { })
        }
    };

    //save widgets states
    this.SaveWidgetPosition = function() {
        var columnsArray = new Array();
        var widgetColumns = jq('ul[id^="widgetCol_' + this.ContainerID + '_"]');

        var positions = new String();
        jq('ul[id^="widgetCol_' + this.ContainerID + '_"]').each(function(j, column) {
            if (j != 0)
                positions += '@';
            positions += jq(this).attr('name');
            positions += '$';

            jq(this).children('li').each(function(i, widget) {
                if (i != 0)
                    positions += ',';
                positions += jq(this).attr('name');
            });
        });

        AjaxPro.onLoading = function(b) { };
        WidgetControl.SortWidgets(this.ContainerID, this.DefaultContainerID, positions, function(result) { });
    };

    this.ShowSettings = function() {
        jq('#widgetSettingsDialogMessage_' + this.ContainerID).hide();
        try {
            jq.blockUI({ message: jq("#widgetSettingsDialog_" + this.ContainerID),
                css: {
                    opacity: '1',
                    border: 'none',
                    padding: '0px',
                    /*width: '800px',*/
                    width: '400px',
                    height: '550px',
                    position: 'absolute',
                    cursor: 'default',
                    textAlign: 'left',
                    backgroundColor: 'Transparent',
                    /*marginLeft: '-400px',*/
                    marginLeft: '-200px',
                    top: '50px'
                },

                overlayCSS: {
                    backgroundColor: '#aaaaaa',
                    cursor: 'default',
                    opacity: '0.3'
                },
                focusInput: false,
                fadeIn: 0,
                fadeOut: 0
            });
        }
        catch (e) { };

        PopupKeyUpActionProvider.ClearActions();
        PopupKeyUpActionProvider.EnterAction = this.VarName + '.SaveSettings();';

        jq('#widgetSettingsDialogMessage_' + this.ContainerID).html('');
    };

    //save widgets settings
    this.SaveSettings = function() {
        var widgetItems = new Array();
        var contaunerID = this.ContainerID;
        jq(':checkbox[id^="widgetItem_' + this.ContainerID + '_"]:checked').each(function(i, el) {

            var item = new Object();
            item.ID = jq(this).val();
            item.SettingsProviderType = jq('#widgetProviderType_' + contaunerID + '_' + item.ID).val();
            item.Settings = new Array();
            //fill settings
            jq('input[id^="widgetSettings_' + contaunerID + '_' + item.ID + '"]').each(function(i, el) {

                var setting = new Object();

                var parts = jq(this).attr('id').split('_');
                if (jq(this).attr('type') == 'text') {
                    setting.Value = jq(this).val();
                }
                else {
                    setting.Value = jq(this).is(':checked');
                }
                setting.ID = parts[parts.length - 1];
                setting.Title = '';
                setting.Description = '';
                item.Settings.push(setting);

            });

            widgetItems.push(item);
        });

        var schemaID = jq(':radio:checked[name^="widgetSchema_' + this.ContainerID + '"]').val();

        AjaxPro.onLoading = function(b) {
            if (b) {
                //jq("#widgetSettingsDialog_"+this.ContainerID).block();
                jq("#widgetSettings_panel_buttons").hide();
                jq("#widgetSettings_action_loader").show();
            }
            else {
                jq("#widgetSettings_action_loader").hide();
                jq("#widgetSettings_panel_buttons").show();
                //jq("#widgetSettingsDialog_"+this.ContainerID).unblock();
            }
        };

        WidgetControl.SaveSettings(this.DefaultContainerID, this.ContainerID, (schemaID == null ? -1 : schemaID), jq('#widgetContainerTitle_' + this.ContainerID).val(), widgetItems, function(result) {
            var res = result.value;
            if (res.Success == '1') {
                jq.unblockUI();
                window.location.reload(true);
            }
            else {
                jq('#widgetSettingsDialogMessage_' + res.ContainerID).html('<div>' + res.Message + '</div>');
                jq('#widgetSettingsDialogMessage_' + res.ContainerID).show();
            }

        });
    };

    this.CancelSettings = function() {
        PopupKeyUpActionProvider.ClearActions();
        jq.unblockUI();
    };
};