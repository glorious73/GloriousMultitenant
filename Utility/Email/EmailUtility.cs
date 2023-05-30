using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Email
{
    public class EmailUtility : IEmailUtility
    {
        private IConfiguration _configuration;
        // Mail Jet
        public MailJetClient MailJetClient { get; set; }
        public string From { get; set; }

        public EmailUtility(IConfiguration configuration)
        {
            _configuration = configuration;
            MailJetClient = _configuration.GetSection("MailJetClient").Get<MailJetClient>();
            From = MailJetClient.EmailAddress;
        }

        public async Task<bool> SendMailJetEmail(string to, string subject, string body)
        {
            MailjetClient client = new MailjetClient(MailJetClient.MJ_APIKEY_PUBLIC, MailJetClient.MJ_APIKEY_PRIVATE)
            {
                Version = ApiVersion.V3_1
            };

            var ToArray = new JArray();
            if (to.Contains(";"))
            {
                foreach (var emailto in to.Split(";"))
                    ToArray.Add(new JObject {
                        { "Email", emailto }
                    });
            }
            else
                ToArray.Add(new JObject {
                    { "Email", to }
                });

            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }.Property(Send.Messages, new JArray {
                new JObject {
                     { "From",
                           new JObject {
                               { "Email", MailJetClient.EmailAddress },
                               { "Name", MailJetClient.Name }
                           }
                     },
                     { "To", ToArray
                     },
                     {
                        "Subject", subject
                     },
                     {
                        "HTMLPart", body
                     }
                 }
            });
            // Send Email
            MailjetResponse response = await client.PostAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}
