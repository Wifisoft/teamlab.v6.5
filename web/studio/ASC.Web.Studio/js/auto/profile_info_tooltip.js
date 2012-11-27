PopupBoxManager = new function() {
    this.Collection = new Array();
    this.TimerCollection = new Array();

    this.RegistryPopupBox = function(popupBox) {
        this.Collection[popupBox.ID] = popupBox;
    };

    this.GetPopupBoxByID = function(id) {
        return this.Collection[id];
    };

    this.AjaxCallback = function(result) { };
};

var PopupBoxContainerElementID = 'container';

PopupBox = function(id, width, height, backgroundCSSClass, borderCSSClass, serverMethodFullName) {
    this.Height = height;
    this.Width = width;

    this.BorderCSSClass = borderCSSClass;
    this.BackgroundCSSClass = backgroundCSSClass;

    this.ID = id;
    this.ContentID = '_' + id;
    this.IsVisible = false;

    this.TimerHandler = null;
    this.CurrentElementID = '';
    this.PreviousElementID = '';

    this.VisibleWidth = -1;
    this.VisibleHeight = -1;

    this.ServerMethodFullName = serverMethodFullName;

    this.BeginBox = '<div class="' + this.BorderCSSClass + ' ' + this.BackgroundCSSClass + '" style="z-index:9999999; position:absolute; display:none; width:' + this.Whidth + 'px;" id=' + this.ID + '>';
    this.EndBox = '</div>';
    this.Content = '<div id=' + this.ContentID + '></div>';

    this.Init = function() {
        jq('#' + PopupBoxContainerElementID).append(this.BeginBox + this.Content + this.EndBox);
        setInterval("PopupBoxManager.GetPopupBoxByID('" + this.ID + "').CheckVisible()", 200);
    };


    PopupBoxManager.RegistryPopupBox(this);
    if (jq('#' + PopupBoxContainerElementID).length > 0)
        this.Init();
    else {
        var popupBoxID = this.ID;
        jq(document).ready(function() {
            PopupBoxManager.GetPopupBoxByID(popupBoxID).Init();
        })
    };

    this.SetContent = function(content) {
        jq('#' + this.ContentID).html(content);
        jq('#' + this.ID).css({
            'width': this.Width + 'px'
        });

    };

    this.CheckVisible = function(id) {
        if (jq('#' + this.ID).is(':visible') && this.IsVisible == false)
            jq("#" + this.ID).fadeOut("slow");
    };

    this.RegistryElement = function(idElement, methodParams) {
        var popupBoxID = this.ID;
        jq('#' + idElement).mouseover(function() {
            PopupBoxManager.GetPopupBoxByID(popupBoxID).ExecAndShow(idElement, methodParams);
        });

        jq('#' + idElement).mouseout(function() {
            PopupBoxManager.GetPopupBoxByID(popupBoxID).StopTimer();
        });

        jq('#' + idElement).click(function() {
            PopupBoxManager.GetPopupBoxByID(popupBoxID).StopTimer();
        });
    };

    this.RegistryElementOnLive = function(idElement, methodParams) {
        var popupBoxID = this.ID;
        jq('#' + idElement).live('mouseover', function() {
            PopupBoxManager.GetPopupBoxByID(popupBoxID).ExecAndShow(idElement, methodParams);
        });

        jq('#' + idElement).live('mouseout', function() {
            PopupBoxManager.GetPopupBoxByID(popupBoxID).StopTimer();
        });

        jq('#' + idElement).live('click', function() {
            PopupBoxManager.GetPopupBoxByID(popupBoxID).StopTimer();
        });
    };

    this.ExecAndShow = function(idElement, methodParams) {
        this.CurrentElementID = idElement;
        if (this.CurrentElementID == this.PreviousElementID && 1 == 2) {
            setTimeout("PopupBoxManager.GetPopupBoxByID('" + this.ID + "').Show('" + idElement + "')", 850);
            return;
        }
        else {
            if (this.TimerHandler)
                clearInterval(this.TimerHandler);

            this.PreviousElementID = this.CurrentElementID;
            this.TimerHandler = setTimeout("PopupBoxManager.GetPopupBoxByID('" + this.ID + "').ExecAjaxCode('" + idElement + "','" + methodParams + "')", 850);
        }
    };

    this.ExecAjaxCode = function(idElement, methodParams) {
        var popupBoxID = this.ID;
        AjaxPro.onLoading = function(b) {
            if (b) {
                PopupBoxManager.GetPopupBoxByID(popupBoxID).SetContent('<img src="' + SkinManager.GetImage('loader.gif') + '" alt="">');
                PopupBoxManager.GetPopupBoxByID(popupBoxID).Show(idElement);
            }
        };
        eval(this.ServerMethodFullName + "(" + methodParams + ",\"" + this.ID + "\",this.AjaxCallback);");
    };

    this.AjaxCallback = function(result) {
        AjaxPro.onLoading = function(b) { };
        var ppb = PopupBoxManager.GetPopupBoxByID(result.value.rs1);
        ppb.SetContent(result.value.rs2);
        ppb.SetPopupPosition(ppb.CurrentElementID);
    };

    this.SetPopupPosition = function(idElement) {
        var ppbHeight = jq('#' + this.ID).height();

        var w = jq(window);
        var TopPadding = w.scrollTop();
        var LeftPadding = w.scrollLeft();
        var ScrWidth = w.width();
        var ScrHeight = w.height();

        var idBox = this.ID;
        var pos = jq('#' + idElement).offset();
        var elHeight = jq('#' + idElement).height();

        var left = "";
        var top = "";

        if ((pos.left + this.Width) > (LeftPadding + ScrWidth))
            left = pos.left - this.Width + 'px';
        else
            left = pos.left + 'px';


        if ((pos.top + ppbHeight + elHeight) > (TopPadding + ScrHeight))
            top = pos.top - 5 - ppbHeight + 'px';
        else
            top = pos.top + 3 + elHeight + 'px'

        jq('#' + idBox).css({
            'left': left,
            'top': top
        });
    };

    this.Show = function(idElement) {
        this.SetPopupPosition(idElement);

        var idBox = this.ID;
        this.VisibleWidth = -1;
        this.VisibleHeight = -1;

        if (jq('#' + idBox).is(':visible') == false) {
            this.IsVisible = true;
            jq("#" + idBox).fadeIn("slow");

            jq('#' + idBox).mouseout(function() {
                PopupBoxManager.GetPopupBoxByID(idBox).SetVisible(false);
            });

            jq('#' + idBox).mouseover(function() {
                PopupBoxManager.GetPopupBoxByID(idBox).SetVisible(true);
            });
        }
    };

    this.SetVisible = function(state) {
        if (jq('#' + this.ID).is(':visible'))
            this.IsVisible = state;
    };

    this.StopTimer = function(id) {
        try {
            this.IsVisible = false;
            clearInterval(this.TimerHandler);
            this.TimerHandler = null;
        }
        catch (e) { };
    };
};

var StudioUserProfileInfo = new PopupBox('pb_StudioUserProfileInfo', 320, 140, 'tintMedium', 'borderBase', 'WebStudio.GetUserInfo');

jq(document).ready(function() {
    //register all
    if (StudioUserProfileInfo) {
        jq(".userLink").each(function() {
            var jqThis = jq(this);
            var id = jqThis.attr('id');
            if (id != null && id != '') {
                StudioUserProfileInfo.RegistryElement(id, "\"" + jqThis.attr('data-uid') + "\",\"" + jqThis.attr('data-pid') + "\"");
            }
        });
    }

});