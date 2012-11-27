using System;
using System.Collections.Generic;
using System.Text;

namespace ASC.Forum
{
    internal abstract class PresenterTemplate<T> : IPresenter
    {
       

        protected T _view;

        #region IPresenter Members

        public void SetView(object view)
        {
            if (!(view is T))
                throw new ViewNotSupportException();

            _view = (T)view;
            RegisterView();
        }

        protected abstract void RegisterView();

        public bool IsSupport(object view)
        {
            if (view is T)
                return true;

            return false;
        }

        private ForumPresenterSettings _settings;

        public void InitPresenter(ForumPresenterSettings settings)
        {
            _settings = settings;
        }

        public ForumPresenterSettings Settings
        {
            get { return _settings; }
        }

        #endregion
    }
}
