using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using WebAppl.Models;

namespace WebAppl.CustomAuthentication
{
    public class CustomMembershipUser : MembershipUser
    {
        public int UserId { get; set; }
        public override string UserName { get;  }

        public CustomMembershipUser(User user) : base("CustomMembership",user.UserName, user.UserId,string.Empty,string.Empty,string.Empty,true,false,DateTime.Now,DateTime.Now,DateTime.Now,DateTime.Now,DateTime.Now)
        {

            UserId = user.UserId;
            UserName = user.UserName;
        }
    }
}