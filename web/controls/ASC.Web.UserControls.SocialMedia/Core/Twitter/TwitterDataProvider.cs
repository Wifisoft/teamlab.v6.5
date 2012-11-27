using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using ASC.SocialMedia.Twitter;
using log4net;
using Twitterizer;

namespace ASC.SocialMedia.Twitter
{
    /// <summary>
    /// Contains methods for getting data from Twitter
    /// </summary>
    public class TwitterDataProvider
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TwitterDataProvider));
        private TwitterApiInfo _apiInfo;

        public enum ImageSize
        {
            Small,
            Original
        }

        /// <summary>
        /// Costructor
        /// </summary>
        /// <param name="apiInfo">TwitterApiInfo object</param>
        public TwitterDataProvider(TwitterApiInfo apiInfo)
        {
            if (apiInfo == null)
                throw new ArgumentNullException("apiInfo");

            _apiInfo = apiInfo;
        }

        /// <summary>
        /// Gets current user (defined by access token) home timeline
        /// </summary>
        /// <param name="messageCount">Message count</param>        
        /// <returns>Message list</returns>
        public List<Message> GetUserHomeTimeLine(int messageCount)
        {
            try
            {
                Twitterizer.OAuthTokens tokens = GetOAuthTokens();

                Twitterizer.UserTimelineOptions options = new Twitterizer.UserTimelineOptions();
                options.Count = messageCount;
                options.IncludeRetweets = true;

                TwitterResponse<TwitterStatusCollection> statusResponse = TwitterTimeline.HomeTimeline(tokens, options);

                if (statusResponse.Result == RequestResult.Success)
                    return MapMessage(statusResponse.ResponseObject);

                else
                    throw CreateException(statusResponse.Result, statusResponse.ErrorMessage);

            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets tweets posted by specified user
        /// </summary>
        /// <param name="messageCount">Message count</param>        
        /// <returns>Message list</returns>
        public List<Message> GetUserTweets(decimal? userID, string screenName, int messageCount)
        {
            try
            {
                Twitterizer.OAuthTokens tokens = GetOAuthTokens();

                Twitterizer.UserTimelineOptions options = new Twitterizer.UserTimelineOptions();
                options.ScreenName = screenName;
                if (userID.HasValue)
                    options.UserId = userID.Value;
                options.IncludeRetweets = true;
                options.Count = messageCount;

                TwitterResponse<TwitterStatusCollection> statusResponse = TwitterTimeline.UserTimeline(tokens, options);

                if (statusResponse.Result == RequestResult.Success)
                    return MapMessage(statusResponse.ResponseObject);

                else
                    throw CreateException(statusResponse.Result, statusResponse.ErrorMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }

        /// <summary>
        /// Loads specified user information
        /// </summary>
        /// <param name="userID">Twitter user ID</param>
        /// <returns>TwitterUserInfo obect</returns>
        public TwitterUserInfo LoadUserInfo(decimal userID)
        {
            try
            {
                Twitterizer.OAuthTokens tokens = GetOAuthTokens();

                TwitterResponse<TwitterUser> userResponse = TwitterUser.Show(tokens, userID);
                if (userResponse.Result == RequestResult.Success)
                    return MapUser(userResponse.ResponseObject);

                else
                    throw CreateException(userResponse.Result, userResponse.ErrorMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets last 20 users
        /// </summary>
        /// <param name="search">Search string</param>
        /// <returns>TwitterUserInfo list</returns>
        public List<TwitterUserInfo> FindUsers(string search)
        {
            const int pageRowCount = 20;
            const int pageNumber = 0;

            try
            {
                Twitterizer.OAuthTokens tokens = GetOAuthTokens();

                UserSearchOptions options = new UserSearchOptions();
                options.Page = pageNumber;
                options.NumberPerPage = pageRowCount;

                TwitterResponse<TwitterUserCollection> userResponse = TwitterUser.Search(tokens, search, options);
                if (userResponse.Result == RequestResult.Success)
                {
                    TwitterUserCollection collection = userResponse.ResponseObject;
                    return MapUsers(userResponse.ResponseObject);
                }
                else
                    throw CreateException(userResponse.Result, userResponse.ErrorMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }

        private Twitterizer.OAuthTokens GetOAuthTokens()
        {
            return new Twitterizer.OAuthTokens
            {
                ConsumerKey = _apiInfo.ConsumerKey,
                ConsumerSecret = _apiInfo.ConsumerSecret,
                AccessToken = _apiInfo.AccessToken,
                AccessTokenSecret = _apiInfo.AccessTokenSecret,
            };
        }

        /// <summary>
        /// Gets url of User image
        /// </summary>
        /// <param name="userScreenName"></param>
        /// <exception cref="ASC.SocialMedia.TwitterDataProvider.Exceptions.ResourceNotFoundException">ResourceNotFoundException</exception>
        /// <exception cref="ASC.SocialMedia.TwitterDataProvider.Exceptions.InternalProviderException">InternalProviderException</exception>
        /// <returns>Url of image or null if resource does not exist</returns>
        public string GetUrlOfUserImage(string userScreenName, ImageSize imageSize)
        {
            String url = String.Format("http://api.twitter.com/1/users/profile_image?screen_name={0}&size={1}", userScreenName, GetTwitterImageSizeText(imageSize));
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    if (response.ResponseUri.ToString().Contains("default_profile"))
                        return null;

                    return response.ResponseUri.ToString();
                }
            }
            catch (WebException ex)
            {
                if (ex.Response == null || ((HttpWebResponse)ex.Response).StatusCode != HttpStatusCode.NotFound)
                {
                    log.Error(ex);
                    throw;
                }
                return null;
            }
        }

        private string GetTwitterImageSizeText(ImageSize imageSize)
        {
            string result = "original";
            if (imageSize == ImageSize.Small)
                result = "normal";
            return result;
        }


        private List<Message> MapMessage(TwitterStatusCollection statusCollection)
        {
            if (statusCollection == null)
                return null;

            List<Message> messageCollection = new List<Message>();

            foreach (TwitterStatus status in statusCollection)
            {
                TwitterMessage message = new TwitterMessage();
                message.PostedOn = status.CreatedDate;
                message.Source = SocialNetworks.Twitter;
                message.Text = status.LinkifiedText();
                message.UserName = status.User.Name;
                message.UserImageUrl = status.User.ProfileImageLocation;

                messageCollection.Add(message);
            }

            return messageCollection.OrderByDescending(msg => msg.PostedOn).ToList();
        }

        private List<TwitterUserInfo> MapUsers(TwitterUserCollection twitterUserCollection)
        {
            if (twitterUserCollection == null)
                return null;

            List<TwitterUserInfo> twitterUsers = new List<TwitterUserInfo>();

            foreach (TwitterUser twitterUser in twitterUserCollection)
            {
                TwitterUserInfo userInfo = MapUser(twitterUser);
                twitterUsers.Add(userInfo);
            }

            return twitterUsers;
        }

        private TwitterUserInfo MapUser(TwitterUser twitterUser)
        {
            if (twitterUser == null)
                return null;

            TwitterUserInfo userInfo = new TwitterUserInfo
            {
                UserName = twitterUser.Name,
                SmallImageUrl = twitterUser.ProfileImageLocation,
                Description = twitterUser.Description,
                UserID = twitterUser.Id,
                ScreenName = twitterUser.ScreenName
            };

            return userInfo;
        }


        private void ProcessError(RequestResult requestResult, string errorMessage, string methodName)
        {
            /*
            var ex = CreateException(requestResult);
            log.Error(ex);
            return ex;
            LogError(requestResult, errorMessage, methodName, null);
            ThrowException(requestResult);*/
        }

        private Exception CreateException(RequestResult requestResult, string errorMessage)
        {
            switch (requestResult)
            {
                case RequestResult.ConnectionFailure:
                    return new ConnectionFailureException(errorMessage);

                case RequestResult.FileNotFound:
                    return new ResourceNotFoundException(errorMessage);

                case RequestResult.RateLimited:
                    return new RateLimitException(errorMessage);

                case RequestResult.Unauthorized:
                    return new UnauthorizedException(errorMessage);
                default:
                    return new TwitterException(errorMessage);
            }
        }
    }
}
