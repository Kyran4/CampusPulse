using System.Net.Http;
using System.Net.Http.Json;

namespace CampusPulse.Services;

public class ApiClient
{
    // Leave blank to use the automatic per-platform defaults below (fine
    // when everything runs on one machine - an emulator plus the API on
    // your own laptop). Fill in your machine's LAN IP (the one running the
    // API) when testing on real, separate devices over WiFi, e.g.:
    //
    //     private const string ServerIp = "192.168.1.23";
    //
    // How to find it (on the machine running the API):
    //   Windows: open Command Prompt, run "ipconfig", look for
    //            "IPv4 Address" under your active Wi-Fi adapter.
    //   Mac:     System Settings -> Wi-Fi -> Details, or run
    //            "ipconfig getifaddr en0" in Terminal.
    // All devices (the API host and every phone/tablet/laptop testing
    // against it) need to be on the same WiFi network, and Windows
    // Firewall needs to allow inbound connections on port 5162.
    private const string ServerIp = "192.168.68.57"; // e.g. "192.168.1.23"

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
    // ServerIp above always wins when set, regardless of platform - that's
    // the switch for "everyone's on their own real device now."
    private static string GetBaseUrl()
    {
        if (!string.IsNullOrWhiteSpace(ServerIp))
            return $"http://{ServerIp}:5162";

#if ANDROID
        return "http://10.0.2.2:5162";
#else
        return "http://localhost:5162";
#endif
    }

    // Set whenever a call fails, so callers that got back null/empty can
    // tell "the server legitimately returned nothing" apart from "we
    // couldn't reach the server at all" and show the right message.
    public string? LastError { get; private set; }

    // A timed-out request throws TaskCanceledException, which reads like
    // "the user cancelled this" rather than "this device couldn't reach the
    // server in time" - the actual, much more common cause (wrong ServerIp,
    // different network, firewall). Centralising this so every method
    // reports the same clear reason instead of a confusing exception name.
    private string DescribeException(Exception ex, string method, string endpoint)
    {
        string reason = ex is TaskCanceledException or TimeoutException
            ? "Couldn't reach the server in time. Check ServerIp in ApiClient.cs is set to the API host's current IP, and that this device is on the same network."
            : ex.Message;

        Console.WriteLine($"API Error ({method} {endpoint}): {reason}");
        return reason;
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            return await _http.GetFromJsonAsync<T>(endpoint);
        }
        catch (Exception ex)
        {
            LastError = DescribeException(ex, "GET", endpoint);
            return default;
        }
    }

    public async Task<bool> PostAsync(string endpoint, object data)
    {
        try
        {
            var response = await _http.PostAsJsonAsync(endpoint, data);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            LastError = DescribeException(ex, "POST", endpoint);
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
            LastError = DescribeException(ex, "POST", url);
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
            LastError = DescribeException(ex, "PUT", endpoint);
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
            LastError = DescribeException(ex, "PUT", endpoint);
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
            LastError = DescribeException(ex, "DELETE", endpoint);
            return false;
        }
    }
}
