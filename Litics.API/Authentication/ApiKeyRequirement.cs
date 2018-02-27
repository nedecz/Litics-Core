using Microsoft.AspNetCore.Authorization;

namespace Litics.API.Authentication
{
    public class ApiKeyRequirement: IAuthorizationRequirement
    {
        public ApiKeyRequirement()
        {

        }
    }
}
