using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace UserRecords
{
    class Program
    {
        private const string baseUrl = "https://jsonplaceholder.typicode.com/";
        private static readonly ReadOnlyCollection<int> validIds = new ReadOnlyCollection<int>(new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
        static void Main(string[] args)
        {
            Program program = new Program();
            Console.WriteLine("Please enter a user id (1-10) to retrieve user records...");

            int id = 0;
            
            try
            {
                while (!validIds.Contains(id))
                {
                    if (!int.TryParse(Console.ReadLine(), out id))
                        Console.WriteLine("Please enter a valid user id between 1 and 10");
                }

                List<User> user = program.GetUser(id).GetAwaiter().GetResult();

                Console.WriteLine(string.Format("retrieving records for user id {0}...", id));

                Console.WriteLine();

                List<UserRecord> users = program.GetUserRecords(user).GetAwaiter().GetResult();

                for (var i = 0; i < users.Count; i++) //Display as many records are found.
                    Console.WriteLine(string.Format(" {0} - {1} ", users[i].Name, users[i].Body));

                Console.ReadLine();

            }catch(Exception ex) //We would normally log the exception here and handle it accordingly.
            {
                Console.WriteLine(string.Format("Something went wrong... Application is stopping for...{0}", ex.Message)); 
                Task.Delay(5000);
                return;
            }
           
        }

        public async Task<List<User>> GetUser(int id)
        {
            var user = new List<User>(); //User needs to be a list because the structure of the JSON is an array.

            try
            {
                var client = new HttpClient(); //we wont need any headers here
                client.BaseAddress = new Uri(baseUrl + "users?id=" + id); // pass the parameter to get our user record 
                var response = await client.GetAsync(client.BaseAddress); 

                response.EnsureSuccessStatusCode(); //throws an exception if API isn't happy with request.

                string record = await response.Content.ReadAsStringAsync();
                user = JsonConvert.DeserializeObject<List<User>>(record); //Json is coming in as an array so let's deserialize to a list.

                client.Dispose(); // dispose the resources after our client is done here. 
                return user;
            }
            catch (Exception e) 
            {
                Console.WriteLine(string.Format("An Exception has occured... {0}", e.Message));
                return null;
            }
        }

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

                for (var i = 0; i < records.Count; i++)
                    records[i].Name = user[0].Name;

                client.Dispose();
                return records;
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("An Exception has occured... {0}", e.Message));

                return null;
            }

        }
    }
 

    public class User
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
    public class UserRecord : User
    {
        public int UserID { get; set; }
        public int ID { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }

}
