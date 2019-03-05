using System;
using StackExchange.Redis;

namespace TextRankCalc
{
    class Program
    {
        private static ConnectionMultiplexer connection = ConnectionMultiplexer.Connect("localhost");
        private static char[] VOWELS = {'A', 'E', 'I', 'O', 'U'};
        private static char[] CONSONANTS = {
            'B', 'C', 'D', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'X', 'Y', 'Z' 
        };

        static void Main(string[] args)
        {
            var sub = connection.GetSubscriber();
            sub.Subscribe("event", (channel, message) => log(message));

            Console.ReadKey();

            void log(string message)
            {
                string id = message.Split(':')[1]; 
                IDatabase db = connection.GetDatabase();
                string value = db.StringGet(id).ToString();
                int v = 0;
                int c = 0;
                foreach (var letter in value)
                {
                    Console.WriteLine(letter);
                    if (Array.IndexOf(VOWELS, Char.ToUpper(letter)) != -1) {
                        v++;
                    } else if (Array.IndexOf(CONSONANTS, Char.ToUpper(letter)) != -1) {
                        c++;
                    }
                }
                string result = v + "/" + c;
                db.StringSet("RANK_" + id, result);
            }
        }
    }
}
