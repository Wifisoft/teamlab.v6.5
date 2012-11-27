using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Xmpp.Server.Utils;

namespace ASC.Xmpp.Server.Authorization
{
	public class AuthManager
	{
		private readonly Dictionary<string, AuthToken> tokenInfos = new Dictionary<string, AuthToken>();

		private const int tokenValidity = 30;//30 min

		public string GetUserToken(string username)
		{
			if (string.IsNullOrEmpty(username)) throw new ArgumentNullException("username");

			var token = new AuthToken(username);
			if (!tokenInfos.ContainsKey(token.Token))
			{
				tokenInfos.Add(token.Token, token);
				return token.Token;
			}
			return string.Empty;
		}

		public string RestoreUserToken(string token)
		{
			if (string.IsNullOrEmpty(token)) throw new ArgumentNullException("token");

			if (tokenInfos.ContainsKey(token) && (DateTime.UtcNow - tokenInfos[token].Created).TotalMinutes < tokenValidity)
			{
				var user = tokenInfos[token].Username;
				CleanTokens(token);
				return user;
			}
			return null;
		}

		private void CleanTokens(string tokeset)
		{
			tokenInfos.Remove(tokeset);
			foreach (var info in new Dictionary<string, AuthToken>(tokenInfos))
			{
				if ((DateTime.UtcNow - info.Value.Created).TotalMinutes > tokenValidity)
				{
					tokenInfos.Remove(info.Key);//remove token
				}
			}
		}


		private class AuthToken
		{
			public string Username
			{
				get;
				private set;
			}

			public DateTime Created
			{
				get;
				private set;
			}

			public string Token
			{
				get;
				private set;
			}

			public AuthToken(string username)
			{
				Username = username;
				Created = DateTime.UtcNow;
				Token = UniqueId.CreateNewId(50);
			}
		}
	}
}