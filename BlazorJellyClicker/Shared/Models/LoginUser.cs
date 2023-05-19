using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlazorJellyClicker.Shared.Models
{
    public class LoginUser
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Jwt { get; set; }
        public ClaimsPrincipal claimsPrincipal { get; set; }
        public LoginUser(string _username, string _jwt, ClaimsPrincipal _claims)
        {
            UserName = _username;
            DisplayName = _username;
            Jwt = _jwt;
            claimsPrincipal = _claims;
        }
    }
}
