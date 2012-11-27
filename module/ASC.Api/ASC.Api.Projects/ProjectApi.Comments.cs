using System;
using ASC.Api.Attributes;
using ASC.Api.Exceptions;
using ASC.Api.Projects.Wrappers;
using ASC.Api.Utils;

namespace ASC.Api.Projects
{
    public partial class ProjectApi
    {
        #region comments
		 ///<summary>
		 ///Returns the information about the comment with the ID specified in the request
		 ///</summary>
		 ///<short>
		 ///Get Comment
		 ///</short>
		 ///<category>Comments</category>
		 ///<param name="commentid">Comment ID</param>
        ///<returns>Comment</returns>        
        /// <exception cref="ItemNotFoundException"></exception>
        [Read(@"comment/{commentid}")]
        public CommentWrapper GetComment(Guid commentid)
        {
            return new CommentWrapper(EngineFactory.GetCommentEngine().GetByID(commentid).NotFoundIfNull());
        }

		  ///<summary>
		  ///Updates the seleted comment using the comment text specified in the request
		  ///</summary>
		  ///<short>
		  ///Update comment
		  ///</short>
		  /// <category>Comments</category>
		  ///<param name="commentid">comment ID</param>
        ///<param name="content">comment text</param>
        ///<returns>Comment</returns>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <example>
        /// <![CDATA[
        /// Sending data in application/json:
        /// 
        /// {
        ///     text:"My comment text",
        ///     
        /// }
        /// 
        /// Sending data in application/x-www-form-urlencoded
        /// content=My%20comment%20text
        /// ]]>
        /// </example>
        [Update(@"comment/{commentid}")]
        public CommentWrapper UpdateComments(Guid commentid, string content)
        {
            var comment = EngineFactory.GetCommentEngine().GetByID(commentid).NotFoundIfNull();
            comment.Content = Update.IfNotEquals(comment.Content, content);
            EngineFactory.GetCommentEngine().SaveOrUpdate(comment);
            return GetComment(commentid);
        }

		  ///<summary>
		  ///Delete the comment with the ID specified in the request from the portal
		  ///</summary>
		  ///<short>
		  ///Delete comment
		  ///</short>
		  /// <category>Comments</category>
		  ///<param name="commentid">comment ID</param>
        /// <exception cref="ItemNotFoundException"></exception>
        [Delete(@"comment/{commentid}")]
        public CommentWrapper DeleteComments(Guid commentid)
        {

            var comment = EngineFactory.GetCommentEngine().GetByID(commentid).NotFoundIfNull();
            comment.Inactive = true;
            EngineFactory.GetCommentEngine().SaveOrUpdate(comment);
            return new CommentWrapper(comment);
        }
        #endregion
    }
}