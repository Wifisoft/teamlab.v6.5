
jq(document).ready(function() {

    jq.dropdownToggle({
        switcherSelector: ".studio-top-panel .product-menu",
        dropdownID: "studio_productListPopupPanel",
        addTop: 2,
        addLeft: 0
    });
    
    jq.dropdownToggle({
        switcherSelector: ".studio-top-panel .staff-profile-box",
        dropdownID: "studio_myStaffPopupPanel",
        addTop: 2,
        addLeft: 12
    });

    if(jq('#studio_search').val() =='' && Searcher.ActiveHandler!=null) 
    {   
       jq('#studio_search').val(Searcher.ActiveHandler.SearchName);
       jq('#studio_search').attr('class','textEditDefaultSearch');
    }
    
    jq('#studio_search').keyup(function(event){ 
        var code;
        if (!e) var e = event;
        if (e.keyCode) code = e.keyCode;
        else if (e.which) code = e.which;    

        if(code==13)        
            Searcher.Search();
	    if(code==27) 
	        jq('#studio_search').val('');
    });
  
    
    jq('#studio_search').focus(function()
    { 
        if(Searcher.ActiveHandler!=null)
        {
            jq('#studio_search').attr('class','textEditMainSearch');
            if(jq('#studio_search').val()==Searcher.ActiveHandler.SearchName)
                jq('#studio_search').val('')            
        }
    });
       
   jq('#studio_search').blur(function(){ 
        if(jq('#studio_search').val()=='')
           setTimeout("if(Searcher.ActiveHandler!=null){ jq('#studio_search').attr('class','textEditDefaultSearch'); jq('#studio_search').val(Searcher.ActiveHandler.SearchName);}",200);            
   }); 
   
 
    
    jq('div[id^="studio_shItem_"]').hover(function () {
        jq(this).addClass('hoverTint');
      }, 
      function () {
        jq(this).removeClass('hoverTint');
      }
    );
    
});


var SearchHandler = function(id, searchName, logoURL, active, searchURL)
{
    this.ID = id;
    this.SearchName  = searchName;
    this.LogoURL = logoURL;
    this.Active = active;   
    this.SearchURL = searchURL; 
};

var Searcher = new function() {
	this.Handlers = new Array();
	this.ActiveHandler = null;

	this.AddHandler = function(searchHandler) {
		if (searchHandler.Active)
			this.ActiveHandler = searchHandler;

		this.Handlers.push(searchHandler);

	};

	this.ShowSearchPlaces = function() {
		if (jq('#studio_searchSwitchBox').is(':visible')) {
			this.HideSearchPlaces();
			return;
		}

		var pos = jq('#studio_searchSwitchButton').offset();
		var topMargin = jq('#studio_searchSwitchButton').outerHeight();

		jq('#studio_searchSwitchBox').css({
			'left': pos.left + "px",
			'top': pos.top + topMargin - 1 + 'px',
			'width': '156px',
			'display': 'block'
		});


		jq('body').click(function(event) {

			var elt = (event.target) ? event.target : event.srcElement;
			var isHide = true;
			if (jq(elt).is('[id="studio_searchSwitchBox"]') || jq(elt).is('[id="studio_searchSwitchButton"]'))
				isHide = false;

			if (isHide)
				jq(elt).parents().each(function() {
					if (jq(this).is('[id="studio_searchSwitchBox"]') || jq(this).is('[id="studio_searchSwitchButton"]')) {
						isHide = false
						return false;
					}
				});

			if (isHide) {
				PopupPanelHelper.HideAllPopupPanels();
			}
		});
	};

	this.GetHandlerByID = function(handlerID) {
		for (var i = 0; i < this.Handlers.length; i++) {
			if (this.Handlers[i].ID == handlerID)
				return this.Handlers[i];
		}

		return null;

	};

	this.HideSearchPlaces = function() {
		jq('#studio_searchSwitchBox').hide();
		jq("body").unbind("click");

	};

	this.SelectSearchPlace = function(handlerID) {
		var handler = this.GetHandlerByID(handlerID);
		if (handler != null) {
			jq('#studio_activeSearchHandlerLogo').attr('src', handler.LogoURL);

			jq('#studio_shItem_' + this.ActiveHandler.ID).attr('class', 'searchHandlerItem');

			if (jq('#studio_search').val() == this.ActiveHandler.SearchName)
				jq('#studio_search').val('');

			jq('#studio_shItem_' + handler.ID).attr('class', 'searchHandlerActiveItem');

			this.ActiveHandler = handler;
		}

		this.HideSearchPlaces();
		jq('#studio_search').focus();
	};

	this.Search = function() {
		var url = this.ActiveHandler.SearchURL; ;
		var text = encodeURIComponent(jq('#studio_search').val());

		if (jq('#studio_search').hasClass('textEditDefaultSearch'))
			text = '';

        if (text == "") return false;

		url = url.replace(/&search=[^&]*/g, '');
		url = url.replace(/\?search=[^&]*/g, '');

		if (url.indexOf('?') != -1)
			url += '&search=' + text;
		else
			url += '?search=' + text;

		window.open(url, '_self');
	};
};




function bindHidePopupPanelsOnBodyClick() {
	jq('body').click(function(event) {

		var elt = (event.target) ? event.target : event.srcElement;
		var el = jq(elt);
		var isHide = true;
		if (el.is('[class="myStaffActiveBox"]') || el.is('[class="myStaffPopupPanel"]') || el.is('[class="switchBox"]'))
			isHide = false;

		if (isHide)
			el.parents().each(function() {
				var curEl = jq(this);
				if (curEl.is('[class="myStaffActiveBox"]') || curEl.is('[class="myStaffPopupPanel"]') || curEl.is('[class="switchBox"]')) {
					isHide = false;
					return false;
				}
			});

		if (isHide) {
			//MyStaffHelper.HidePanel();
			//Searcher.HideSearchPlaces();
			PopupPanelHelper.HideAllPopupPanels();
		}
	});
}

PopupPanelHelper = new function() {

    this.HideAllPopupPanels = function () {
        jq('div .myStaffPopupPanel').filter(':visible').hide();
        jq('div .switchBox').filter(':visible').hide();
        jq('div .myStaffActiveBox').attr('class', 'myStaffBox');
        jq("body").unbind("click");
    };
}