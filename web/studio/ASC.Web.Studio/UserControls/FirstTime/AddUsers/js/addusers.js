
var AddUsersManager = new function() {
    this._mobile = null;
    this.RemoveItem = function(item) {
        jq(item).parent().parent().addClass('remove');
        if (jq('#userList').find(".userItem").not('.remove').length == 0)
            jq('#addUserBlock').removeClass('bordered');
    }

    this.ValidateEmail = function(email) {

        var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
        if (reg.test(email) == true)
            return true;
        return false;
    }

    this.AppendUser = function(parent, firstName, lasdtName, email) {
        //make with template
    parent.append('<tr class="userItem"><td class="logo"></td><td class="name"><div class="clearFix"><div class="firstname"><div class="fade"></div><div class="data">' + firstName + '</div></div><div class="lastname"><div class="fade"></div><div class="data">' + lasdtName + '</div></div></div></td><td class="email">' + email + '</td><td class="remove' + (this._mobile ? " mob" : "") + '"><div onclick="AddUsersManager.RemoveItem(this);"></div></td></tr>');
    }

    this.AddUser = function() {
        var parent = jq('#userList');

        var items = jq('#userList').find(".userItem").not('.remove');
        var firstName = jQuery.trim(jq('#firstName').val());
        var lastName = jQuery.trim(jq('#lastName').val());
        var address = jQuery.trim(jq('#email').val());

        if (!(this.isExists(items, address) || address == '' || firstName == '' || lastName == '' || !this.ValidateEmail(address))) {
            jq('#email').removeClass('incorrectEmailBox');
            jq('#firstName').removeClass('incorrectEmailBox');
            jq('#lastName').removeClass('incorrectEmailBox');
            this.AppendUser(parent, jq('#firstName').val(), jq('#lastName').val(), jq('#email').val());
            //parent.append('<tr class="userItem"><td class="logo"></td><td class="name"><div class="clearFix"><div class="firstname">' + jq('#firstName').val() + '</div><div class="lastname">' + jq('#lastName').val() + '</div></div></td><td class="email">' + jq('#email').val() + '</td><td class="remove"><div onclick="AddUsersManager.RemoveItem(this);"></div></td></tr>');
            jq('#firstName').val('');
            jq('#lastName').val('');
            jq('#email').val('');
        }
        else {
            if (firstName == '')
                jq('#firstName').addClass('incorrectEmailBox');
            else
                jq('#firstName').removeClass('incorrectEmailBox');

            if (lastName == '')
                jq('#lastName').addClass('incorrectEmailBox');
            else
                jq('#lastName').removeClass('incorrectEmailBox');

            if (address == '' || !this.ValidateEmail(address))
                jq('#email').addClass('incorrectEmailBox');
            else
                jq('#email').removeClass('incorrectEmailBox');
        }
    }

    this.isExists = function(items, email) {
        for (var index = 0; index < items.length; index++) {
            if (jQuery.trim(jq(items[index]).find('.email').html()) == email) {
                jq(items[index]).find('.email').addClass('incorrectValue');
                return true;
            }
            jq(items[index]).find('.email').removeClass('incorrectValue');
        }
        return false;
    }

    this.GetUsers = function() {

        var items = jq('#userList').find('.userItem').not('.remove');
        var arr = new Array();

        for (var i = 0; i < items.length; i++) {
            arr.push({ "FirstName": jQuery.trim(jq(items[i]).find('.name .firstname .data').html()), "LastName": jQuery.trim(jq(items[i]).find('.name .lastname .data').html()), "Email": jQuery.trim(jq(items[i]).find('.email').html()) });
        }
        return arr;
    }
}

var AddUsersManagerComponent = function() {

    this.SaveUsers = function(parentCallback) {
        var users = AddUsersManager.GetUsers();
        AddUsersController.SaveUsers(JSON.stringify(users), function(result) {

            //do somethin'

            if (parentCallback != null) {
                parentCallback(result.value);
            }
        });
    }
}