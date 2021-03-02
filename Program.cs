using System;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;

namespace GmailSendWithOauth
{


    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");

            AuthorizeAsync().Wait();

            Console.WriteLine("Authorized");
            Console.ReadKey();


        }

        public static async Task AuthorizeAsync()
        {
            var cred = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = "771637093207-vtia38o76pv4vjjfl9ags695sucai372.apps.googleusercontent.com",
                    ClientSecret = "eM7-k5tLFJFBqrCjHp5rjcGD"
                },
                new[] { "email", "profile", "https://mail.google.com" },
                "user",
                CancellationToken.None
                );

            var jwtPayload = GoogleJsonWebSignature.ValidateAsync(cred.Token.IdToken).Result;
            var username = jwtPayload.Email;

            // Create Gmail API service.
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = cred,
                ApplicationName = "TestGmailSendWithOauth"
            });

            CreateGmailDraft(service, username);
        }

        private static void CreateGmailDraft(GmailService service, string username)
        {

            // Create the email body
            var email = CreateEmail();
            Message message = new Message();
            message.Raw = EncodeMessage(email);

            // Create a draft of an e-mail
            Draft draft = new Draft();
            draft.Message = message;
            draft = service.Users.Drafts.Create(draft, username).Execute();

            Console.WriteLine($"Test draft created");

        }

        private static string EncodeMessage(MailMessage message)
        {
            var mimeMessage = MimeKit.MimeMessage.CreateFromMailMessage(message);

            //m.Raw = Convert.ToBase64String(Encoding.UTF8.GetBytes(msg.ToString()));
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(mimeMessage.ToString())).Replace('+', '-').Replace('/', '_').Replace("=", "");
        }

        private static MailMessage CreateEmail()
        {
            // Create the message first
            var msg = new MailMessage()
            {
                Subject = $"Zach Lifferth - TEST SCHOOL U Soccer",
                Body = "This is just a test",
                IsBodyHtml = true,
            };
            msg.To.Add(new MailAddress("coach@school.edu", "Coach Coachy"));

            return msg;
        }

    }


}
