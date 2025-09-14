namespace CityInfo.API.Services.MailServices
{
    public class CloudMailService : IMailService
    {
       private readonly string _mailTo = string.Empty;
       private readonly string _mailFrom = string.Empty;
        public CloudMailService(IConfiguration configuration)
        {
            _mailTo = configuration["MailSetting:MailTo"];       
            _mailFrom = configuration["MailSetting:MailFrom"];
        }


        public void Send(string subject, string message)
        {
            Console.WriteLine($"Mail From {_mailFrom} To {_mailTo}  With {nameof(CloudMailService)}");
            Console.WriteLine($"Subject : {subject}\n Message : {message}");
           
        }

      
        #region Example Of Real Send Method 
        //public static void Email(string htmlstring,string subject, string to)
        //{
        //    try
        //    {
        //        MailMessage message = new MailMessage();
        //        SmtpClient smtp = new SmtpClient();
        //        message.From = new MailAddress("_mailFrom");
        //        message.To.Add(new MailAddress("To"));
        //        message.Subject = subject;
        //        message.IsBodyHtml = true; //to make message body as html
        //        message.Body = htmlstring;
        //        smtp.Port = 587;
        //        smtp.Host = "smtp.Gmail.com"; //for gmail host
        //        smtp.EnableSsl = true;
        //        smtp.UseDefaultCredentials = false;
        //        smtp.Credentials = new NetworkCredential("FromMailAdress", "passWord");
        //        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        //        smtp.Send(message);

        //    }
        //    catch (Exception) { }
        //}
        #endregion
    }
}
