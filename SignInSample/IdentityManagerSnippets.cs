using System;
using System.Net;
using ESRI.ArcGIS.Client;

namespace SignInSample
{
    public class IdentityManagerSnippets
    {
        #region RegisterServer
        // Register Server
        public void RegisterServers()
        {
            var serverInfo = new IdentityManager.ServerInfo()
            {
                ServerUrl = "http://myserver",
                TokenServiceUrl = "http://myserver/sharing/rest/generateToken"
            };
            IdentityManager.Current.RegisterServers(new[] { serverInfo });
        } 
        #endregion

        #region GenerateCredential
        // Generate Credential outside of Challenge
        public void GenerateCredentialNotChallenged()
        {
            IdentityManager.Current.GenerateCredentialAsync(
                "https://myserver/",
                "username",
                "password",
                (cred, ex) =>
                {
                    if (ex == null)
                        IdentityManager.Current.AddCredential(cred);
                },
                new IdentityManager.GenerateTokenOptions() { Referer = "myReferer" });
        }
        // Generate Credential while being Challenged
        public void GenerateCredentialWhenChallenged()
        {
            IdentityManager.Current.ChallengeMethodEx =
                (infos, callback, gto) =>
                {
                    IdentityManager.Current.GenerateCredentialAsync(
                        infos.Url,
                        "username",
                        "password",
                        callback,
                        gto);
                };
        } 
        #endregion

        #region AddCredential (and AutoRefresh)
        // Bootstrap IdentityManager with stored Token
        public void BootstrapWithKnownToken()
        {
            //Register the server first

            //Provide IdentityManager with Token credentials
            IdentityManager.Current.AddCredential(
                new IdentityManager.Credential()
                    {
                        Url = "http://myserver/",
                        Token = "myPersistedToken"
                    });
        }

        // Bootstrap IdentityManager with known credentials
        public void BootstrapWithKnownUsernameAndPassword()
        {
            //Register the server first

            IdentityManager.Current.TokenGenerationReferer = "myReferer";

            //Provide IdentityManager with known username and password
            IdentityManager.Current.AddCredential(
                new IdentityManager.Credential()
                {
                    Url = "http://myserver/",
                    AutoRefresh = true, //Tells IdentityManager to automatically regenerate a Token
                    UserName = "username",
                    Password = "password",
                    // Provide some arbitrary text as Token to force IdentityManager to AutoRefresh
                    Token = "anyTextHere"
                });
        } 
        #endregion

        #region WP8 WebRequest Referer Injection
        //When our App targets WP8 it is important to note that all WebRequests 
        //do not automatically sent a default Referer like it does when our App targets WP7!

        //How to force all WebRequests on WP8 to send the Referer we use for generating Tokens
        public void InjectingRefererOnWP8()
        {
            //Specify the referer we want to use when generating tokens
            IdentityManager.Current.TokenGenerationReferer = "myReferer";

            //Force all web requests made by the system to automatically send the same referer
            //so token secure requests will include the correct referer
            var refererWebRequestCreator = new RefererInjectionWebRequestCreator();
            WebRequest.RegisterPrefix("http://", refererWebRequestCreator);
            WebRequest.RegisterPrefix("https://", refererWebRequestCreator);

            //Tip: The same trick can be used to be able to log and debug all WebRequests
            //made by our App and the API at runtime!
        }

        public class RefererInjectionWebRequestCreator : IWebRequestCreate
        {
            /// <summary>
            /// Automatically inject a Referer on all WebRequests 
            /// </summary>
            public WebRequest Create(Uri uri)
            {
                var webRequest = WebRequest.CreateHttp(uri);
                webRequest.Headers[HttpRequestHeader.Referer] = IdentityManager.Current.TokenGenerationReferer;
                return webRequest;
            }
        } 
        #endregion

        #region SingeSignOn

        public void EnforcingSingeSignOn()
        {
            //We assume IdentityManager already has registered credentials for our Portal
            const string ConnectedPortalUrl = "http://myportal/";

            IdentityManager.Current.ChallengeMethodEx =
                (infos, callback, gto) =>
                    {
                        var serverInfo = IdentityManager.Current.FindServerInfo(infos.Url);
                        if(HasSameOrigin(serverInfo.OwningSystemUrl, ConnectedPortalUrl)) //Host matches?
                        {
                            //We are challenged for a secure resource that our credentials should
                            //have been able to access. Enforce SingeSignOn by auto-failing
                            callback(null, new Exception("SingleSignOn"));
                        }
                        else
                        {
                            //Prompt user for credentials (show SignIn Dialog)
                        }
                    };
        }

        private bool HasSameOrigin(string url1, string url2)
        {
            Uri uri1 = new Uri(url1);
            Uri uri2 = new Uri(url2);
            return uri1.Scheme == uri2.Scheme && uri1.Host == uri2.Host && uri1.Port == uri2.Port;
        }
        #endregion

        #region IdentityManager Safe Challenge Cancelling
        //How to stop challenging the user for secure resources that our pre-registered
        //credentials (the only ones we know) don't have access to and also keeping
        //IdentityManager Enabled. Setting IdentityManager's Challenge method to null
        //disables IdentityManager completely.

        public void SafeChallengeCancelling()
        {
            //First register our single credential with IdentityManager

            //Dont set the Challenge method to null as a means to stop being challenged!
            //Instead always callback all challenges with an Exception.
            IdentityManager.Current.ChallengeMethodEx =
                (infos, callback, gto) =>
                {
                    callback(null, new Exception("Resource is inaccessible with our current single credentials"));
                };
        }
        #endregion
    }
}
