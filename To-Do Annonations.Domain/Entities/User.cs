using System.ComponentModel.DataAnnotations;

namespace To_Do_Annonations.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The Username is required")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "The password is required")]
        public string Password { get; set; } = string.Empty;
    }
}
