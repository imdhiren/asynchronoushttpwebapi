using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        [CompressResponse]
        public IEnumerable<Person> Get()
        {
            return new Person[]
            {
                new Person()
                {
                    Name = "James",
                    Age = 21
                },
                    new Person()
                {
                    Name = "Jon",
                    Age = 21
                }
            };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public IEnumerable<Person> Post([FromBody]string value)
        {
            return new Person[]
    {
                new Person()
                {
                    Name = "James",
                    Age = 21
                },
                    new Person()
                {
                    Name = "Jon",
                    Age = 21
                }
    };
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
