using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Request.Account
{
    public class LoginDTO
    {
        [EmailAddress, Required,DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        [RegularExpression("^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+[a-zA-Z]{2,}))$$", ErrorMessage = "your email is not valid , provied a valid email such as @gmail , @hotmail etc")]
        public string Email { get; set; }
        [Required]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?\\d)(?=.*?[#?!@$%^&*()\\-+=])(?=.{8,})$", ErrorMessage = "your password must be mix of Alphanumeric and special chrachters")]
        public string Password { get; set; } = string.Empty; 
    }
}
