if (typeof ASC === "undefined")
    ASC = {};

if (typeof ASC.Settings === "undefined")
    ASC.Settings = (function() { return {}; })();

ASC.Settings.AccessRights = new function() {
    
    var timeoutHandler = null;
    
    var cancelBubble = function(e) {
        if (!e) e = window.event;
        e.cancelBubble = true;
        if (e.stopPropagation) e.stopPropagation();
    };

    var blockUI = function(obj, width, height, top) {
        try {
            width = parseInt(width || 0);
            height = parseInt(height || 0);
            left = parseInt(-width / 2);
            top = parseInt(top || -height / 2);
            jq.blockUI({ message: jq(obj),
                css: {
                    left: '50%',
                    top: '50%',
                    opacity: '1',
                    border: 'none',
                    padding: '0px',
                    width: width > 0 ? width + 'px' : 'auto',
                    height: height > 0 ? height + 'px' : 'auto',
                    cursor: 'default',
                    textAlign: 'left',
                    position: 'fixed',
                    'margin-left': left + 'px',
                    'margin-top': top + 'px',
                    'background-color': 'Transparent'
                },

                overlayCSS: {
                    backgroundColor: '#aaaaaa',
                    cursor: 'default',
                    opacity: '0.3'
                },

                focusInput: true,
                baseZ: 666,

                fadeIn: 0,
                fadeOut: 0,
                onBlock: function() {
                    var $blockUI = jq(obj).parents('div.blockUI:first'), blockUI = $blockUI.removeClass('blockMsg').addClass('blockDialog').get(0), cssText = '';
                    if (jq.browser.msie && jq.browser.version < 9 && $blockUI.length !== 0) {
                        var prefix = ' ', cssText = prefix + blockUI.style.cssText, startPos = cssText.toLowerCase().indexOf(prefix + 'filter:'), endPos = cssText.indexOf(';', startPos);
                        if (startPos !== -1) {
                            if (endPos !== -1) {
                                blockUI.style.cssText = [cssText.substring(prefix.length, startPos), cssText.substring(endPos + 1)].join('');
                            } else {
                                blockUI.style.cssText = cssText.substring(prefix.length, startPos);
                            }
                        }
                    }
                }
            });
        }
        catch (e) { };
    };

	return {

		init: function() {
			jq("#changeOwnerBtn").click(ASC.Settings.AccessRights.changeOwner);
			jq("#adminTable tbody tr").remove();
            jq("#adminTmpl").tmpl(window.adminList).prependTo("#adminTable tbody");
			
			for(var i=0; i<window.adminList.length; i++)
				window.adminSelector.HideUser(window.adminList[i].id);
            
            jq("div.accessRights-switcher").click(function(){
                jq(this).next().slideToggle("slow");
                if(jq(this).find("div").hasClass("accessRights-collapseDown"))
                    jq(this).find("div").removeClass("accessRights-collapseDown").toggleClass("accessRights-collapseRight");
                else
                    jq(this).find("div").removeClass("accessRights-collapseRight").toggleClass("accessRights-collapseDown");    
            });
		},
        
		changeOwner: function() {
            var ownerId = window.ownerSelector.SelectedUserId;
            
            if (ownerId == null) return false;
            
            if (timeoutHandler)
                clearTimeout(timeoutHandler);

            window.AjaxPro.onLoading = function(b) {
                if (b)
                    jq("#ownerSelectorContent").block();
                else
                    jq("#ownerSelectorContent").unblock();
            };

            window.AccessRightsController.ChangeOwner(ownerId, function(result) {
                var res = result.value;
                if (res.Status == 1)
                    jq("#accessRightsInfo").html("<div class='okBox'>" + res.Message + "</div>");
                else
                    jq("#accessRightsInfo").html("<div class='errorBox'>" + res.Message + "</div>");

                timeoutHandler = setTimeout(function() { jq("#accessRightsInfo").html(""); }, 4000);
            });
			return false;
		},
        
        addAdmin: function(uId)
        {
            window.AjaxPro.onLoading = function(b) {
                if (b)
                    jq("#adminContent").block();
                else
                    jq("#adminContent").unblock();
            };
            
            window.AccessRightsController.AddAdmin(uId, function(res) {
                if (res.error != null) {
                	alert(res.error.Message);
                	return false;
                }

            	jq("#adminTmpl").tmpl(res.value).appendTo("#adminTable tbody");
            	ASC.Settings.AccessRights.Selectors.full.hideUser(uId, true);
            	window.adminSelector.HideUser(uId);
            });
        },
		
		manageProduct: function(e, itemId, itemName, subjects) {
    	    if (jq("#managePanel_"+itemName).length == 0) return false;

			var enableProduct = jq("#cbxEnableProduct_" + itemId).attr("default") == "true";
    		jq("#managePanel_"+itemName).find("input[type=checkbox]").each(function(index) {
    			if(!enableProduct) {
    			    jq(this).attr("checked", false);
    				if(index>0)
    					jq(this).attr("disabled", true);
    			} else {
    				var isChecked = jq(this).attr("default") == "true";
    				jq(this).attr("checked", isChecked);
    				if(jq(this).attr("id"))
    					jq(this).attr("disabled", false);
    				else
    					jq(this).attr("disabled", true);
    			}
		    });
			
    		blockUI("#managePanel_"+itemName, 550, 500, 0);
    	    jq("#managePanel_"+itemName+" div.action-block a:first").unbind("click").bind("click", function() {
    		    ASC.Settings.AccessRights.saveProductSettings(itemId, itemName, subjects);
    	    });
    	    cancelBubble(e);
        },
    		
	    saveProductSettings: function(itemId, itemName, subjects) {
		    var header = jq("#selectorContent_"+itemName).parents("div.accessRights-content").prev();
		    var enableProduct = jq("#managePanel_"+itemName+" input[id^=cbxEnableProduct_]").is(":checked");
		    var params = { };
		    var data = {
			    id: itemId,
			    enabled: enableProduct
		    };
	    	
		    if (!enableProduct) {
			    Teamlab.setWebItemSecurity(params, data, {
				    success: function() {
					    jq(header).find("img:first").hide();
					    jq(header).find("img:last").show();
					    jq(header).next().find("div:first").hide();
					    jq(header).next().find("div:last").show();
					    jq(header).addClass("accessRights-disabledText");
				    	
				    	jq("#managePanel_"+itemName).find("input[type=checkbox]").each(function() {
				    		var isChecked = jq(this).is(":checked");
				            jq(this).attr("default", isChecked ? "true" : "false");
		                });
				    },
				    before: function(params) {
				    	jq("#managePanel_" + itemName + " div.action-block").hide();
				    	jq("#managePanel_" + itemName + " div.info-block").show();
				    },
				    after: function(params) {
				    	jq("#managePanel_" + itemName + " div.info-block").hide();
				    	jq("#managePanel_" + itemName + " div.action-block").show();
				    	jq.unblockUI();
				    }
			    });
		    } else {
			    if (jq("#fromList_"+itemId).is(":checked") && subjects.length > 0)
				    data.subjects = subjects;
			    Teamlab.setWebItemSecurity(params, data, {
				    success: function() {
					    jq(header).find("img:first").show();
					    jq(header).find("img:last").hide();
					    jq(header).next().find("div:first").show();
					    jq(header).next().find("div:last").hide();
					    jq(header).removeClass("accessRights-disabledText");
					    
					    jq("#managePanel_"+itemName).find("input[type=checkbox]").each(function() {
				    		var isChecked = jq(this).is(":checked");
				            jq(this).attr("default", isChecked ? "true" : "false");
		                });

				    	var count = jq("#managePanel_" + itemName + " input[id^=cbxEnableModule_]").length;
				    	
					    if(count>0) {
					    	jq("#managePanel_" + itemName + " input[id^=cbxEnableModule_]").each(function(index) {
					    		var d = {
					    			id: jq(this).attr("id").split("_")[1],
					    			enabled: jq(this).is(":checked")
					    		};
					    		Teamlab.setWebItemSecurity(params, d, {
					    			after: function(params) {
					    				if (index == count - 1) {
					    					jq("#managePanel_" + itemName + " div.info-block").hide();
					    					jq("#managePanel_" + itemName + " div.action-block").show();
					    					jq.unblockUI();
					    				}
					    			}
					    		});
					    	});
					    }
				    },
				    before: function(params) {
				    	jq("#managePanel_" + itemName + " div.action-block").hide();
				    	jq("#managePanel_" + itemName + " div.info-block").show();
				    }
			    });
		    }
	    },
		
		selectModules: function(obj) {
			var enableProduct = jq(obj).is(":checked");
			jq(obj).parents("table").find("input[type=checkbox]").each(function(index) {
				jq(this).attr("checked", enableProduct);
				if(!enableProduct) {
    				if(index>0)
    					jq(this).attr("disabled", true);
    			} else {
    				if(jq(this).attr("id"))
    					jq(this).attr("disabled", false);
    				else
    					jq(this).attr("disabled", true);

				}
		    });
		},
		
        accessSwitch: function(e, switcher, id, onText, offText, subjects)
        {
        	var parent = jq(switcher).parent();
        	var disabled = jq(parent).hasClass("accessRights-disabledText");
        	var params = {};
        	var data = {
        		id: id,
        		enabled: disabled
        	};

		    if(disabled && jq("#fromList_"+id).is(":checked") && subjects.length>0)
	            data.subjects = subjects;
        	
        	Teamlab.setWebItemSecurity(params, data, { success: function() {
        		
        		if(!disabled) {
        			jq(switcher).text(onText);
        			jq(parent).find("img:first").hide();
        			jq(parent).find("img:last").show();
        			jq(parent).next().find("div:first").hide();
        			jq(parent).next().find("div:last").show();
        			jq(parent).addClass("accessRights-disabledText");
        		} else {
        			jq(switcher).text(offText);
        			jq(parent).find("img:first").show();
        			jq(parent).find("img:last").hide();
        			jq(parent).next().find("div:first").show();
        			jq(parent).next().find("div:last").hide();
        			jq(parent).removeClass("accessRights-disabledText");
        		}
        	}
        	});
        	cancelBubble(e);
        },
		
	    changeAccessType: function(obj, itemName, subjects) {

	    	var type = jq(obj).attr("id").split("_")[0];
	    	var id = jq(obj).attr("id").split("_")[1];
	    	var params = { };
	    	var data = {
	    		id: id,
	    		enabled: true
	    	};

	    	if (type == "all") {
	    		Teamlab.setWebItemSecurity(params, data, {
	    			success: function() {
	    				jq("#selectorContent_" + itemName).hide();
	    			}
	    		});
	    	} else {
	    	    if (subjects.length == 0)
	    	        data.enabled = false; 
	    	    if (subjects.length > 0)
	    			data.subjects = subjects;
	    		Teamlab.setWebItemSecurity(params, data, {
	    			success: function() {
	    				jq("#selectorContent_" + itemName).show();
	    			}
	    		});
	    	}
	    },
		
		selectedItem_mouseOver: function (obj)
        {
	        jq(obj).find("img:first").hide();
	        jq(obj).find("img:last").show();
        },
    	
        selectedItem_mouseOut: function (obj)
        {
	        jq(obj).find("img:first").show();
	        jq(obj).find("img:last").hide();
        }
	};
};

if (typeof ASC.Settings.AccessRights.Selectors === "undefined")
    ASC.Settings.AccessRights.Selectors = (function() { return {}; })();
