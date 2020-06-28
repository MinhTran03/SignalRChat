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

            var chatClient = new ChatClient(username, HubConstant.HubUrl);
            chatClient.MessageReceived += ChatClient_MessageReceived;
            chatClient.NotificationStateChange += ChatClient_NotificationStateChange;
            chatClient.NotificationAddedToGroup += ChatClient_NotificationAddedToGroup;
            await chatClient.StartAsync();

            System.Console.WriteLine("Connect success. Start to chat\nTo exit press \'exit\'");
            string message = string.Empty;
            do
            {
               System.Console.Write("[You]: ");
               message = System.Console.ReadLine();
               if(message != "exit") await chatClient.SendAsync(message);
            } while (message != "exit");
            await chatClient.StopAsync();
         }
         catch (System.Exception e)
         {
            System.Console.WriteLine("Error: {0}", e.Message);
         }
      }

      private static void ChatClient_NotificationAddedToGroup(object sender, NotifyAddedToGroupEventArgs e)
      {
         ClearCurrentConsoleLine();
         System.Console.WriteLine("You had added to group {0} by {1}",
            e.GroupIdentity.ToString(), e.ClientAdded.ToString());
         System.Console.Write("[You]: ");
      }

      private static void ChatClient_NotificationStateChange(object sender, NotifyStateEventArgs e)
      {
         ClearCurrentConsoleLine();
         if(e.State == State.Register)
            System.Console.WriteLine("[{0}] has join the chat", e.ClientIdentity.Username);
         else
            System.Console.WriteLine("[{0}] disconnected", e.ClientIdentity.Username);
         System.Console.Write("[You]: ");
      }

      private static void ChatClient_MessageReceived(object sender, MessageReceivedEventArgs e)
      {
         ClearCurrentConsoleLine();
         System.Console.WriteLine("[{0}]: {1}", e.ClientIdentity.Username, e.Message);
         System.Console.Write("[You]: ");
      }

      public static void ClearCurrentConsoleLine()
      {
         int currentLineCursor = System.Console.CursorTop;
         System.Console.SetCursorPosition(0, System.Console.CursorTop);
         System.Console.Write(new string(' ', System.Console.WindowWidth));
         System.Console.SetCursorPosition(0, currentLineCursor);
      }
   }
}
