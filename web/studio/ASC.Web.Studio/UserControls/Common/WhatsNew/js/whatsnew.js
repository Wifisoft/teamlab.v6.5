var WhatsNew = new function() {
	this.MadeAnchor = function(modules, page) {
		var str = new Array();

		if (modules) str.push('mid' + modules + 'mid');
		str.push('daterange=' + jq('[id^="optionDateWhatsNew_"]:selected').val());
		str.push('type=' + jq('[id^="optionTypeWhatsNew_"]:selected').val())
		str.push('page=' + (page != undefined ? page : '1'));

		str.push('uid=' + (WhatsNew.GetUserOrDeptID()));

		ASC.Controls.AnchorController.move(str);
	}

	this.SetCheckbox = function(str) {
		var module = str.split(',');

		jq('[id^="whatsnew_modulefilter_"]').each(function() {
			jq(this).attr('checked', false);
		});

		for (var i = 0; i < module.length; i++)
			jq("#whatsnew_modulefilter_" + module[i]).attr('checked', true);
	}

	this.SetPage = function(page) {
		WhatsNew.CheckboxToAnchor(page);
	}

	this.SetDateData = function() {
		var index = jq('[id^="optionDateWhatsNew_"]:selected').val();
		var currentTime = new Date();

		var from_date = jq.datepick.today();
		var to_date = jq.datepick.today();

		switch (parseInt(index)) {
			case 0: //'today':
				to_date = jq.datepick.formatDate(currentTime);
				from_date = jq.datepick.formatDate(currentTime);
				break;

			case 1: //'yesterday':
				currentTime.setDate(currentTime.getDate() - 1);
				to_date = jq.datepick.formatDate(currentTime);
				from_date = jq.datepick.formatDate(currentTime);
				break;

			case 2: //'lastweek':
				to_date = jq.datepick.formatDate(currentTime);
				currentTime.setDate(currentTime.getDate() - 6);
				from_date = jq.datepick.formatDate(currentTime);
				break;

			case 3: //'lastmonth':            
				to_date = jq.datepick.formatDate(currentTime);
				currentTime.setMonth(currentTime.getMonth() - 1);
				from_date = jq.datepick.formatDate(currentTime);
				break;

			case 4: //'lastyear':
				to_date = jq.datepick.formatDate(currentTime);
				currentTime.setFullYear(currentTime.getFullYear() - 1);
				from_date = jq.datepick.formatDate(currentTime);
				break;

			default:
				from_date = jq("#txtFromDateWhatsNew").val();
				from_date = from_date.length == 0 ? jq.datepick.formatDate(jq.datepick.today()) : from_date;
				to_date = jq("#txtToDateWhatsNew").val();
				to_date = to_date.length == 0 ? jq.datepick.formatDate(jq.datepick.today()) : to_date;
				break;
		}

		jq('#txtFromDateWhatsNew').datepick('option', 'maxDate', to_date);
		jq('#txtToDateWhatsNew').datepick('option', 'minDate', from_date);
		jq('#txtToDateWhatsNew').datepick('option', 'maxDate', new Date());

		jq("#txtFromDateWhatsNew").val(from_date)
		jq("#txtToDateWhatsNew").val(to_date)

		jq("#optionDateWhatsNew_" + index).attr('selected', 'selected');
	}

	this.SelectDateOption = function() {
		WhatsNew.CheckboxToAnchor();
	}

	this.SelectDate = function(dates) {
		if (this.id == 'txtFromDateWhatsNew')
			jq('#txtToDateWhatsNew').datepick('option', 'minDate', dates[0] || null);
		else
			jq('#txtFromDateWhatsNew').datepick('option', 'maxDate', dates[0] || new Date());

		jq('#optionDateWhatsNew_5').attr('selected', 'selected');
		WhatsNew.CheckboxToAnchor();
	}

	this.SelectTypeOption = function() {
		WhatsNew.CheckboxToAnchor(1);
	}

	this.SelectGroupOption = function() {
		jq('#Users').val('-1')
		WhatsNew.CheckboxToAnchor(1);
	}

	this.SelectUserOption = function() {
		WhatsNew.CheckboxToAnchor(1);
	}

	this.CheckboxToAnchor = function(page) {
		var ModuleId = new Array();

		var action = (jq('[id^="whatsnew_modulefilter_"]').length == jq('[id^="whatsnew_modulefilter_"]:checked').length)

		jq('[id^="whatsnew_modulefilter_"]:checked').each(function() {
			var id = jq(this).attr('id').split('whatsnew_modulefilter_')[1];
			ModuleId.push(id);
		});

		WhatsNew.MadeAnchor(action ? null : ModuleId, page);
	}

	this.AnchorToCheckbox = function(str) {
		WhatsNew.SetCheckbox(str);

		WhatsNew.ShowData(1);
	}	

	this.AnchorProcess = function(modules, dateRange, type, page, uid) {
		jq('#optionDateWhatsNew_' + dateRange).attr('selected', 'selected');
		jq('#optionTypeWhatsNew_' + type).attr('selected', 'selected');

		WhatsNew.SetUserOrDeptID(uid);

		WhatsNew.SetCheckbox(modules);

		WhatsNew.ShowData(page);
	}

	this.AnchorToDate = function(dateRange, type, page, uid) {
		jq('#optionDateWhatsNew_' + dateRange).attr('selected', 'selected');
		jq('#optionTypeWhatsNew_' + type).attr('selected', 'selected');

		WhatsNew.SetUserOrDeptID(uid);

		jq('[id^="whatsnew_modulefilter_"]').each(function() {
			jq(this).attr('checked', true);
		});

		WhatsNew.ShowData(page);
	}
	
	this.ExistsCheckboxValue = function(chkID, val){
		if (jq('#{0} option[value="{1}"]'.format(chkID, val)).size() > 0) {
			return true;
		}
		return false;
	}

	this.SetUserOrDeptID = function(uid) {
		jq('#Users').val(-1);
		jq('#Departments').val(-1);
		if (WhatsNew.ExistsCheckboxValue('Users', uid)) {
			jq('#Users').val(uid);
			return;
		}
		if (WhatsNew.ExistsCheckboxValue('Departments', uid)) {
			jq('#Departments').val(uid);
		}
	}

	this.GetUserOrDeptID = function() {
		var userID = jq('#Users').val();
		if (userID == '-1' || userID == null || !userID) {
			userID = jq('#Departments').val();
		}
		if (userID == '-1' || userID == null || !userID) {
			userID = null;
		}
		return userID;
	}

	this.InitAnchor = function() {
		ASC.Controls.AnchorController.bind(/^mid(\S*)mid,daterange=(\d),type=(\d),page=(\d+),uid=(\S*)/, this.AnchorProcess);
		ASC.Controls.AnchorController.bind(/^daterange=(\d),type=(\d),page=(\d+),uid=(\S*)/, this.AnchorToDate);
		ASC.Controls.AnchorController.bind(/^mid(\S*)mid$/, this.AnchorToCheckbox);
		ASC.Controls.AnchorController.bind(null, this.ShowData);
	}

	this.ShowData = function(page) {
		var ModuleId = new Array();
		jq('[id^="whatsnew_modulefilter_"]:checked').each(function() {
			var id = jq(this).attr('id').split('whatsnew_modulefilter_')[1];
			ModuleId.push(id);
		});

		if (ModuleId.length == 0) {
			jq("#divWhatNewContainer").empty();
			jq("#divForNavigation").hide();
			return;
		}

		WhatsNew.SetDateData();
		var from_date = jq("#txtFromDateWhatsNew").val();
		var to_date = jq("#txtToDateWhatsNew").val();
		var ProductId = jq("#hiddenProductId").val()
		var type = parseInt(jq('[id^="optionTypeWhatsNew_"]:selected').val());

		if (!page)
			page = 1;

		var userID = WhatsNew.GetUserOrDeptID();

		AjaxPro.onLoading = function(b) {
			if (b)
				jq(".whatsNewFilter").block();
			else
				jq(".whatsNewFilter").unblock();
		}
		AjaxPro.WhatsNewBody.ShowRecentActivity(ProductId, ModuleId, from_date, to_date, type, page, userID,
			function(_res) {
				if (_res.error != null) {
					alert(_res.error.Message);
					return;
				}
				var res = _res.value;

				var mydata = {
					dateKey: "",
					table: eval(res.rs1)
				};

				jq("#divWhatNewContainer").setTemplateElement("jTemplateTableWhatsNew", null, { filter_data: false });
				jq("#divWhatNewContainer").processTemplate(mydata);

				var visiblePageCount = parseInt(res.rs10);
				var amountPage = parseInt(res.rs11);
				var currentPageNumber = parseInt(res.rs12);

				jq("#divForNavigation").hide();
				if (amountPage == 0)
					return;
				jq("#divForNavigation").show();

				var startPage = currentPageNumber - (visiblePageCount / 2).toFixed(0);
				if (startPage + visiblePageCount > amountPage)
					startPage = amountPage - visiblePageCount;
				if (startPage < 0)
					startPage = 0;

				var endPage = startPage + visiblePageCount;
				if (endPage > amountPage)
					endPage = amountPage;

				var sb = new String("<div class='pagerNavigationLinkBox'>");

				if (currentPageNumber > 1)
					sb += "<a class='pagerPrevButtonCSSClass' href='#' onclick='WhatsNew.SetPage(" + (currentPageNumber - 1) + ");return false;'>" + res.rs13 + "</a>";

				for (var i = startPage; i < endPage && endPage - startPage > 1; i++) {
					if (i == startPage && i != 0) {
						sb += "<a class='pagerNavigationLinkCSSClass' href='#'onclick='WhatsNew.SetPage(1);return false;'>1</a>";
						if (i != 1)
							sb += "<span class='splitter'>...</span>";
					}
					if ((currentPageNumber - 1) == i) {
						sb += "<span class='pagerCurrentPosition'>" + currentPageNumber + "</span>";
					}
					else {
						sb += "<a class='pagerNavigationLinkCSSClass' href='#' onclick='WhatsNew.SetPage(" + (i + 1) + ");return false;'>" + (i + 1) + "</a>";
					}
					if (i == endPage - 1 && i != amountPage - 1) {
						if (i != amountPage - 2)
							sb += "<span class='splitter'>...</span>";
						sb += "<a class='pagerNavigationLinkCSSClass' href='#'onclick='WhatsNew.SetPage(" + amountPage + ");return false;'>" + amountPage + "</a>";
					}
				}
				if (currentPageNumber != amountPage && amountPage != 1) {
					sb += "<a class='pagerNextButtonCSSClass' href='#' onclick='WhatsNew.SetPage(" + (currentPageNumber + 1) + ");return false;'>" + res.rs14 + "</a>";
				}
				sb += "</div>";
				jq('#divForNavigation').html(sb);
			});
	}

}

WhatsNew.InitAnchor();

jq(function()
	{
		jq('#txtFromDateWhatsNew,#txtToDateWhatsNew').val(jq.datepick.formatDate(new Date()));
		jq('#txtFromDateWhatsNew,#txtToDateWhatsNew').datepick(
				{
					onSelect: WhatsNew.SelectDate,
					maxDate: new Date(),
//					defaultDate: new Date(),
					selectDefaultDate: true,
					showAnim: ''
				});	
		jq('#txtToDateWhatsNew').datepick('option', 'minDate', new Date());
		jq('#txtFromDateWhatsNew,#txtToDateWhatsNew').datepick('option', 'maxDate', new Date()); 
	});