using System;
using NetMQ;
using NetMQ.Monitoring;
using NetMQ.Sockets;

namespace RouterSocketTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var routerSocket = new RouterSocket();
            var endpoint = "tcp://0.0.0.0:49152";
            var monitor = new NetMQMonitor(routerSocket, endpoint);

            routerSocket.Bind(endpoint);
            Console.WriteLine($"Router socket bound to {endpoint}");

            NetMQMessage message = new NetMQMessage();
            while (true)
            {
                PrintMenu();
                Console.WriteLine("Enter your selection: ");
                var selection = int.Parse(Console.ReadLine());
                Console.WriteLine();

                switch (selection)
                {
                    case 1:
                        SendMessage(routerSocket);
                        break;
                    case 2:
                        message = WaitForMessage(routerSocket);
                        var itr = message.GetEnumerator();
                        Console.WriteLine(
                            string.Format("{0} greeting frame", itr.MoveNext() ? "found" : "not found"));
                        Console.WriteLine(
                            string.Format("Message: {0}", itr.Current.ConvertToString()));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };
        }

        private static void PrintMenu()
        {
            Console.WriteLine("Select an option:");
            Console.WriteLine();
            Console.WriteLine("1: Send a message");
            Console.WriteLine("2: Wait for a message");
            Console.WriteLine();
        }

        private static void SendMessage(RouterSocket socket)
        {
            var message = new NetMQMessage();
            Console.WriteLine("Type the message you want to send: ");
            var body = Console.ReadLine();
            message.Append(body);

            socket.SendMultipartMessage(message);
            Console.WriteLine();
            Console.WriteLine("Sent message to router socket");
            Console.WriteLine();
        }

        private static NetMQMessage WaitForMessage(RouterSocket socket)
        {
            Console.WriteLine("Waiting on message from router socket");
            return socket.ReceiveMultipartMessage();
        }

        private void OnDisconnected(NetMQMonitorErrorEventArgs e)
        {
            Console.WriteLine("Disconnected");
        }
    }
}
