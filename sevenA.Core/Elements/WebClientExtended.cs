using System;
using System.Net;

namespace sevenA.Core.Elements
{
    public class WebClientExtended : WebClient
    {
        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            w.Timeout = 10 * 1000;
            return w;
        }
    }
}