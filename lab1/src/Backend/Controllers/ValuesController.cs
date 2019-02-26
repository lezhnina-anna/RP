using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using StackExchange.Redis;

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
            string value = null;
            _data.TryGetValue(id, out value);
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
            pub.Publish("TextCreated", id);
            RedisValue res = db.StringGet(id);
            _data[id] = value;
            return id;
        }
    }
}
