
NamingPeopleManager = new function() {

    this.MessageTimer = null;

    this.SaveSchema = function() {
        AjaxPro.onLoading = function(b) {
            if (b)
                jq('#studio_namingPeopleBox').block();
            else
                jq('#studio_namingPeopleBox').unblock();
        };

        if (NamingPeopleManager.MessageTimer != null)
            clearTimeout(NamingPeopleManager.MessageTimer);
            
        var namingContentManager = new NamingPeopleContentManager();
        namingContentManager.SaveSchema(NamingPeopleManager.SaveSchemaCallback);
    }
    this.SaveSchemaCallback = function(result) {
        if (result.Status == 1)
            jq('#studio_namingPeopleInfo').html('<div class="okBox">' + result.Message + '</div>')
        else
            jq('#studio_namingPeopleInfo').html('<div class="errorBox">' + result.Message + '</div>')

        NamingPeopleManager.MessageTimer = setTimeout(function() { jq('#studio_namingPeopleInfo').html('') }, 4000);
    }
};

jq(function() {
    var namingContentManager = new NamingPeopleContentManager();
    jq('#saveNamingPeopleBtn').click(NamingPeopleManager.SaveSchema);

   
});