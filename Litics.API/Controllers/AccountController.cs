using System.Threading.Tasks;
using Litics.DAL.Interfaces;
using Litics.Model;
using Litics.Model.Entites;
using Litics.Model.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Cryptography;

namespace Litics.API.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;
        private JwtIssuerOptions _jwtOptions;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly IAccountRepository _accountRepository;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 RoleManager<IdentityRole> roleManager,
                                 IOptions<JwtIssuerOptions> jwtOptions,
                                 IAccountRepository accountRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtOptions = jwtOptions.Value;
            _accountRepository = accountRepository;

            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
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
            using (var ms = new MemoryStream(2048))
            {
                await request.Body.CopyToAsync(ms);
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
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody]object model)
        {

            // if (!ModelState.IsValid)
            //            {

            var remoteIpAddress = HttpContext.Connection.RemoteIpAddress;

            if (HttpContext.Request.Path.StartsWithSegments("/api"))
            {
                var req = HttpContext.Request;
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
                        System.Console.WriteLine();

                        byte[] hash = await ComputeHash(req);
                        var requestContentBase64String = "";
                        if (hash != null)
                        {
                            requestContentBase64String = Convert.ToBase64String(hash);
                        }
                        Console.WriteLine();
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
                }
            }
            /*    string errorMsg = null;

                foreach (var test in ModelState.Values)
                {
                    foreach (var msg in test.Errors)
                    {
                        errorMsg = msg.ErrorMessage;
                    }
                }
                return BadRequest(errorMsg);
            }

            var account = await _accountRepository.GetSingle(acc => acc.Name == model.AccountName);
            var role = "";
            if (account == null)
            {
                account = new Account
                {
                    Name = model.AccountName
                };
                role = "Admin";
            }
            else
            {
                role = "User";
            }


            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Account = account,
                AccountID = account.AccountID
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var userAccount = await _userManager.FindByEmailAsync(model.Email);
                // This code can be deleted when the user must activate their account via email.
                userAccount.EmailConfirmed = true;

                // Create user role                
                var findUserRole = await _roleManager.FindByNameAsync(role);
                var userRole = new IdentityRole(role);

                // Add userAccount to a user role
                if (!await _userManager.IsInRoleAsync(userAccount, userRole.Name))
                {
                    await _userManager.AddToRoleAsync(userAccount, userRole.Name);
                }
                return new OkResult();
            }

            // If result is not successful, add error message(s)
            AddErrors(result);

            return new BadRequestObjectResult(result.Errors);*/
            //                  }
            return Ok();
        }

        #region Helpers
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        #endregion
    }
}
