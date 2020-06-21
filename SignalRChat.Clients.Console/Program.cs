using SignalRChat.Shared;
using System.Threading.Tasks;

namespace SignalRChat.Clients.Console
{
   class Program
   {
      static async Task Main(string[] args)
      {
         try
         {
            System.Console.Write("Enter your name: ");
            var username = System.Console.ReadLine();

            var chatClient = new ChatClient(username, "http://localhost:5000");
            chatClient.MessageReceived += ChatClient_MessageReceived;
            chatClient.NewClientNotification += ChatClient_NewClientNotification;
            await chatClient.StartAsync();

            System.Console.WriteLine("Connect success. Start to chat\nTo exit press \'exit\'");
            string message = string.Empty;
            do
            {
               message = System.Console.ReadLine();
               await chatClient.SendAsync(message);
            } while (message != "exit");
         }
         catch (System.Exception e)
         {
            System.Console.WriteLine("Error: {0}", e.Message);
         }
      }

      private static void ChatClient_NewClientNotification(object sender, string e)
      {
         System.Console.WriteLine("[{0}] joined the chat", e);
      }

      private static void ChatClient_MessageReceived(object sender, MessageReceivedEventArgs e)
      {
         System.Console.WriteLine("[{0}]: {1}", e.UserName, e.Message);
      }
   }
}
