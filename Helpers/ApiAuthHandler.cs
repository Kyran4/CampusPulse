using System.Net.Http.Headers;
using CampusPulse.Services;

namespace CampusPulse.Helpers;

public class ApiAuthHandler : DelegatingHandler
{
    private readonly DatabaseService _db;

    public ApiAuthHandler(DatabaseService db)
    {
        _db = db;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _db.GetTokenAsync();

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
