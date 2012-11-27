var SpecRules =
{
    Allow: 0,
    Deny: 1,
    Restrictions: 2
};

var RowChangeType =
{
    OK: 0,
    CHANGE: 1,
    DELETE: 2
};

AccessSettingsManager = new function() {

    this.TimeoutHandler = null;
    this._ips = '';
    this.addUserLabel = 'Add IP address';

    this.AddUserItem = function(userId, userName) {
        AccessSettingsManager.AddChildElement(jq('#userList').find('.userItem').length, jq('#userList'), { "ID": userId, "Name": userName, "IPs": '' });
        shareUserSelector.DisplayAll();
    };

    this.AddGroupItem = function(group) {
        AccessSettingsManager.AddChildElement(jq('#userList').find('.userItem').length, jq('#userList'), { "ID": group.Id, "Name": group.Name, "IPs": '' });
    };

    this.ChangeRuleType = function(obj) {
        switch (parseInt(jq(obj).val())) {
        case SpecRules.Restrictions:
            jq('#dvSpecialRules').css('display', 'block');
            jq('#dvOwnerNote').css('display', 'none');
            break;

        case SpecRules.Deny:
            jq('#dvOwnerNote').css('display', 'block');
            jq('#dvSpecialRules').css('display', 'none');
            break;

        case SpecRules.Allow:
            jq('#dvSpecialRules').css('display', 'none');
            jq('#dvOwnerNote').css('display', 'none');
            break;
        }
    };

    this.GetRuleType = function() {

        var radioButtons = document.getElementsByName('commonRules');
        for (var x = 0; x < radioButtons.length; x++) {
            if (radioButtons[x].checked) {
                return jq(radioButtons[x]).val();
            }
        }
        return null;
    };

    this.SetRuleType = function(rule) {

        var radioButtons = document.getElementsByName('commonRules');
        for (var x = 0; x < radioButtons.length; x++) {
            if (jq(radioButtons[x]).val() == rule) {
                radioButtons[x].checked = true;
                jq(radioButtons[x]).click();
            }
        }
    };

    this.LoadSettings = function() {
        AccessSettingsController.LoadUsersRules(function(result) {
            if (result == null)
                return;
            AccessSettingsManager.ParseUserDataCallback(result.value);
        });
    };

    this.ParseUserDataCallback = function(res) {
        if (res == null)
            return;
        var jsonObj = JSON.parse(res);
        AccessSettingsManager.SetRuleType(jsonObj.Rule);

        for (var i = 0; i < jsonObj.UserRules.length; i++) {
            AccessSettingsManager.AddChildElement(i, jq('#userList'), jsonObj.UserRules[i]);
        }
    };

    this.ShowIPBlock = function(parent) {
        var position = jq(parent).position();
        jq('#addTagDialog').css('left', position.left + 50);
        jq('#addTagDialog').css('top', position.top + 18);
        jq('#addTagDialog').css('display', 'block');
        jq('#addTagDialog .textEdit').val('');

        var elem = jq(parent).parent().parent().find('div.ips');

        var row = jq(parent).parent().parent().attr('id').toString().split('_')[1];

        jq('#addTagDialog #addThisTag').bind('click', function() {
            elem.append(AccessSettingsManager.GetOneIPObj(row, jq('#addTagDialog .textEdit').val()));
            jq('#addTagDialog .textEdit').val('');
            jq('#hdCng_' + row).val(RowChangeType.CHANGE);
            jq('#addTagDialog').css('display', 'none');
            jq('#addTagDialog #addThisTag').unbind('click');
        });
    };

    this.AddChildElement = function(objNum, parent, object) {

        var list = '';
        if (object.IPs.toString().length > 0) {
            for (var i = 0; i < object.IPs.toString().split(',').length; i++) {
                list += this.GetOneIPObj(objNum, object.IPs.toString().split(',')[i], i);
            }
        }

        parent.append('<tr class="userItem" id="item_' + objNum + '"><td class="name"><input type="hidden" value="" class="hdCng" id="hdCng_' + objNum + '" /><input class="hdG" type="hidden" value="' + object.ID + '" id="hd_' + objNum + '" />' + object.Name + '</td><td class="link"><a class="addAddress" href="javascript:void(0);" onclick="AccessSettingsManager.ShowIPBlock(this);">' + AccessSettingsManager.addUserLabel + '</a></td><td class="ips" id="ips_' + objNum + '"><div class="clearFix"><div class="ips clearFix">' + list + '</div><div class="editable"><div class="clearFix params" style="display:none"><div class="delete" id="del_' + objNum + '"></div></div></div></td></tr>');
        //parent.append('<tr class="userItem" id="item_' + objNum + '"><td class="name"><input type="hidden" value="" class="hdCng" id="hdCng_' + objNum + '" /><input class="hdG" type="hidden" value="' + object.ID + '" id="hd_' + objNum + '" />' + object.Name + '</td><td class="ips" id="ips_' + objNum + '"><div class="clearFix"><div class="ips">' + object.IPs + '</div><div class="editable"><div class="clearFix params" style="display:none"><div class="edit" id="edit_' + objNum + '"></div><div class="delete" id="del_' + objNum + '"></div></div></div></td></tr>');
        jq('#edit_' + objNum).bind('click', function() { AccessSettingsManager.ShowEditableBlock(objNum); });
        jq('#del_' + objNum).bind('click', function() { AccessSettingsManager.DeleteRecord(objNum); });
        jq('#item_' + objNum).mouseover(function() { AccessSettingsManager.ShowEditablePanel(objNum); });
        jq('#item_' + objNum).mouseout(function() { AccessSettingsManager.HideEditablePanel(objNum); });
    };

    this.AddOneIP = function(objNumber, address) {
        jq('ips_' + objNumber + ' .ips').append(this.GetOneIPObj(objNumber, address));
    };

    this.GetOneIPObj = function(num, address) {
        return '<div class="addressBlock clearFix"><div class="singleAddress">' + address + '</div><div class="deleteBlock"><div class="delete" onclick="AccessSettingsManager.DeleteOneIPObj(this,' + num + ');"></div></div></div>';
    };

    this.DeleteOneIPObj = function(elem, objNum) {
        jq('#hdCng_' + objNum).val(RowChangeType.CHANGE);
        // wrong way
        jq(elem).parent().parent().remove();
    };

    this.ShowEditablePanel = function(objNum) {
        jq('#item_' + objNum + " .ips .editable div.params").css('display', 'inline');
    };

    this.HideEditablePanel = function(objNum) {
        jq('#item_' + objNum + " .ips .editable div.params").css('display', 'none');
    };

    this.ShowEditableBlock = function(objNum) {
        var newData = '';
        if (this._ips == '')
            this._ips = jq('#ips_' + objNum + ' div.ips').html();

        if (this._ips != null)
            newData = this._ips.replace( /,/g , "\n");


        var items = jq('#userList').find(".userItem");
        jQuery.each(items, function(index) {
            if (index != objNum) {
                jq(items[index]).unbind('mouseover');
                jq(items[index]).unbind('mouseout');
            }
        });

        jq('#studio_accessSettings #userList #ips_' + objNum + ' div.ips').html('');
        jq('#studio_accessSettings #userList #ips_' + objNum + ' div.ips').append('<textarea cols="15" class="area" id="txIps_' + objNum + '">' + newData + '</textarea>');
        jq('#item_' + objNum).addClass("editingItem");
        jq('#edit_' + objNum).unbind('click');
        jq('#del_' + objNum).unbind('click');
        jq('#edit_' + objNum).bind('click', function() { AccessSettingsManager.HideEditableBlock(objNum, RowChangeType.CHANGE); });
        jq('#del_' + objNum).bind('click', function() { AccessSettingsManager.HideEditableBlock(objNum, RowChangeType.OK); });
    };

    this.HideEditableBlock = function(objNum, chType) {
        var newData;
        if (chType == RowChangeType.CHANGE) {
            newData = jq('#txIps_' + objNum).val().replace( /\n/g , ",");
            jq('#hdCng_' + objNum).val(RowChangeType.CHANGE);
        }
        else
            newData = this._ips;

        jq('#studio_accessSettings #userList #item_' + objNum).removeClass("editingItem");
        jq('#ips_' + objNum + ' div.ips').html('');
        jq('#ips_' + objNum + ' div.ips').html(newData);
        this._ips = '';

        var items = jq('#userList').find(".userItem");
        jQuery.each(items, function(index) {
            jq(items[index]).mouseover(function() { AccessSettingsManager.ShowEditablePanel(index); });
            jq(items[index]).mouseout(function() { AccessSettingsManager.HideEditablePanel(index); });
        });

        jq('#edit_' + objNum).unbind('click');
        jq('#del_' + objNum).unbind('click');
        jq('#edit_' + objNum).bind('click', function() { AccessSettingsManager.ShowEditableBlock(objNum); });
        jq('#del_' + objNum).bind('click', function() { AccessSettingsManager.DeleteRecord(objNum); });
    };


    this.SaveSettings = function() {

        var rule = AccessSettingsManager.GetRuleType();
        var users = null;

        if (rule == SpecRules.Restrictions)
            users = AccessSettingsManager.GetUsers();
        var jObj = { "Rule": rule, "UserRules": users };
        AccessSettingsController.SaveUsersRules(JSON.stringify(jObj), function(result) {

            var res = result.value;
            if (res.Status == 1) {
                if (rule != SpecRules.Restrictions)
                    jq('#userList').html('');
                jq('#studio_accessSettingsInfo').html('<div class="okBox">' + res.Message + '</div>');
            }
            else
                jq('#studio_accessSettingsInfo').html('<div class="errorBox">' + res.Message + '</div>');

            PasswordSettingsManager.TimeoutHandler = setTimeout(function() { jq('#studio_accessSettingsInfo').html(''); }, 4000);

        });
    };

    this.GetUsers = function() {
        var items = jq('#userList').find('.userItem');
        var arr = new Array();
        for (var i = 0; i < items.length; i++) {
            var val = jq(items[i]).find('.hdCng').val();
            if (val == '')
                continue;
            if (val == RowChangeType.CHANGE || val == RowChangeType.DELETE) {
                arr.push({ "ID": jq(items[i]).find('.hdG').val(), "IPs": AccessSettingsManager.GetIPsForUser(jq(items[i]).find('div.ips .addressBlock .singleAddress')), "Keep": val == RowChangeType.CHANGE ? true : false });
            }
        }
        return arr;
    };

    this.GetIPsForUser = function(elem) {
        var ips = new Array();
        elem.each(function(index, el) {
            ips.push(jQuery.trim(jq(el).html()));
        });

        return ips;
    };

    this.DeleteRecord = function(objNum) {

        jq('#hdCng_' + objNum).val(RowChangeType.DELETE);
        jq('#item_' + objNum).attr('disabled', 'disabled');
        jq('#item_' + objNum).addClass('remove');
        jq('#edit_' + objNum).unbind('click');
        jq('#del_' + objNum).unbind('click');
    };

    this.GetUserObj = function(userid, ips) {
        return { "ID": userid,
            "IPs": ips.split('\n'),
            "Name": ''
        };
    };
};

jq(function() {
    jq('#saveAccessSettingsBtn').click(AccessSettingsManager.SaveSettings);
    jq('#rbAllow').val(SpecRules.Allow);
    jq('#rbDeny').val(SpecRules.Deny);
    jq('#rbSpecialRules').val(SpecRules.Restrictions);

    AccessSettingsManager.LoadSettings();
    shareUserSelector.AdditionalFunction = AccessSettingsManager.AddUserItem;
    accessGroupSelector.AdditionalFunction = AccessSettingsManager.AddGroupItem;
});


jq(document).click(function(event) {
    var elt = (event.target) ? event.target : window.event;

    if (jq(elt).attr('class') == 'addAddress')
        return;
    var isHide = true;

    jq(elt).parents().each(function(i, el) {
        if (jq(el).attr('id') == 'addTagDialog') {
            isHide = false;
            return false;
        }
    });
    if (isHide) {
        jq('#addTagDialog').css('display', 'none');
    }

});