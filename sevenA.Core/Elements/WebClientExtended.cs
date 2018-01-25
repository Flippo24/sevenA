namespace sevenA.Core.Elements
{
    using System;
    using System.Net;

    public class WebClientExtended : WebClient
    {
        protected override WebRequest GetWebRequest(Uri uri)
        {
            var request = base.GetWebRequest(uri);

            if (request != null)
            {
                request.Timeout = 10 * 1000;
            }

            return request;
        }
    }
}