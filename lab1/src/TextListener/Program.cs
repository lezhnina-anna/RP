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
            sub.Subscribe("TextCreated", (channel, message) => ProcessMessage(message));

             while (true) {
             }

            void ProcessMessage(string id)
            {
                IDatabase db = connection.GetDatabase();
                Console.WriteLine("id: " + id + " value: " + db.StringGet(id).ToString());
            }
        }
    }
}
