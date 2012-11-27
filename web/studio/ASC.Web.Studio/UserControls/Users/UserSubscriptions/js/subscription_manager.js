jq(function() {
    AjaxPro.onLoading = function(b) {
        if (b)
            jq.blockUI();
        else
            jq.unblockUI();
    };

    var timeoutPeriod = AjaxPro.timeoutPeriod;
    AjaxPro.timeoutPeriod = 5 * 60 * 1000;
    SubscriptionManager.GetAllSubscriptions(function(result) {
        jq('#studio_notifySenders').html(jq("#itemSubscriptionsTemplate").tmpl(result.value));
        CommonSubscriptionManager.InitNotifyByComboboxes();
        AjaxPro.timeoutPeriod = timeoutPeriod;
    });
});

var CommonSubscriptionManager = new function() {
    this.ConfirmMessage = 'Are you sure?';

    this.SubscribeToWhatsNew = function() {
        AjaxPro.onLoading = function(b) {
            if (b)
                jq.blockUI();
            else
                jq.unblockUI();
        };
        SubscriptionManager.SubscribeToWhatsNew(function(result) {
            var res = result.value;
            if (res.rs1 == '1')
                jq('#studio_newSubscriptionButton').html(res.rs2);

            else
                jq('#studio_newSubscriptionButton').html('<div class="errorBox">' + res.rs2 + '</div>');
        });
    };

    this.SubscribeToAdminNotify = function() {
        AjaxPro.onLoading = function(b) {
            if (b)
                jq.blockUI();
            else
                jq.unblockUI();
        };
        SubscriptionManager.SubscribeToAdminNotify(function(result) {
            var res = result.value;
            if (res.rs1 == '1')
                jq('#studio_adminSubscriptionButton').html(res.rs2);

            else
                jq('#studio_adminSubscriptionButton').html('<div class="errorBox">' + res.rs2 + '</div>');
        });
    };

    var UpdateProductSubscriptionCallback = function(result) {
        var res = result.value;
        if (res.Status == 1) {
            jq('#subscriptionProductItem_' + res.Data.Id).replaceWith(jq('#itemSubscriptionsTemplate').tmpl({ Items: [res.Data] }));
            CommonSubscriptionManager.InitNotifyByComboboxes();
        }
        else
            jq('#subscriptionProductItem_' + res.ItemId).html('<div class="errorBox">' + res.Message + '</div>');
    };

    this.UnsubscribeProduct = function(productID) {
        if (!confirm(this.ConfirmMessage))
            return;

        AjaxPro.onLoading = function(b) {
            if (b)
                jq.blockUI();
            else
                jq.unblockUI();
        };
        SubscriptionManager.UnsubscribeProduct(productID, UpdateProductSubscriptionCallback);
    };

    this.UnsubscribeType = function(productID, moduleID, subscribeType) {
        if (!confirm(this.ConfirmMessage))
            return;

        AjaxPro.onLoading = function(b) {
            if (b)
                jq('#subscriptionProductItem_' + productID).block();
            else
                jq('#subscriptionProductItem_' + productID).unblock();
        };

        SubscriptionManager.UnsubscribeType(productID, moduleID, subscribeType, UpdateProductSubscriptionCallback);
    };

    this.SubscribeType = function(productID, moduleID, subscribeType) {

        AjaxPro.onLoading = function(b) {
            if (b)
                jq('#subscriptionProductItem_' + productID).block();
            else
                jq('#subscriptionProductItem_' + productID).unblock();
        };

        SubscriptionManager.SubscribeType(productID, moduleID, subscribeType, UpdateProductSubscriptionCallback);
    };

    this.UnsubscribeObjects = function(productID, moduleID, subscribeType) {
        AjaxPro.onLoading = function(b) {
            if (b)
                jq('#studio_subscribeType_' + productID + '_' + moduleID + '_' + subscribeType).block();
            else
                jq('#studio_subscribeType_' + productID + '_' + moduleID + '_' + subscribeType).unblock();
        };

        var items = new Array();
        jq('input[id^="studio_subscribeItemChecker_' + productID + '_' + moduleID + '_' + subscribeType + '_"]:checked').each(function(i, n) {
            items.push(jq(this).val());
        });

        SubscriptionManager.UnsubscribeObjects(productID, moduleID, subscribeType, items, function(result) {
            var res = result.value;
            var productID = res.rs2;
            var moduleID = res.rs3;
            var typeID = res.rs4;
            var items = res.rs5.split(',');
            if (res.rs1 == '1') {
                for (var i = 0; i < items.length; i++)
                    jq('#studio_subscribeItem_' + productID + '_' + moduleID + '_' + typeID + '_' + items[i]).remove();


                if (jq('div[id^="studio_subscribeItem_' + productID + '_' + moduleID + '_' + typeID + '_"]').length == 0)
                    jq('#studio_subscribeType_' + productID + '_' + moduleID + '_' + typeID).remove();
            }
            else
                jq('#studio_subscribeType_' + productID + '_' + moduleID + '_' + typeID).html(res.rs6);

        });
    };

    this.ToggleProductList = function(productID) {
        if (jq('#studio_product_subscriptions_' + productID).is(':visible')) {
            jq('#studio_subscribeProductState_' + productID).attr('src', SkinManager.GetImage('collapse_right_dark.png'));
            jq('#studio_product_subscriptions_' + productID).hide();
        }
        else {
            jq('#studio_subscribeProductState_' + productID).attr('src', SkinManager.GetImage('collapse_down_dark.png'));
            jq('#studio_product_subscriptions_' + productID).show();
        }
    };

    this.ToggleModuleList = function(productID, moduleID) {
        if (jq('#studio_module_subscriptions_' + productID + '_' + moduleID).is(':visible')) {
            jq('#studio_subscribeModuleState_' + productID + '_' + moduleID).attr('src', SkinManager.GetImage('collapse_right_dark.png'));
            jq('#studio_module_subscriptions_' + productID + '_' + moduleID).hide();
        }
        else {
            jq('#studio_subscribeModuleState_' + productID + '_' + moduleID).attr('src', SkinManager.GetImage('collapse_down_dark.png'));
            jq('#studio_module_subscriptions_' + productID + '_' + moduleID).show();
        }
    };

    this.ToggleSubscriptionList = function(productID, moduleID, typeID) {
        var subscriptionState = jq('#studio_subscriptionsState_' + productID + '_' + moduleID + '_' + typeID);

        var subscriptionElementID = 'studio_subscriptions_' + productID + '_' + moduleID + '_' + typeID;
        var subscriptionElement = jq('#' + subscriptionElementID);

        if (subscriptionElement == null || subscriptionElement.attr('id') == null) {
            subscriptionState.attr('src', SkinManager.GetImage('mini_loader.gif'));

            AjaxPro.onLoading = function(b) {
                if (b)
                    jq('#studio_subscribeType_' + productID + '_' + moduleID + '_' + typeID).block();
                else
                    jq('#studio_subscribeType_' + productID + '_' + moduleID + '_' + typeID).unblock();
            };

            SubscriptionManager.RenderGroupItemSubscriptions(productID, moduleID, typeID, function(res) {
                var el = jq('#studio_subscribeType_' + productID + '_' + moduleID + '_' + typeID);

                var resultHTML = '';
                if (res.value.Status = 1)
                    resultHTML = jq('<div></div>').html(jq("#subscribtionObjectsTemplate").tmpl(res.value)).html();

                if (resultHTML == null || '' == resultHTML) {
                    resultHTML = "<div id='" + subscriptionElementID + "' style='height: 0px;'>&nbsp</div>";
                }
                el.html(el.html() + resultHTML);
            });
        }

        if (subscriptionElement.is(':visible')) {
            subscriptionState.attr('src', SkinManager.GetImage('collapse_right_light.png'));
            subscriptionElement.hide();
        }
        else {
            subscriptionState.attr('src', SkinManager.GetImage('collapse_down_light.png'));
            subscriptionElement.show();
        }
    };

    this.InitNotifyByComboboxes = function() {
        jq('select[id^="NotifyByCombobox_"]').each(
            function() {
                jq(this).tlcombobox();
            }
		);
    };


    this.SetNotifyByMethod = function(productID, notifyBy) {

        AjaxPro.onLoading = function(b) {
            if (b)
                jq.blockUI();
            else
                jq.unblockUI();
        };
        SubscriptionManager.SetNotifyByMethod(productID, notifyBy, function(result) { });
    };

    this.SetWhatsNewNotifyByMethod = function(notifyBy) {

        AjaxPro.onLoading = function(b) {
            if (b)
                jq.blockUI();
            else
                jq.unblockUI();
        };
        SubscriptionManager.SetWhatsNewNotifyByMethod(notifyBy, function(result) { });
    };
    this.SetAdminNotifyNotifyByMethod = function(notifyBy) {

        AjaxPro.onLoading = function(b) {
            if (b)
                jq.blockUI();
            else
                jq.unblockUI();
        };
        SubscriptionManager.SetAdminNotifyNotifyByMethod(notifyBy, function(result) { });
    };
};