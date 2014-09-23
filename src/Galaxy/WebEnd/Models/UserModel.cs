using System.ComponentModel.DataAnnotations;
using Codestellation.Galaxy.Domain;
using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd.Models
{
    public class UserModel
    {
        public UserModel()
        {
            Id = new ObjectId().ToString();
            IsNew = true;
        }
        public UserModel(User user)
        {
            Id = user.Id.ToString();
            IsAdmin = user.IsAdmin;
            Login = user.Login;

            IsNew = false;
        }

        public string Id { get; set; }

        [Display(Name = "Login", Prompt = "Login")]
        public string Login { get; set; }

        [Display(Name = "User administrator")]
        public bool IsAdmin { get; set; }

        public bool IsNew { get; set; }

        public User ToUser()
        {
            var user = new User
                       {
                           Id = new ObjectId(Id)
                       };

            Update(user);
            return user;
        }

        public void Update(User user)
        {
            user.IsAdmin = IsAdmin;
            user.Login = Login;
        }
    }
}