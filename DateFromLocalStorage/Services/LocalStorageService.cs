using Microsoft.JSInterop;


namespace DateFromLocalStorage.Services
{
    public class LocalStorageService
    {
        private Lazy<IJSObjectReference> _accessorJsRef = new();
        private readonly IJSRuntime _jsRuntime;

        public LocalStorageService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        private async Task WaitForReference()
        {
            if (_accessorJsRef.IsValueCreated is false)
            {
                _accessorJsRef = new(await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/LocalStorageAccessor.js"));
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_accessorJsRef.IsValueCreated)
            {
                await _accessorJsRef.Value.DisposeAsync();
            }
        }

        public async Task SetSessionString(string title, string valeur)
        {
            try
            {
                await WaitForReference();
                await _accessorJsRef.Value.InvokeAsync<string>("set", title, valeur);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<T> GetSessionStringAsync<T>(string title)
        {
            try
            {
                await WaitForReference();
                var result = await _accessorJsRef.Value.InvokeAsync<T>("get", title);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return default(T);
            }
        }
    }
}
