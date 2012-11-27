using System;
using System.Collections.Generic;
using System.Text;

namespace ASC.Forum
{
    public interface IPresenterFactory
    {
        IPresenter GetPresenter<T>() where T : class;
    }

    public interface IPresenter
    {
        void SetView(object view);

        bool IsSupport(object view);

        void InitPresenter(ForumPresenterSettings settings);

        ForumPresenterSettings Settings { get; }

    }

    public class ViewNotSupportException : Exception
    {
        public ViewNotSupportException()
            : base("Current view not support") { }
    }
}
