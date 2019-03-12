using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using StackExchange.Redis;
using System.Threading;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        static readonly ConcurrentDictionary<string, string> _data = new ConcurrentDictionary<string, string>();
        static private ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");

        // GET api/values/<id>
        [HttpGet("{id}")]
        public string Get(string id)
        {
            string value = "null";
            IDatabase db = redis.GetDatabase();
            const int maxTries = 5;
            var key = "RANK_" + id;
            for (var i = 0; i < maxTries; i++)
            {
                if (db.KeyExists(key) && db.KeyExists(id))
                {
                    value = db.StringGet(key) + "_" + db.StringGet(id);
                    break;
                }
                else
                    Thread.Sleep(500);
            }
            return value;
        }

        // POST api/values
        [HttpPost]
        public string Post([FromBody]string value)
        {
            var id = Guid.NewGuid().ToString();
            IDatabase db = redis.GetDatabase();
            db.StringSet(id, value);
            var pub = redis.GetSubscriber();
            string message = "TEXT_CREATED:" + id;
            pub.Publish("event", message);
            _data[id] = value;
            return id;
        }
    }
}
