using System;
using System.Collections.Generic;

namespace ASC.Web.Controls.CommentInfoHelper
{
    public class Attachment
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }

    public class CommentInfo
    {
        private string commentID;
        private string commentBody;
        private Guid userID;
        private string userFullName;
        private string userPost;
        private string userAvatar;
        private DateTime timeStamp;
        private string timeStampStr;
        private bool isRead;
        private bool inactive;
        private bool isEditPermissions;
        private bool isResponsePermissions;
        private string javascriptEdit;
        private string javascriptResponse;
        private string javascriptRemove;
        private IList<CommentInfo> commentList;
        private IList<Attachment> attachments;

        public string CommentID
        {
            get { return commentID; }
            set { commentID = value; }
        }
        public Guid UserID
        {
            get { return userID; }
            set { userID = value; }
        }
        public string UserPost 
        {
            get { return userPost; }
            set { userPost = value; }
        }
        public string UserFullName
        {
            get { return userFullName; }
            set { userFullName = value; }
        }
        public string UserAvatar
        {
            get { return userAvatar; }
            set { userAvatar = value; }
        }
        public string CommentBody
        {
            get { return commentBody; }
            set { commentBody = value; }
        }
        public bool Inactive
        {
            get { return inactive; }
            set { inactive = value; }
        }
        public bool IsRead
        {
            get { return isRead; }
            set { isRead = value; }
        }
        public bool IsEditPermissions
        {
            get { return isEditPermissions; }
            set { isEditPermissions = value; }
        }
        public bool IsResponsePermissions
        {
            get { return isResponsePermissions; }
            set { isResponsePermissions = value; }
        }
        public string JavascriptEdit
        {
            get { return javascriptEdit; }
            set { javascriptEdit = value; }
        }
        public string JavascriptResponse
        {
            get { return javascriptResponse; }
            set { javascriptResponse = value; }
        }
        public string JavascriptRemove
        {
            get { return javascriptRemove; }
            set { javascriptRemove = value; }
        }
        public DateTime TimeStamp
        {
            get { return timeStamp; }
            set { timeStamp = value; }
        }
        public string TimeStampStr
        {
            get { return timeStampStr; }
            set { timeStampStr = value; }
        }
        public IList<CommentInfo> CommentList
        {
            get { return commentList; }
            set { commentList = value; }
        }
        public IList<Attachment> Attachments
        {
            get { return attachments; }
            set { attachments = value; }
        }
    }
}
