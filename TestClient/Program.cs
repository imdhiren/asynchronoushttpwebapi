using API.Models;
using APICore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static ApiClient client => new ApiClient(true);

        static void Main(string[] args)
        {
            try
            {
                Task.Run(async () =>
                {

                    var response = await client.GetJsonAsync<IEnumerable<Person>>("http://localhost:36232/Api/Values");
                    var people = response.BodyDeserialized;

                    var properties = typeof(Person).GetProperties();
                    var output = String.Join("\r\n", people.Select(x =>
                                     String.Join(",", properties.Select(f => f.GetValue(x)))
                                 ));

                    Console.WriteLine(output);

                }).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.Read();
        }
    }
}
