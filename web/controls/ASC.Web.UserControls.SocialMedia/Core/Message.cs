using System;

namespace ASC.SocialMedia
{
    /// <summary>
    /// Represents an user activity message
    /// </summary>
    public class Message
    {
        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Message text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The date of message post
        /// </summary>
        public DateTime PostedOn { get; set; }

        /// <summary>
        /// Social network
        /// </summary>
        public SocialNetworks Source { get; set; }

    }
}
