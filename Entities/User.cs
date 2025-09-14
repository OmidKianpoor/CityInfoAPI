using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfo.API.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Required]
        [MaxLength(200)]
        public string UserName { get; set; }
        public string Password { get; set; }
        [MaxLength(50)]
        public string FirstName { get; set; }
        [MaxLength(50)]
        public string LastName { get; set; }
        [Required]
        [MaxLength(50)]
        public string UserEmail { get; set; }
        [MaxLength(16)]
        public string UserPhone { get; set; }

        public string City { get; set; }
        public bool IsActive { get; set; } = false;
        public string Token { get; set; } = "";

        public User
            ( string userName, string firstName,
            string lastName, string userEmail, string userPhone, string city,string password)
        {
            
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            UserEmail = userEmail;
            UserPhone = userPhone;
            City = city;
            Password = password;
        }
    }
}
