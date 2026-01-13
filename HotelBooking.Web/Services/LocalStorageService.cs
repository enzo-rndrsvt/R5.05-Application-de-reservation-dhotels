using Microsoft.JSInterop;
using System.Text.Json;

namespace HotelBooking.Web.Services
{
    public interface ILocalStorageService
    {
        Task<T?> GetItemAsync<T>(string key);
        Task SetItemAsync<T>(string key, T value);
        Task RemoveItemAsync(string key);
    }

    public class LocalStorageService : ILocalStorageService
    {
        private readonly IJSRuntime _jsRuntime;

        public LocalStorageService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<T?> GetItemAsync<T>(string key)
        {
            try
            {
                var json = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);

                if (json == null)
                    return default;

                return JsonSerializer.Deserialize<T>(json);
            }
            catch (InvalidOperationException)
            {
                // JSRuntime n'est pas disponible pendant le prerendering
                return default;
            }
            catch (JSException)
            {
                // Erreur JavaScript
                return default;
            }
        }

        public async Task SetItemAsync<T>(string key, T value)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, 
                    JsonSerializer.Serialize(value));
            }
            catch (InvalidOperationException)
            {
                // JSRuntime n'est pas disponible pendant le prerendering
            }
            catch (JSException)
            {
                // Erreur JavaScript - ignore silencieusement
            }
        }

        public async Task RemoveItemAsync(string key)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
            }
            catch (InvalidOperationException)
            {
                // JSRuntime n'est pas disponible pendant le prerendering
            }
            catch (JSException)
            {
                // Erreur JavaScript - ignore silencieusement
            }
        }
    }
}