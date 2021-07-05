using System;
using System.Collections.Generic;
using System.Text;

namespace SOE.Services.ActionResponse
{
    public class UrlAction : PendingAction
    {
        public  readonly Uri Url;
        public UrlAction(string Url)
        {
            this.Url = new Uri(Url);
        }
    }
}
