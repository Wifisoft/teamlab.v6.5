using System;
using System.Collections.Generic;
using System.Text;

namespace ASC.Forum
{
    internal class UserKeys
    {
        public static readonly string StringSessionKey = "asc_forum_keys_";
        public static readonly string StringThreadKey = "thread_";
        //public static readonly string StringTopicKey = "topic_";
        //public static readonly string AttachmentsKey = "attachments";
        //public static readonly string QuoteKey = "quote";

        public static string ForumVisitsCookies
        {
            get
            {
                return "asc_forum_visits";
            }
        }

    }
}
