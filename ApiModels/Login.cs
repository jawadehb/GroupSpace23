using System.ComponentModel.DataAnnotations;

namespace GroupSpace23.ApiModels
{
    public class LoginModel
    {
        [Key]
        public int? Id {get; set;}
        public string Name { get; set; }
        public string Password { get; set; }
       
    }
}