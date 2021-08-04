#nullable enable
using AsyncAwaitBestPractices;
using Kit;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SOEWeb.Server.Components
{
    public delegate Task<IEnumerable<T>> InfiniteScrollingItemsProviderRequestDelegate<T>(InfiniteScrollingItemsProviderRequest context);
    public partial class InfiniteScrolling<T> : IList<T>
    {
        private IList<T> _items = new List<T>();
        private ElementReference _lastItemIndicator;
        private DotNetObjectReference<InfiniteScrolling<T>>? _currentComponentReference;
        private IJSObjectReference? _module;
        private IJSObjectReference? _instance;
        private bool _loading = false;
        private CancellationTokenSource? _loadItemsCts;


        [Parameter]
        public InfiniteScrollingItemsProviderRequestDelegate<T>? ItemsProvider { get; set; }

        [Parameter]
        public RenderFragment<T>? ItemTemplate { get; set; }

        [Parameter]
        public RenderFragment? LoadingTemplate { get; set; }

        public bool IsEnabled
        {
            get;
            private set;
        }


        public int Count => this._items.Count;

        public bool IsReadOnly => this._items.IsReadOnly;

        public T this[int index] { get => this._items[index]; set => this._items[index] = value; }

        public InfiniteScrolling()
        {
            IsEnabled = true;
        }


        [JSInvokable]
        public async Task LoadMoreItems()
        {
            if (!IsEnabled)
                return;

            if (_loading)
                return;

            if (ItemsProvider == null)
                return;

            _loading = true;
            try
            {
                _loadItemsCts ??= new CancellationTokenSource();

                StateHasChanged(); // Allow the UI to display the loading indicator
                try
                {
                    var newItems = await ItemsProvider(new InfiniteScrollingItemsProviderRequest(_items.Count, _loadItemsCts.Token));
                    if (newItems?.Any() ?? false)
                    {
                        _items.AddRange(newItems);
                        if (this._instance is not null)
                            await _instance.InvokeVoidAsync("onNewItems");
                    }
                    else
                    {
                        IsEnabled = false;
                    }
                }
                catch (OperationCanceledException oce) when (oce.CancellationToken == _loadItemsCts.Token)
                {
                    // No-op; we canceled the operation, so it's fine to suppress this exception.
                }
            }
            finally
            {
                _loading = false;
            }

            StateHasChanged(); // Display the new items and hide the loading indicator
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            // Initialize the IntersectionObserver
            if (firstRender)
            {
                _module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./InfiniteScrolling.js");
                _currentComponentReference = DotNetObjectReference.Create(this);
                _instance = await _module.InvokeAsync<IJSObjectReference>("initialize", _lastItemIndicator, _currentComponentReference);
            }
        }

        public async ValueTask DisposeAsync()
        {
            // Cancel the current load items operation
            if (_loadItemsCts != null)
            {
                _loadItemsCts.Dispose();
                _loadItemsCts = null;
            }

            // Stop the IntersectionObserver
            if (_instance != null)
            {
                await _instance.InvokeVoidAsync("dispose");
                await _instance.DisposeAsync();
                _instance = null;
            }

            if (_module != null)
            {
                await _module.DisposeAsync();
            }

            _currentComponentReference?.Dispose();
        }

        public int IndexOf(T item)
        {
            return this._items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            this._items.Insert(index, item);
            _instance?.InvokeVoidAsync("onNewItems").SafeFireAndForget();
        }

        public void RemoveAt(int index)
        {
            this._items.RemoveAt(index);
        }

        public void Add(T item)
        {
            this._items.Add(item);
            _instance?.InvokeVoidAsync("onNewItems").SafeFireAndForget();
        }

        public void Clear()
        {
            this._items.Clear();
        }

        public bool Contains(T item)
        {
            return this._items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this._items.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return this._items.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this._items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)this._items).GetEnumerator();
        }
    }

}