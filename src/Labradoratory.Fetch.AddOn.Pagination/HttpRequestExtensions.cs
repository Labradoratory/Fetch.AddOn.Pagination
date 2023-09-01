using System;
using Microsoft.AspNetCore.Http;

namespace Labradoratory.Fetch.AddOn.Pagination
{
    public static class HttpRequestExtensions
    {
        public static Uri GetBaseUri(this HttpRequest request)
        {
            return new Uri($"{request.Scheme}://{request.Host}/{request.PathBase}");
        }
    }
}
