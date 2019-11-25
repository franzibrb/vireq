using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;

namespace WebAppl.CustomAuthentication
{
    public class CustomPrincipal : IPrincipal
    {
        /// <summary>
        /// Eigenschaft der Identify
        /// </summary>
        public int UserId { get; set; }
        public IIdentity Identity { get; private set; }

        public bool IsInRole(string role)
        {
            return true;
        }

        public CustomPrincipal(string userName, int userId)
        {
            Identity = new GenericIdentity(userName);
            UserId = userId;
        }
    }
}