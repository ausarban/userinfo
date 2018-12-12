using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private const string baseUrl = "https://jsonplaceholder.typicode.com/";

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<List<UserRecord>> GetUserRecords(List<User> user)
        {
            var records = new List<UserRecord>();

            try
            {
                var client = new HttpClient();

                client.BaseAddress = new Uri(baseUrl + "posts?userid=" + user[0].ID);
                var response = await client.GetAsync(client.BaseAddress);

                response.EnsureSuccessStatusCode();

                var userRecords = await response.Content.ReadAsStringAsync();

                records = JsonConvert.DeserializeObject<List<UserRecord>>(userRecords);

                client.Dispose();
                return records;
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("An Exception has occured, Application is stopping with... {0}", e.Message));
               
                return null;
            }

        }
        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

    }

    public class User : UserRecord
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public Address Address { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public Company Company { get; set; }

    }
    public class Address
    {
        public string Street { get; set; }
        public string Suite { get; set; }
        public string City { get; set; }
        public string Zipcode { get; set; }
        public Geo Geo { get; set; }

    }
    public class Geo
    {
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }
    }
    public class Company
    {
        public string Name { get; set; }
        public string CatchPhrase { get; set; }
        public string Bs { get; set; }

    }
    public class UserRecord
    {
        public int UserID { get; set; }
        public int ID { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}
