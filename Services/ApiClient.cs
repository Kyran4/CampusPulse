using System.Net.Http;
using System.Net.Http.Json;

namespace CampusPulse.Services;

public class ApiClient
{
    private readonly HttpClient _http;

    public ApiClient(HttpClient http)
    {
        _http = http;
        _http.BaseAddress = new Uri(GetBaseUrl());
    }

    // 10.0.2.2 is a special alias that ONLY resolves from inside the Android
    // emulator (it maps to the host machine's localhost). It means nothing
    // on Windows, iOS simulator, or a real device - every request would
    // fail there. #if ANDROID / #else are compile-time platform checks the
    // MAUI SDK already defines per target framework, so each platform build
    // gets the right constant baked in.
    //
    // Physical device (real phone, not an emulator/simulator): neither of
    // these works - use your PC's actual LAN IP instead, e.g.
    // "http://192.168.1.23:5162", and make sure the phone is on the same
    // network and Windows Firewall allows inbound on that port.
    private static string GetBaseUrl()
    {
#if ANDROID
        return "http://10.0.2.2:5162";
#else
        return "http://localhost:5162";
#endif
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            return await _http.GetFromJsonAsync<T>(endpoint);
        }
        catch (Exception ex)
        {
            // Surfaced to Console so it shows in the debug output - if
            // every call is failing with this, it's almost always the base
            // URL not matching whatever you're running the app on (see
            // GetBaseUrl above), not a bug in the calling page.
            Console.WriteLine("API Error (GET " + endpoint + "): " + ex.Message);
            LastError = ex.Message;
            return default;
        }
    }

    // Set whenever a call fails, so callers that got back null/empty can
    // tell "the server legitimately returned nothing" apart from "we
    // couldn't reach the server at all" and show the right message.
    public string? LastError { get; private set; }

    public async Task<bool> PostAsync(string endpoint, object data)
    {
        try
        {
            var response = await _http.PostAsJsonAsync(endpoint, data);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine("API Error (POST " + endpoint + "): " + ex.Message);
            LastError = ex.Message;
            return false;
        }
    }

    public async Task<T?> PostAsync<T>(string url, object body)
    {
        try
        {
            var response = await _http.PostAsJsonAsync(url, body);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine("API Error (POST " + url + "): " + error);
                LastError = error;
                return default;
            }

            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch (Exception ex)
        {
            Console.WriteLine("API Error (POST " + url + "): " + ex.Message);
            LastError = ex.Message;
            return default;
        }
    }

    public async Task<bool> PutAsync(string endpoint, object data)
    {
        try
        {
            var response = await _http.PutAsJsonAsync(endpoint, data);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine("API Error (PUT " + endpoint + "): " + ex.Message);
            LastError = ex.Message;
            return false;
        }
    }

    public async Task<T?> PutAsync<T>(string endpoint, object data)
    {
        try
        {
            var response = await _http.PutAsJsonAsync(endpoint, data);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine("API Error (PUT " + endpoint + "): " + error);
                LastError = error;
                return default;
            }

            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch (Exception ex)
        {
            Console.WriteLine("API Error (PUT " + endpoint + "): " + ex.Message);
            LastError = ex.Message;
            return default;
        }
    }

    public async Task<bool> DeleteAsync(string endpoint)
    {
        try
        {
            var response = await _http.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine("API Error (DELETE " + endpoint + "): " + ex.Message);
            LastError = ex.Message;
            return false;
        }
    }
}
