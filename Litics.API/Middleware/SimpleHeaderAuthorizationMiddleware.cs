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
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Internal;

namespace Litics.API.Middleware
{
    public class SimpleHeaderAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string authenticationScheme = "API-LIT";
        public SimpleHeaderAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var remoteIpAddress = context.Connection.RemoteIpAddress;
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                var req = context.Request;
                StringValues autHeader = req.Headers["Authorization"];

                if (autHeader.Any())// && authenticationScheme.Equals(req.Scheme, StringComparison.OrdinalIgnoreCase))
                {
                    var headerparams = autHeader.FirstOrDefault().ToString().Split(' ');
                    var scheme = headerparams[0];
                    if (string.CompareOrdinal(scheme, authenticationScheme) == 0)
                    {
                        var rawAuthzHeader = headerparams[1];

                        var autherizationHeaderArray = GetAutherizationHeaderValues(rawAuthzHeader);

                        if (autherizationHeaderArray != null)
                        {
                            var APPId = autherizationHeaderArray[0];
                            var incomingBase64Signature = autherizationHeaderArray[1];
                            var nonce = autherizationHeaderArray[2];
                            var requestTimeStamp = autherizationHeaderArray[3];
                            byte[] hash = await ComputeHash(req);
                            var requestContentBase64String = "";
                            if (hash != null)
                            {
                                requestContentBase64String = Convert.ToBase64String(hash);
                            }
                            Console.WriteLine();
                            var isValid = isValidRequest(req, APPId, incomingBase64Signature, nonce, requestTimeStamp);

                            if (isValid.Result)
                            {
                                await _next.Invoke(context);
                                return;
                            }
                            else
                            {
                                context.Response.StatusCode = 405; //Method Not Allowed               
                                await context.Response.WriteAsync("Method Not Allowed");
                                return;
                            }
                        }
                    }
                        context.Response.StatusCode = 405; //Method Not Allowed               
                        await context.Response.WriteAsync("Method Not Allowed");
                        return;
                }
            }
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
        public static async Task<byte[]> GetRawBodyBytesAsync(HttpRequest request)
        {

            var inputStream = request.Body;
            inputStream.Position = 0;

            using (var ms = new MemoryStream(2048))
            {
                await inputStream.CopyToAsync(ms);
                return ms.ToArray();
            }
        }
        private static async Task<byte[]> ComputeHash(HttpRequest request)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = null;
                var content = await GetRawBodyBytesAsync(request);
                if (content.Length != 0)
                {
                    hash = md5.ComputeHash(content);
                }
                return hash;
            }
        }

        private async Task<bool> isValidRequest(HttpRequest req, string APPId, string incomingBase64Signature, string nonce, string requestTimeStamp)
        {
            //string APPId = "34edce2f-12f9-4d73-ac9a-45763a87bb16";
            string APIKey = "AuuwfNQYlyYO5YmUm/scN6WdvIkTzPeYQgVELQu/3rg=";


            string requestContentBase64String = "";
            string requestUri = req.GetEncodedUrl().ToLower(); //HttpUtility.UrlEncode(req.RequestUri.AbsoluteUri.ToLower());
            string requestHttpMethod = req.Method;

            /*if (!allowedApps.ContainsKey(APPId))
            {
                return false;
            }*/

            var sharedKey = APIKey;//allowedApps[APPId];

            //if (isReplayRequest(nonce, requestTimeStamp))
            //{
            //    return false;
            //}

            byte[] hash = await ComputeHash(req);

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

        /*private bool isReplayRequest(string nonce, string requestTimeStamp)
        {
            if (MemoryCache.Default.Contains(nonce))
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
        }*/
    }

    public static class SimpleHeaderAuthorizationMiddlewareExtension
    {
        public static IApplicationBuilder UseSimpleHeaderAuthorization(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<SimpleHeaderAuthorizationMiddleware>();
        }
    }
    public class SimpleHeaderAuthorizationPipeline
    {
        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseSimpleHeaderAuthorization();
        }
    }
}

