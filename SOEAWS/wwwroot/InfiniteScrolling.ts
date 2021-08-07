declare module DotNet {
    /**
     * Invokes the specified .NET public method synchronously. Not all hosting scenarios support
     * synchronous invocation, so if possible use invokeMethodAsync instead.
     *
     * @param assemblyName The short name (without key/version or .dll extension) of the .NET assembly containing the method.
     * @param methodIdentifier The identifier of the method to invoke. The method must have a [JSInvokable] attribute specifying this identifier.
     * @param args Arguments to pass to the method, each of which must be JSON-serializable.
     * @returns The result of the operation.
     */
    function invokeMethod<T>(assemblyName: string, methodIdentifier: string, ...args: any[]): T;
    /**
     * Invokes the specified .NET public method asynchronously.
     *
     * @param assemblyName The short name (without key/version or .dll extension) of the .NET assembly containing the method.
     * @param methodIdentifier The identifier of the method to invoke. The method must have a [JSInvokable] attribute specifying this identifier.
     * @param args Arguments to pass to the method, each of which must be JSON-serializable.
     * @returns A promise representing the result of the operation.
     */
    function invokeMethodAsync<T>(assemblyName: string, methodIdentifier: string, ...args: any[]): Promise<T>;
    /**
     * Represents the .NET instance passed by reference to JavaScript.
     */
    interface DotNetObject {
        /**
        * Invokes the specified .NET instance public method synchronously. Not all hosting scenarios support
        * synchronous invocation, so if possible use invokeMethodAsync instead.
        *
        * @param methodIdentifier The identifier of the method to invoke. The method must have a [JSInvokable] attribute specifying this identifier.
        * @param args Arguments to pass to the method, each of which must be JSON-serializable.
        * @returns The result of the operation.
        */
        invokeMethod<T>(methodIdentifier: string, ...args: any[]): T;
        /**
         * Invokes the specified .NET instance public method asynchronously.
         *
         * @param methodIdentifier The identifier of the method to invoke. The method must have a [JSInvokable] attribute specifying this identifier.
         * @param args Arguments to pass to the method, each of which must be JSON-serializable.
         * @returns A promise representing the result of the operation.
         */
        invokeMethodAsync<T>(methodIdentifier: string, ...args: any[]): Promise<T>;
    }
}

export function initialize(lastIndicator: HTMLElement, instance: DotNet.DotNetObject) {
    const options = {
        root: findClosestScrollContainer(lastIndicator),
        rootMargin: '0px',
        threshold: 0,
    };

    const observer = new IntersectionObserver(async (entries) => {
        for (const entry of entries) {
            if (entry.isIntersecting) {
                await instance.invokeMethodAsync("LoadMoreItems");
            }
        }
    }, options);

    observer.observe(lastIndicator);

    return {
        dispose: () => infiniteScollingDispose(observer),
        onNewItems: () => {
            observer.unobserve(lastIndicator);
            observer.observe(lastIndicator);
        },
    };
}

function findClosestScrollContainer(element: HTMLElement | null): HTMLElement | null {
    while (element) {
        const style = getComputedStyle(element);
        if (style.overflowY !== 'visible') {
            return element;
        }

        element = element.parentElement;
    }

    return null;
}

function infiniteScollingDispose(observer: IntersectionObserver) {
    observer.disconnect();
}