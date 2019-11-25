using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebAppl.Models
{
    public class AccountModel
    {
        [Required]
        [Display(Name = "Nutzername")]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "Passwort")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
      
    }
}