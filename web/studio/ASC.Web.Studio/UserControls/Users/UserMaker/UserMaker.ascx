<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserMaker.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Users.UserMaker" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Import Namespace="ASC.Data.Storage" %>
<div id="studio_userMakerDialog" style="display: none;">
    <ascwc:Container runat="server" ID="UserMakerContainer">
        <Header>
            <div id="studio_userMakerDialogTitle">
            </div>
        </Header>
        <Body>
            <asp:HiddenField runat="server" ID="_jqueryDateMask" />
            <div id="studio_um_dialogContent">
                <div class="clearFix">
                    <div style="float: left; width: 240px;">
                        <div id="studio_um_photo">
                        </div>
                        <%if (!ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context))
                          { %>                        
                        <div align="justify" class="textMediumDescribe" style="padding: 5px 0px;">
                            <%=ASC.Web.Studio.Core.FileSizeComment.GetFileImageSizeNote(Resources.Resource.UserPhotoLimitationMessage, false)%>
                        </div>
                        <div id="studio_um_uploadMessage">
                        </div>
                        <div class="clearFix">
                            <a class="baseLinkButton" id="studio_usrPhotoUploader" style="float: left;" href="javascript:StudioUserMaker.ShowFileDialog();">
                                <%=Resources.Resource.ChangeButton%></a>
                        </div>
                        <input id="studio_um_photoPath" type="hidden" value="" />
                        <% } %>
                    </div>
                    <div style="float: left; margin-left: 15px; padding-top: 10px; width: 350px;">
                        <div id="idTabsContainer" class="tabsContainer" runat="server">
                        </div>
                        <div id="studio_um_message">
                        </div>
                        <div class="propertiesContainer">
                            <div id="idMajorProperties" class="properties majorProperties">
                                <%--FirstName--%>
                                <div class="clearFix requiredField">
                                    <span class="requiredErrorText"><%=Resources.Resource.ErrorEmptyUserFirstName%></span>
                                    <div class="headerPanel">
                                        <%=Resources.Resource.FirstName%>
                                    </div>
                                    <input type="text" autocomplete="off" id="studio_umInfo_FirstName" class="textEdit" maxlength="60" />
                                </div>
                                <%--LastName--%>
                                <div class="clearFix requiredField">
                                    <span class="requiredErrorText"><%=Resources.Resource.ErrorEmptyUserLastName%></span>
                                    <div class="headerPanel">
                                             <%=Resources.Resource.LastName%>
                                    </div>
                                    <input type="text" autocomplete="off" id="studio_umInfo_LastName" class="textEdit" maxlength="60" />
                                </div>
                               
                                <%--Email--%>
                                <div id="email_property" class="clearFix requiredField">
                                    <span class="requiredErrorText"><%=Resources.Resource.ErrorNotCorrectEmail %></span>
                                    <div class="headerPanel">
                                            <%=Resources.Resource.Email%>
                                    </div>
                                    <input type="text" autocomplete="off" id="studio_umInfo_Email" class="textEdit" <%=ASC.Core.SecurityContext.CheckPermissions(ASC.Core.Users.Constants.Action_AddRemoveUser)?"":"disabled" %> maxlength="60" />
                                </div>  
                                
                                 <%--EmployeedSince--%>
                                <div class="property clearFix">
                                    
                                    <div class="headerPanel">
                                            <%=ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("WorkFromDate").HtmlEncode()%>
                                    </div>
                                    <asp:TextBox runat="server" ID="studio_umInfo_WorkFromDate" CssClass="textCalendar textEditCalendar" />
                                </div>                              
                                
                                <%--Department--%>
                                <div class="property clearFix">
                                    <div class="headerPanel name">
                                        <%=ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("Department").HtmlEncode()%>
                                    </div>
                                    
                                    <div class="value">
                                        <select id="studio_umInfo_Department" <%=ASC.Core.SecurityContext.CheckPermissions(ASC.Core.Users.Constants.Action_EditGroups)?"":"disabled" %>  class="comboBox">
                                            <option class="optionItem" value="<%=Guid.Empty%>"><%=Resources.UserControlsCommonResource.LblSelect %></option>
                                            <%=RenderDepartOptions()%>
                                        </select>
                                    </div>
                                </div>
                                
                                <%--Post--%>
                                <div class="property clearFix">
                                    <div class="headerPanel">
                                            <%=ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("UserPost").HtmlEncode()%>
                                    </div>
                                    <input type="text" id="studio_umInfo_Post" class="textEdit" maxlength="60" />
                                </div>
                                
                                <%--Location--%>
                                <div class="property clearFix">
                                    <div class="headerPanel">
                                            <%=Resources.Resource.Location%>
                                    </div>
                                    <input type="text" id="studio_um_UserLocation" class="textEdit" maxlength="250" />
                                </div>
                                
                            </div>
                            <div id="idSocialProperties" class="properties socialProperties">
                                <%--Location--%>
                                <%--
                                    <div class="property clearFix">
                                      <div class="name">
                                        <span class="title">Location:</span>
                                      </div>
                                      <div class="value">  
                                        <input type="text" id="studio_umInfo_Location" class="textEdit" />
                                      </div>
                                    </div>
                                --%>
                                <%--Sex--%>
                                <div class="property clearFix" style="position: relative; z-index: 4">
                                    <div class="headerPanel name">
                                        <span class="title">
                                            <%=Resources.Resource.Sex%></span>
                                    </div>
                                    <div class="value">
                                        <select id="studio_umInfo_Sex" class="comboBox">
                                            <option class="optionItem" value="-1">
                                                <%=Resources.UserControlsCommonResource.LblSelect %></option>
                                            <option class="optionItem" value="1">
                                                <%=Resources.Resource.MaleSexStatus%></option>
                                            <option class="optionItem" value="0">
                                                <%=Resources.Resource.FemaleSexStatus%></option>
                                        </select>
                                    </div>
                                </div>
                                <%--BirthDate--%>
                                <div class="property clearFix">
                                    <div class="headerPanel name">
                                        <span class="title">
                                            <%=Resources.Resource.Birthdate%></span>
                                    </div>
                                    <div class="value">
                                        <asp:TextBox runat="server" ID="studio_umInfo_BirthDate" CssClass="textEditCalendar textCalendar" />
                                    </div>
                                </div>
                                <%--SocialContacts--%>
                                <div class="property clearFix contacts" style="position: relative; z-index: 2">
                                    <div class="headerPanel name">
                                        <span class="title">
                                            <%=Resources.Resource.Contacts%></span>
                                    </div>
                                    <div class="value">
                                        <div id="contactsContainer" onclick="StudioUserMaker.editContacts(event)">
                                            <ul>
                                                <%-- cloned hidden item --%>
                                                <li class="contact default clearFix">
                                                    <div class="link">
                                                        <input type="text" class="textEdit" value="" />
                                                    </div>
                                                    <select class="comboBox type">
                                                        <option class="optionItem mail" value="mail" selected="selected"><%=Resources.Resource.TitleEmail%></option>
                                                        <option class="optionItem phone" value="phone"><%=Resources.Resource.TitlePhone%></option>
                                                        <option class="optionItem mobphone" value="mobphone"><%=Resources.Resource.TitleMobphone%></option>
                                                        <option class="optionItem facebook" value="facebook"><%=Resources.Resource.TitleFacebook%></option>
                                                        <option class="optionItem livejournal" value="livejournal"><%=Resources.Resource.TitleLiveJournal%></option>
                                                        <option class="optionItem myspace" value="myspace"><%=Resources.Resource.TitleMyspace%></option>
                                                        <option class="optionItem twitter" value="twitter"><%=Resources.Resource.TitleTwitter%></option>
                                                        <option class="optionItem gmail" value="gmail"><%=Resources.Resource.TitleGooglemail%></option>
                                                        <%-- <option class="optionItem gtalk" value="gtalk"><%=Resources.Resource.TitleGoogletalk%></option> --%>
                                                        <%-- <option class="optionItem gbuzz" value="gbuzz"><%=Resources.Resource.TitleGooglebuzz%></option> --%>
                                                        <option class="optionItem blogger" value="blogger"><%=Resources.Resource.TitleBlogger%></option>
                                                        <option class="optionItem skype" value="skype"><%=Resources.Resource.TitleSkype%></option>
                                                        <option class="optionItem yahoo" value="yahoo"><%=Resources.Resource.TitleYahoo%></option>
                                                        <option class="optionItem msn" value="msn"><%=Resources.Resource.TitleMsn%></option>
                                                        <option class="optionItem icq" value="icq"><%=Resources.Resource.TitleIcq%></option>
                                                        <option class="optionItem jabber" value="jabber"><%=Resources.Resource.TitleJabber%></option>
                                                        <option class="optionItem aim" value="aim"><%=Resources.Resource.TitleAim%></option>
                                                    </select>
                                                    <div class="remove">
                                                    </div>
                                                </li>
                                            </ul>
                                            <div class="add"><%=Resources.Resource.BtnAddNewContact%></div>
                                        </div>
                                    </div>
                                </div>
                                <%--Comments--%>
                                <div class="property clearFix">
                                    <div class="headerPanel name">
                                        <span class="title">
                                            <%=Resources.Resource.Comments%></span>
                                    </div>
                                    <div class="value">
                                        <textarea type="text" id="studio_umInfo_AboutMe"></textarea>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="clearFix" id="emp_add_panel_buttons" style="margin-top: 16px;">
                    <a class="baseLinkButton" id="studio_um_saveButton" href="javascript:void(0);" onclick="StudioUserMaker.SaveNewUser();"
                        style="float: left;">
                        <%=Resources.Resource.SaveButton%></a>
                        
                        <a class="grayLinkButton" href="javascript:void(0);"
                            onclick="StudioUserMaker.CloseNewUserDialog();" style="float: left; margin-left: 8px;">
                            <%=Resources.Resource.CancelButton %></a>
                </div>
                <div style="padding-top: 9px; display: none;" id="emp_add_action_loader"
                    class="clearFix">
                    <div class="textMediumDescribe">
                        <%=Resources.Resource.PleaseWaitMessage%>
                    </div>
                    <img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")%>">
                </div>
            </div>
            <div id="studio_um_dialogMessage" style="display: none;">
                <div style="padding: 20px 0px; text-align: center;" id="studio_um_resultMessage">
                </div>
            </div>
        </Body>
        <Options IsPopup="true" />
    </ascwc:Container>
</div>
