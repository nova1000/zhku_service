using System.ComponentModel.DataAnnotations;
namespace WebApplication1.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Не указан Email")]
        public string login { get; set; }
         
        [Required(ErrorMessage = "Не указан пароль")]
        public string password { get; set; }
    }
}