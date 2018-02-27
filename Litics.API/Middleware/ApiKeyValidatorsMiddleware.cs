using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Litics.API.Middleware
{
    public class ApiKeyValidatorsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string authenticationScheme = "API-LIT";
        public ApiKeyValidatorsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                var remoteIpAddress = httpContext.Connection.RemoteIpAddress;

                if (httpContext.Request.Path.StartsWithSegments("/api"))
                {
                    var req = httpContext.Request;
                    StringValues autHeader = req.Headers["Authorization"];

                    if (autHeader.Any())// && authenticationScheme.Equals(req.Scheme, StringComparison.OrdinalIgnoreCase))
                    {
                        var headerparams = autHeader.FirstOrDefault().ToString().Split(' ');
                        var scheme = headerparams[0];
                        var rawAuthzHeader = headerparams[1];

                        var autherizationHeaderArray = GetAutherizationHeaderValues(rawAuthzHeader);

                        if (autherizationHeaderArray != null)
                        {
                            var APPId = autherizationHeaderArray[0];
                            var incomingBase64Signature = autherizationHeaderArray[1];
                            var nonce = autherizationHeaderArray[2];
                            var requestTimeStamp = autherizationHeaderArray[3];

                            //var isValid = isValidRequest(req, APPId, incomingBase64Signature, nonce, requestTimeStamp);

                            //if (isValid.Result)
                            //{
                            //var currentPrincipal = new GenericPrincipal(new GenericIdentity(APPId), null);
                            //context.Principal = currentPrincipal;
                            // }
                            // else
                            //{
                            //context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                            // }
                        }



                        if (httpContext.Request.Method != "POST")
                        {
                            httpContext.Response.StatusCode = 405; //Method Not Allowed               
                            await httpContext.Response.WriteAsync("Method Not Allowed");
                            return;
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }
        private string[] GetAutherizationHeaderValues(string rawAuthzHeader)
        {

            var credArray = rawAuthzHeader.Split(':');

            if (credArray.Length == 4)
            {
                return credArray;
            }
            else
            {
                return null;
            }

        }
        /*
        private async Task<bool> isValidRequest(HttpRequestMessage req, string APPId, string incomingBase64Signature, string nonce, string requestTimeStamp)
        {
            
            string requestContentBase64String = "";
            string requestUri = HttpUtility.UrlEncode(req.RequestUri.AbsoluteUri.ToLower());
            string requestHttpMethod = req.Method.Method;

            /*if (!allowedApps.ContainsKey(APPId))
            {
                return false;
            }

            var sharedKey = allowedApps[APPId];
            
            if (isReplayRequest(nonce, requestTimeStamp))
            {
                return false;
            }

            byte[] hash = await ComputeHash(req.Content);

            if (hash != null)
            {
                requestContentBase64String = Convert.ToBase64String(hash);
            }

            string data = String.Format("{0}{1}{2}{3}{4}{5}", APPId, requestHttpMethod, requestUri, requestTimeStamp, nonce, requestContentBase64String);

            var secretKeyBytes = Convert.FromBase64String(sharedKey);

            byte[] signature = Encoding.UTF8.GetBytes(data);

            using (HMACSHA256 hmac = new HMACSHA256(secretKeyBytes))
            {
                byte[] signatureBytes = hmac.ComputeHash(signature);

                return (incomingBase64Signature.Equals(Convert.ToBase64String(signatureBytes), StringComparison.Ordinal));
            }

        }

        private bool isReplayRequest(string nonce, string requestTimeStamp)
        {
            if (System.Runtime.Caching.MemoryCache.Default.Contains(nonce))
            {
                return true;
            }

            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan currentTs = DateTime.UtcNow - epochStart;

            var serverTotalSeconds = Convert.ToUInt64(currentTs.TotalSeconds);
            var requestTotalSeconds = Convert.ToUInt64(requestTimeStamp);

            if ((serverTotalSeconds - requestTotalSeconds) > requestMaxAgeInSeconds)
            {
                return true;
            }

            System.Runtime.Caching.MemoryCache.Default.Add(nonce, requestTimeStamp, DateTimeOffset.UtcNow.AddSeconds(requestMaxAgeInSeconds));

            return false;
        }
        */

        private static async Task<byte[]> ComputeHash(HttpContent httpContent)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = null;
                var content = await httpContent.ReadAsByteArrayAsync();
                if (content.Length != 0)
                {
                    hash = md5.ComputeHash(content);
                }
                return hash;
            }
        }
    }
}

/*if (keyvalue.Count == 0)
{
    httpContext.Response.StatusCode = 400; //Bad Request                
    await httpContext.Response.WriteAsync("API Key is missing");
    return;
}
else
{
    string[] serviceName = httpContext.Request.Path.Value.Split('/');

}
}


}
await _next.Invoke(httpContext);
}
catch (Exception)
{
throw;
}
}
}

// Extension method used to add the middleware to the HTTP request pipeline.
public static class MiddlewareExtensions
{
public static IApplicationBuilder UseMiddleware(this IApplicationBuilder builder)
{
return builder.UseMiddleware<ApiKeyValidatorsMiddleware>();
}
}
}
*/
