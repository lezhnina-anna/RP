using System;
using StackExchange.Redis;

namespace VowelConsRater
{
    class Program
    {
        private static IConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
        private static string RATE_QUEUE = "vowel-cons-rater-jobs";
        private static string RATE_CHANNEL = "rate-channel";
        static void Main(string[] args)
        {
            redis.GetSubscriber().Subscribe(RATE_CHANNEL, delegate
            {
                string msg = redis.GetDatabase().ListRightPop(RATE_QUEUE);
                while (msg != null)
                {
                    string[] parsedMsg = parseMsg(msg);
                    Calc(parsedMsg[0], Convert.ToDouble(parsedMsg[1]), Convert.ToDouble(parsedMsg[2]));
                    msg = redis.GetDatabase().ListRightPop(RATE_QUEUE);
                }
            });
            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
        }

        private static String[] parseMsg(string msg)
        {
            return msg.Split(',');
        }
        private static void Calc(string id, double vowelNum, double consNum)
        {
            IDatabase db = redis.GetDatabase();
            if (consNum == 0)
            {
                db.StringSet("RANK_" + id, "0");
            }
            double result = vowelNum / consNum;
            Console.WriteLine(result.ToString());
            db.StringSet("RANK_" + id, result.ToString());
        }
    }
}
