
NamingPeopleContentManager = function() {

    this.SaveSchema = function(parentCallback) {
        var schemaId = jq('#namingPeopleSchema').val();

        if (schemaId == 'custom') {
            NamingPeopleContentController.SaveCustomNamingSettings(jq('#usrcaption').val().substring(0, 30), jq('#usrscaption').val().substring(0, 30),jq('#addusrscaption').val().substring(0, 30),
                                                       jq('#grpcaption').val().substring(0, 30), jq('#grpscaption').val().substring(0, 30),
                                                       jq('#usrstatuscaption').val().substring(0, 30), jq('#regdatecaption').val().substring(0, 30),
                                                       jq('#grpheadcaption').val().substring(0, 30), jq('#globalheadcaption').val().substring(0, 30),
                                                       function(result) { if (parentCallback != null) parentCallback(result.value); });
        }
        else
            NamingPeopleContentController.SaveNamingSettings(schemaId, function(result) { if (parentCallback != null) parentCallback(result.value); });
    }

    this.SaveSchemaCallback = function(res) {
    }

    this.LoadSchemaNames = function(parentCallback) {

        var schemaId = jq('#namingPeopleSchema').val();
        NamingPeopleContentController.GetPeopleNames(schemaId, function(res) {
            var names = res.value;

            jq('#usrcaption').val(names.UserCaption);
            jq('#usrscaption').val(names.UsersCaption);
            jq('#grpcaption').val(names.GroupCaption);
            jq('#grpscaption').val(names.GroupsCaption);
            jq('#usrstatuscaption').val(names.UserPostCaption);
            jq('#regdatecaption').val(names.RegDateCaption);
            jq('#grpheadcaption').val(names.GroupHeadCaption);
            jq('#globalheadcaption').val(names.GlobalHeadCaption);
            jq('#addusrscaption').val(names.AddUsersCaption);

            if (parentCallback != null)
                parentCallback(res.value);
        });
    }
}

NamingPeopleContentViewer = new function() {
    this.ChangeValue = function(event) {
    jq('#namingPeopleSchema').val('custom');
    }
};

jq(function() {
    jq('.namingPeopleBox input[type="text"]').each(function(i, el) {
        jq(el).keypress(function(event) { NamingPeopleContentViewer.ChangeValue() });
    });
    var manager = new NamingPeopleContentManager();
    jq('#namingPeopleSchema').change(function() {
        manager.LoadSchemaNames(null);
    })
    manager.LoadSchemaNames(null);
});