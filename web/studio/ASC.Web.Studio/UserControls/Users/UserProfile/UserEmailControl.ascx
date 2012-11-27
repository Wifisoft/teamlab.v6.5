<%@ Import Namespace="ASC.Web.Core.Users" %>
<%@ Import Namespace="ASC.Core" %>
<%@ Import Namespace="ASC.Core.Users" %>
<%@ Import Namespace="ASC.Web.Studio.Core" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserEmailControl.ascx.cs"
    Inherits="ASC.Web.Studio.UserControls.Users.UserProfile.UserEmailControl" %>
<%if (!String.IsNullOrEmpty(User.Email))
  {
      if (Viewer.IsAdmin() || Viewer.ID == User.ID)
      {
          if (User.ActivationStatus == EmployeeActivationStatus.Activated)
          {%>
            <tr class="field" style="vertical-align: top;">
                <td class="name textBigDescribe">
                    <%=Resources.Resource.Email%>:
                </td>
                <td class="value contact">
                    <div style="width: 600px;">
                        <a class="mail" href="mailto:<%=User.Email.ToLower()%>" title="<%=HttpUtility.HtmlEncode(User.Email.ToLower())%>">
                            <%=HttpUtility.HtmlEncode(User.Email.ToLower())%></a>
                        <a class="linkAction baseLinkAction" style="padding-left: 0px; margin-left: 25px;" onclick="EmailOperationManager.ShowEmailChangeWindow('<%=User.Email%>','<%=User.ID%>', <%= Viewer.IsAdmin().ToString().ToLower() %>); return false;">
                            <%=Resources.Resource.ChangeEmail%></a>
                    </div>
                </td>
            </tr>
            <%}
          else if (User.ActivationStatus == EmployeeActivationStatus.NotActivated)
          {%>
            <tr class="field" style="vertical-align: top;">
                <td class="name textBigDescribe">
                    <%=Resources.Resource.Email%>:
                </td>
                <td class="value contact">
                    <div class="tintMedium emailWarning">
                        <a class="mail" href="mailto:<%=User.Email.ToLower()%>" title="<%=HttpUtility.HtmlEncode(User.Email.ToLower())%>">
                            <%=HttpUtility.HtmlEncode(User.Email.ToLower())%></a>
                            <div class="caption">
                                <%=Resources.Resource.EmailIsNotConfirmed%>
                            </div>
                        <a href="javascript:void(0);" class="activate" onclick="EmailOperationManager.ShowEmailActivationWindow('<%=User.Email%>','<%=User.ID%>',  <%= Viewer.IsAdmin().ToString().ToLower() %>); return false;">
                            <%=Resources.Resource.ActivateEmailAgain%>
                        </a>
                    </div>
                </td>
            </tr>
            <%}
          else if (User.ActivationStatus == EmployeeActivationStatus.Pending)
          {%>
              
               <tr class="field" style="vertical-align: top;">
                <td class="name textBigDescribe">
                    <%=Resources.Resource.Email%>:
                </td>
                <td class="value contact">
                    <div class="tintMedium emailWarning">
                        <a class="mail" href="mailto:<%=User.Email.ToLower()%>" title="<%=HttpUtility.HtmlEncode(User.Email.ToLower())%>">
                            <%=HttpUtility.HtmlEncode(User.Email.ToLower())%></a>
                            <div class="caption">
                                <%=Resources.Resource.PendingTitle%>
                            </div>
                            <div class="description"><%=ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("PendingDescription").HtmlEncode()%></div>
                         <%if (User.Status != ASC.Core.Users.EmployeeStatus.Terminated)
              {%>
                        <a href="javascript:void(0);" class="activate" onclick="EmailOperationManager.ShowResendInviteWindow('<%=User.Email%>','<%=User.ID%>', <%= Viewer.IsAdmin().ToString().ToLower() %>); return false;">
                            <%=Resources.Resource.SendInviteAgain%>
                        </a>
                        <% }    %>
                    </div>
                </td>
            </tr>
          
         <% }   
          
      }
      // Viewer is not the admin
      else
      {%>
            <tr class="field" style="vertical-align: top;">
                <td class="name textBigDescribe">
                    <%=Resources.Resource.Email%>:
                </td>
                <td class="value contact">
                    <div style="width: 600px;">
                        <a class="mail" href="mailto:<%=User.Email.ToLower()%>" title="<%=HttpUtility.HtmlEncode(User.Email.ToLower())%>">
                            <%=HttpUtility.HtmlEncode(User.Email.ToLower())%></a>
                    </div>
                </td>
            </tr>
      <%}
      
  }%>