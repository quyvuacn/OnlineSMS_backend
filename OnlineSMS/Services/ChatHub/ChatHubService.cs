using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR;
using OnlineSMS.Models;
using OnlineSMS.RequestModels;
using Microsoft.AspNetCore.Http.Connections;
using OnlineSMS.Data;

namespace OnlineSMS.Services.ChatHub
{
    public class ChatHubService
    {

        private readonly IHubContext<Controllers.Hub> hubContext;
        private readonly IConfiguration configuration;
        private readonly OnlineSMSContext onlineSMSContext;

        public ChatHubService(IHubContext<Controllers.Hub> hubContext, IConfiguration configuration, OnlineSMSContext onlineSMSContext)
        {
            this.hubContext = hubContext;
            this.configuration = configuration;
            this.onlineSMSContext = onlineSMSContext;
        }

        public async Task<RequestResult> ConnectToChatHub(string userId)
        {
            
            try
            {
                string baseUrlChatHub = configuration["ChatHubUrl"];

                var connection = new HubConnectionBuilder().WithUrl(baseUrlChatHub, 
                    options => {
                        options.Transports = HttpTransportType.WebSockets;
                    }).WithAutomaticReconnect().Build();

                await connection.StartAsync();

                var connectionId = connection.ConnectionId;

                UserConnection userConnection = new UserConnection
                {
                    ConnectionId = connectionId,
                    UserId = userId,
                };

                onlineSMSContext.UserConnection.Add(userConnection);

                await onlineSMSContext.SaveChangesAsync();

                await connection.InvokeAsync("Hello", connectionId, connectionId);

                return new RequestResult
                {
                    IsSuccess = true,
                    Data = connectionId
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new RequestResult
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
            

           

            


        }

    }
}
