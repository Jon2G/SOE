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

namespace SOEAWS.Components
{
    public delegate Task<IEnumerable<T>> InfiniteScrollingItemsProviderRequestDelegate<T>(InfiniteScrollingItemsProviderRequest context);
    public partial class InfiniteScrolling<T> : IList<T>
    {
        [Inject] 
        private IJSRuntime JSRuntime { get; set; }

        private IList<T> _items = new List<T>();
        private ElementReference _lastItemIndicator;
        private DotNetObjectReference<InfiniteScrolling<T>>? _currentComponentReference;
        //private IJSObjectReference? _module;
        //private IJSObjectReference? _instance;
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
            this.IsEnabled = true;
        }


        [JSInvokable]
        public async Task LoadMoreItems()
        {
            if (!this.IsEnabled)
                return;

            if (this._loading)
                return;

            if (this.ItemsProvider == null)
                return;

            this._loading = true;
            try
            {
                this._loadItemsCts ??= new CancellationTokenSource();

                this.StateHasChanged(); // Allow the UI to display the loading indicator
                try
                {
                    var newItems = await this.ItemsProvider(new InfiniteScrollingItemsProviderRequest(this._items.Count, this._loadItemsCts.Token));
                    if (newItems?.Any() ?? false)
                    {
                        this._items.AddRange(newItems);
                        //if (this._instance is not null)
                        //    await this._instance.InvokeVoidAsync("onNewItems");
                    }
                    else
                    {
                        this.IsEnabled = false;
                    }
                }
                catch (OperationCanceledException oce) when (oce.CancellationToken == this._loadItemsCts.Token)
                {
                    // No-op; we canceled the operation, so it's fine to suppress this exception.
                }
            }
            finally
            {
                this._loading = false;
            }

            this.StateHasChanged(); // Display the new items and hide the loading indicator
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            // Initialize the IntersectionObserver
            if (firstRender)
            {
                //_module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./InfiniteScrolling.js");
                //_currentComponentReference = DotNetObjectReference.Create(this);
                //_instance = await _module.InvokeAsync<IJSObjectReference>("initialize", _lastItemIndicator, _currentComponentReference);
            }
        }

        public async ValueTask DisposeAsync()
        {
            // Cancel the current load items operation
            if (this._loadItemsCts != null)
            {
                this._loadItemsCts.Dispose();
                this._loadItemsCts = null;
            }

            // Stop the IntersectionObserver
            //if (this._instance != null)
            //{
            //    await this._instance.InvokeVoidAsync("dispose");
            //    await this._instance.DisposeAsync();
            //    this._instance = null;
            //}

            //if (this._module != null)
            //{
            //    await this._module.DisposeAsync();
            //}

            this._currentComponentReference?.Dispose();
        }

        public int IndexOf(T item)
        {
            return this._items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            this._items.Insert(index, item);
            //this._instance?.InvokeVoidAsync("onNewItems").SafeFireAndForget();
        }

        public void RemoveAt(int index)
        {
            this._items.RemoveAt(index);
        }

        public void Add(T item)
        {
            this._items.Add(item);
            //this._instance?.InvokeVoidAsync("onNewItems").SafeFireAndForget();
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