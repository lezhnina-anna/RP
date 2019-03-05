using System;
using StackExchange.Redis;

namespace TextListener
{
    class Program
    {
        private static ConnectionMultiplexer connection = ConnectionMultiplexer.Connect("localhost");
        static void Main(string[] args)
        {
            var sub = connection.GetSubscriber();
            sub.Subscribe("event", (channel, message) => log(message));

            Console.ReadKey();

            void log(string message)
            {
                string id = message.Split(':')[1]; 
                IDatabase db = connection.GetDatabase();
                Console.WriteLine("id: " + id + " value: " + db.StringGet(id).ToString());
            }
        }
    }
}
