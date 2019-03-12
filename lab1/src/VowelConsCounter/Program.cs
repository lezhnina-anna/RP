using System;
using StackExchange.Redis;

namespace VowelConsCounter
{
    class Program
    {
        private static IConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
        const string QUEUE_NAME = "vowel-cons-counter-jobs";
        private static string HINTS_CHANNEL = "job_hints";
        private static string RATE_QUEUE = "vowel-cons-rater-jobs";
        private static string RATE_CHANNEL = "rate-channel";
        private static char[] VOWELS = { 'A', 'E', 'I', 'O', 'U' };
        private static char[] CONSONANTS = {
            'B', 'C', 'D', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'X', 'Y', 'Z'
        };
        static void Main(string[] args)
        {
            redis.GetSubscriber().Subscribe(HINTS_CHANNEL, delegate
           {
               string msg = redis.GetDatabase().ListRightPop(QUEUE_NAME);
               while (msg != null)
               {
                   Calc(msg);
                   msg = redis.GetDatabase().ListRightPop(QUEUE_NAME);
               }
           });

            redis.GetSubscriber().Publish(RATE_CHANNEL, "");
            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
        }

        private static void Calc(string id)
        {
            Console.WriteLine($"Job data: {id}");
            IDatabase db = redis.GetDatabase();
            string value = db.StringGet(id).ToString();
            int v = 0;
            int c = 0;
            foreach (var letter in value)
            {
                if (Array.IndexOf(VOWELS, Char.ToUpper(letter)) != -1)
                {
                    v++;
                }
                else if (Array.IndexOf(CONSONANTS, Char.ToUpper(letter)) != -1)
                {
                    c++;
                }
            }
            redis.GetDatabase().ListLeftPush(RATE_QUEUE, $"{id},{v},{c}", flags: CommandFlags.FireAndForget);
            redis.GetSubscriber().Publish(RATE_CHANNEL, "");
        }
    }
}
