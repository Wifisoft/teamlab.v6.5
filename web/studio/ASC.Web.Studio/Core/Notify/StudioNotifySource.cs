using System;
using ASC.Core.Notify;
using ASC.Notify.Engine;
using ASC.Notify.Model;
using ASC.Notify.Patterns;

namespace ASC.Web.Studio.Core.Notify
{
    class StudioNotifySource : NotifySource
    {
        public StudioNotifySource()
            : base("asc.web.studio")
        {
        }


        protected override IActionProvider CreateActionProvider()
        {
            return new ConstActionProvider(
                    Constants.ActionPasswordChanged,

                    Constants.ActionYouAdded,
                    Constants.ActionYouAddedAfterInvite,

                    Constants.ActionYourProfileUpdated,

                    Constants.ActionSelfProfileUpdated,
                    Constants.ActionSendPassword,
                    Constants.ActionInviteUsers,
                    Constants.ActionJoinUsers,
                    Constants.ActionSendWhatsNew,
                    Constants.ActionUserHasJoin,
                    Constants.ActionBackupCreated,
                    Constants.ActionPortalDeactivate,
                    Constants.ActionPortalDelete,
                    Constants.ActionDnsChange,
                    Constants.ActionConfirmOwnerChange,
                    Constants.ActionActivateUsers,
                    Constants.ActionEmailChange,
                    Constants.ActionPasswordChange,
                    Constants.ActionActivateEmail,
                    Constants.ActionProfileDelete,
                    Constants.ActionPhoneChange,
                    Constants.ActionCongratulations,
                    Constants.ActionUserMessageToAdmin
                );
        }

        protected override IPatternProvider CreatePatternsProvider()
        {
            return new XmlPatternProvider(WebPatternResource.webstudio_patterns);
        }

        protected override IActionPatternProvider CreateActionPatternProvider()
        {
            var provider = new XmlActionPatternProvider(
                GetType().Assembly,
                "ASC.Web.Studio.Core.Notify.accordings.xml",
                ActionProvider,
                PatternProvider);

            provider.GetPatternMethod = SelectPattern;
            return provider;
        }

        private IPattern SelectPattern(INotifyAction action, string sender, NotifyRequest request)
        {
            if (action != Constants.ActionAdminNotify) return null; //after that pattern will be selected by xml

            var tagvalue = request.Arguments.Find(tag => tag.Tag.Name == "UNDERLYING_ACTION");
            if (tagvalue == null) return null;

            return ActionPatternProvider.GetPattern(new NotifyAction(Convert.ToString(tagvalue.Value), ""), sender);
        }
    }
}
