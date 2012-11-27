#region Import

using System;
using System.Reflection;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;
using NotifySourceBase = ASC.Core.Notify.NotifySource;

#endregion

namespace ASC.Web.CRM.Services.NotifyService
{
    
    public class NotifySource : NotifySourceBase
    {
        private static NotifySource instance;

        public static NotifySource Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (typeof(NotifySource))
                    {
                        if (instance == null) instance = new NotifySource();
                    }
                }
                return instance;
            }
        }

        public NotifySource()
            : base(new Guid("{13FF36FB-0272-4887-B416-74F52B0D0B02}"))
        {

        }

        protected override IActionProvider CreateActionProvider()
        {
            return new ConstActionProvider(NotifyConstants.Event_ResponsibleForTask,
                                           NotifyConstants.Event_AddRelationshipEvent,
                                           NotifyConstants.Event_SetAccess,
                                           NotifyConstants.Event_ExportCompleted,
                                           NotifyConstants.Event_CreateNewContact);
        }

        protected override IActionPatternProvider CreateActionPatternProvider()
        {
            return new XmlActionPatternProvider(
                GetType().Assembly,
                "ASC.Web.CRM.Services.NotifyService.action_pattern.xml",
                ActionProvider,
                PatternProvider
            ) { GetPatternMethod = ChoosePattern };
        }

        private IPattern ChoosePattern(INotifyAction action, string senderName, ASC.Notify.Engine.NotifyRequest request)
        {
            return null;
        }
        
        protected override IPatternProvider CreatePatternsProvider()
        {
            return new XmlPatternProvider(Assembly.GetExecutingAssembly(), "ASC.Web.CRM.Services.NotifyService.patterns.xml");
        }
    }
}