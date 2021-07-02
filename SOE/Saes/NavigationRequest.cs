﻿using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SOE.Saes
{
    public class NavigationRequest:IDisposable
    {
        private readonly ManualResetEvent NavigatedCallback;
        public readonly Uri Url;
        public readonly Guid RequestGuid;
        public bool IsComplete = false;
        public NavigationRequest(string Url)
        {
            this.Url =new Uri(Url);
            this.NavigatedCallback = new ManualResetEvent(false);
            this.RequestGuid = Guid.NewGuid();
        }

        internal Task Wait()
        {
            this.NavigatedCallback.Reset();
           return Task.Run(() => this.NavigatedCallback.WaitOne());
        }

        internal void Done()
        {
            this.NavigatedCallback.Set();
        }

        public void Dispose()
        {
            NavigatedCallback?.Dispose();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.RequestGuid).Append(" - ").Append(this.Url.AbsoluteUri);
            return sb.ToString();
        }
    }
}
