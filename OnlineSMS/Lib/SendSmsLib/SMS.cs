using System;
using System.Collections.Generic;
using System.Linq;
using Infobip.Api.Client;
using Infobip.Api.Client.Api;
using Infobip.Api.Client.Model;



namespace OnlineSMS.Lib.SendSmsLib
{
    public class SMS
    {
        private readonly string BASE_URL;
        private readonly string API_KEY;

        private readonly string SENDER = "OnlineSMS";

        public SMS(IConfiguration configuration)
        {
            BASE_URL = configuration["InfobipSMS:BASE_URL"];
            API_KEY = configuration["InfobipSMS:API_KEY"];
        }
        public bool SendTo(string recipient, string code)
        {
            var message = $"Your verification code is : {code}";
            var configuration = new Configuration()
            {
                BasePath = BASE_URL,
                ApiKeyPrefix = "App",
                ApiKey = API_KEY
            };

            var sendSmsApi = new SendSmsApi(configuration);

            var smsMessage = new SmsTextualMessage()
            {
                From = SENDER,
                Destinations = new List<SmsDestination>()
                {
                    new SmsDestination(to: recipient)
                },
                Text = message
            };

            var smsRequest = new SmsAdvancedTextualRequest()
            {
                Messages = new List<SmsTextualMessage>() { smsMessage }
            };

            try
            {
                var smsResponse = sendSmsApi.SendSmsMessage(smsRequest);

                Console.WriteLine("Response: " + smsResponse.Messages.FirstOrDefault());
                return true;
            }
            catch (ApiException apiException)
            {
                Console.WriteLine("Error occurred! \n\tMessage: {0}\n\tError content", apiException.ErrorContent);
                return false;
            }
        }

    }
}
