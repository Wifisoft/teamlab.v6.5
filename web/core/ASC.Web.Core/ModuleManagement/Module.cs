using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Web.Core.WebZones;

namespace ASC.Web.Core.ModuleManagement
{
    [WebZoneAttribute(WebZoneType.Nowhere)]
    public class Module : IModule
    {
        public Module()
        {
            Widgets = Enumerable.Empty<Widget>();
            Actions = Enumerable.Empty<IWebItem>();
            Navigations = Enumerable.Empty<IWebItem>();
            Context = new ModuleContext();
        }


        public virtual Guid ID
        {
            get;
            set;
        }

        public virtual string Name
        {
            get;
            set;
        }

        public virtual string ModuleSysName
        {
            get;
            set;
        }

        public virtual string Description
        {
            get;
            set;
        }

        public virtual string StartURL
        {
            get;
            set;
        }

        public virtual ModuleContext Context
        {
            get;
            set;
        }

        WebItemContext IWebItem.Context
        {
            get { return Context; }
        }


        public virtual IEnumerable<Widget> Widgets
        {
            get;
            protected set;
        }

        public virtual IEnumerable<IWebItem> Actions
        {
            get;
            protected set;
        }

        public virtual IEnumerable<IWebItem> Navigations
        {
            get;
            protected set;
        }

        public bool DisplayedAlways
        {
            get;
            set;
        }
    }
}
