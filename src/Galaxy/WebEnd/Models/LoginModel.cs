using System.ComponentModel.DataAnnotations;

namespace Codestellation.Galaxy.WebEnd.Models
{
    public class LoginModel
    {
        [Display(Name = "Логин", Prompt = "Логин")]
        public string Login { get; set; }

        [Display(Name = "Пароль", Prompt = "Пароль")]
        public string Password { get; set; }

        public LoginModel(string login)
        {
            Login = login;
        }
    }
}
