jq(function() {
jq('#accountLinks').delegate('.popup','click',function() {
                var obj = jq(this);
                if (obj.hasClass('linked'))  {
                    //unlink
                    var res = AccountLinkControl.UnlinkAccount(obj.attr('id'));
                    jq('#accountLinks').html(res.value.rs1);
                }
                else {
                    var link = obj.attr('href');
                    window.open(link, 'login', 'width=800,height=500,status=no,toolbar=no,menubar=no,resizable=yes,scrollbars=no');
                }
                return false;
            });
        });

function loginCallback(profile) {
    var res = AccountLinkControl.LinkAccount(profile.Serialized);
    jq('#accountLinks').html(res.value.rs1);
}

function authCallback(profile) {
    __doPostBack('signInLogin', profile.HashId);
}