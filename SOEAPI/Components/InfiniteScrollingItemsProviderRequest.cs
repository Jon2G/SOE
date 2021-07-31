using System.Threading;

namespace SOEWeb.Server.Components
{
    public sealed class InfiniteScrollingItemsProviderRequest
    {
        public InfiniteScrollingItemsProviderRequest(int startIndex, CancellationToken cancellationToken)
        {
            StartIndex = startIndex;
            CancellationToken = cancellationToken;
        }

        public int StartIndex { get; }

        public CancellationToken CancellationToken { get; }

        public static InfiniteScrollingItemsProviderRequest Zero =>
            new InfiniteScrollingItemsProviderRequest(0,CancellationToken.None);
    }
}
