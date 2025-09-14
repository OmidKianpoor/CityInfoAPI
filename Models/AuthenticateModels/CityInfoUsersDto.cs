using System.ComponentModel.Design.Serialization;

namespace CityInfo.API.Models.AuthenticateModels
{
    public class CityInfoUsersDto
    {
       

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string UserEmail { get; set; }

        public string UserPhone { get; set; }
        public string Password { get; set; }
        public string City { get; set; }

        public CityInfoUsersDto
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
