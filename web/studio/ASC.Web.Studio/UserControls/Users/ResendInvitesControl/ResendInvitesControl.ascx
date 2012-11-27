<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ResendInvitesControl.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Users.ResendInvitesControl" %>
<%@ Import Namespace="ASC.Web.Studio.Core" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>

<style type="text/css">
    #inviteResender{display:none;}
    #inviteResender .buttonBox{margin-top:20px;}
    #resendCancelBtn{margin-left:10px;}
    #resendInvitesResult{display:none;}
</style>

<div id="inviteResender">
    <ascwc:Container ID="_invitesResenderContainer" runat="server">
        <Header><%= Resources.Resource.ResendInviteTitle %></Header>
        <Body>
        <div id="resendInvitesContent">
            <div>
                <%=Resources.Resource.ResendInvitesText%>
            </div>
            <div class="buttonBox clearFix">
                <a id="resendBtn" class="baseLinkButton" href="#"><%=Resources.Resource.ResendInvitesButton%></a>
                <a id="resendCancelBtn" class="grayLinkButton" href="#"><%=Resources.Resource.CancelButton%></a>
            </div>
        </div>
        <div id="resendInvitesResult">
            <div id="resendInvitesResultText"></div>
            <div class="buttonBox clearFix">
                <a id="resendInvitesCloseBtn" class="grayLinkButton" href="#"><%=Resources.Resource.CloseButton%></a>
            </div>
        </div>
        </Body>
    </ascwc:Container>
</div>

<script language="javascript" type="text/javascript">
    jq(function() {
        jq('#resendBtn').click(function() {
            InvitesResender.Resend();
        })

        jq('#resendCancelBtn, #resendInvitesCloseBtn').click(function() {
            InvitesResender.Hide();
        })
    });
    var InvitesResender = new function() {
        this.Resend = function() {
            AjaxPro.onLoading = function(b) {
                if (b)
                    jq('#inviteResender').block();
                else
                    jq('#inviteResender').unblock();
            };
            InviteResender.Resend(function(result) {
                var res = result.value;
                jq('#resendInvitesResult').show();
                jq('#resendInvitesContent').hide();

                jq('#resendInvitesResultText').html(res.message);
                if (res.status == 1)
                    jq('#resendInvitesResultText').attr('class', 'okBox');                
                else
                    jq('#resendInvitesResultText').attr('class', 'errorBox');

            })
        }

        this.Show = function() {
            jq('#resendInvitesResult').hide();
            jq('#resendInvitesContent').show();
            try {
                jq.blockUI({ message: jq("#inviteResender"),
                    css: {
                        opacity: '1',
                        border: 'none',
                        padding: '0px',
                        width: '300px',
                        height: '150px',
                        cursor: 'default',
                        textAlign: 'left',
                        'background-color': 'Transparent',
                        'margin-left': '-150px',
                        'top': '25%'
                    },

                    overlayCSS: {
                        backgroundColor: '#aaaaaa',
                        cursor: 'default',
                        opacity: '0.3'
                    },
                    focusInput: false,
                    fadeIn: 0,
                    fadeOut: 0
                });
            }
            catch (e) { };

        }
        this.Hide = function() {
            jq.unblockUI();
        }
    }
</script>