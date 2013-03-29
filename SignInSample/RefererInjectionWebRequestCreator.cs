using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SignInSample
{
    /// <summary>
    /// A WebRequest factory that automatically injects a specified Referer 
    /// on all of the WebRequests it generates
    /// </summary>
    public class RefererInjectionWebRequestCreator : IWebRequestCreate
    {
        private readonly string referer;

        public RefererInjectionWebRequestCreator(string referer)
        {
            this.referer = referer;
        }

        /// <summary>
        /// Automatically inject a Referer on all WebRequests 
        /// </summary>
        public WebRequest Create(Uri uri)
        {
            var webRequest = WebRequest.CreateHttp(uri);
            webRequest.Headers[HttpRequestHeader.Referer] = referer;
            return webRequest;
        }
    }
}
