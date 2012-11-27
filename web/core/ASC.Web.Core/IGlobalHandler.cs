using System;

namespace ASC.Web.Core
{
    public interface IGlobalHandler
    {
        /// <summary>
        /// User login handler
        /// </summary>
        /// <param name="userID">User indetifier</param>
        void Login(Guid userID);

        /// <summary>
        /// User logout handler
        /// </summary>
        /// <param name="userID">User identifier</param>
        void Logout(Guid userID);
    }   
}
