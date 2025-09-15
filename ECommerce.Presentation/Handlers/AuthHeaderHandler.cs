using System.Net.Http.Headers;
using ECommerce.Presentation.Interfaces.Auth;

namespace ECommerce.Presentation.Handlers;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly IAuthTokenHolder _tokenHolder;

    public AuthHeaderHandler(IAuthTokenHolder tokenHolder)
    {
        _tokenHolder = tokenHolder;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(_tokenHolder.Token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenHolder.Token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}