using System.Threading;

namespace SOEWeb.Components
{
    public sealed class InfiniteScrollingItemsProviderRequest
    {
        public InfiniteScrollingItemsProviderRequest(int startIndex, CancellationToken cancellationToken)
        {
            this.StartIndex = startIndex;
            this.CancellationToken = cancellationToken;
        }

        public int StartIndex { get; }

        public CancellationToken CancellationToken { get; }

        public static InfiniteScrollingItemsProviderRequest Zero =>
            new InfiniteScrollingItemsProviderRequest(0,CancellationToken.None);
    }
}
