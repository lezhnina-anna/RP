using System;
using StackExchange.Redis;

namespace TextRankCalc
{
    class Program
    {
        private static ConnectionMultiplexer connection = ConnectionMultiplexer.Connect("localhost");
        const string JOBS_QUEUE_NAME = "vowel-cons-counter-jobs";
        const string JOB_HINTS_CHANNEL = "job_hints";

        static void Main(string[] args)
        {
            var sub = connection.GetSubscriber();
            sub.Subscribe("event", (channel, message) => log(message));

            Console.ReadKey();

            void log(string message)
            {
                string id = message.Split(':')[1];
                SendMessage(id, connection);
            }
        }

        private static void SendMessage(string message, IConnectionMultiplexer redis)
        {
            redis.GetDatabase().ListLeftPush(JOBS_QUEUE_NAME, message, flags: CommandFlags.FireAndForget);
            redis.GetSubscriber().Publish(JOB_HINTS_CHANNEL, "");
        }
    }
}
