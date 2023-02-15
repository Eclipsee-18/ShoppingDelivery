using Confluent.Kafka;
using System.Data.SqlClient;
using static Confluent.Kafka.ConfigPropertyNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System.Net.Http.Headers;
using System.ComponentModel;

namespace consumer;
public class Program
{
	private ConsumerConfig config;

	public Program(ConsumerConfig config)
	{
		this.config = config;
	}

	static void Main(string[] args)
	{
		var config = new ConsumerConfig
		{
			BootstrapServers = "localhost:9092",
			GroupId = "customer_metadata_group",
			AutoOffsetReset = AutoOffsetReset.Earliest
		};

		var token=getToken();
		

		using (var consumer = new ConsumerBuilder<string, string>(config).Build())
		{
			consumer.Subscribe("customer_id");

			while (true)
			{
				var message = consumer.Consume();
				Console.WriteLine($"Customer Id: {message.Value}");
				Console.WriteLine("\nCustomer-ID recieved from producer! ["+DateTime.Now+"]");
			

				using (SqlConnection connection = new SqlConnection("Server=ECLIPSE;Database=Test;Trusted_Connection=True;TrustServerCertificate=True;"))
				{
					connection.Open();
					
					string customerId = message.Value;

					string query = "SELECT * FROM metadata WHERE OrderId = @CustomerId";

					using (SqlCommand command = new SqlCommand(query, connection))
					{
						command.Parameters.AddWithValue("@CustomerId", customerId);

						using (SqlDataReader reader = command.ExecuteReader())
						{
							var read=reader.Read();
							Console.WriteLine("\nFetching Data from Database!");
							if (read)
							{
								//while (read)
								//{
									Console.WriteLine("Name: " + reader["Name"]);
									Console.WriteLine("Product: " + reader["Product"]);
									Console.WriteLine("City: " + reader["City"]);
									Console.WriteLine("Zip: " + reader["Zip"]);

									var data = new
									{
										OrderId = reader["OrderId"],
										Name = reader["Name"],
										Product = reader["Product"],
										City = reader["City"],
										Zip = reader["Zip"].ToString()
									};
									Console.WriteLine("\nData fetched! ["+DateTime.Now+"]");
								 
									Console.WriteLine(postdata(data,token));
								//}
							}
							else { Console.WriteLine("Data Unavailable for this Customer Id"); }
						}
					}

				}
			}
		}
	}

	public static string getUserInfo()
	{
		string user = null;
		using (SqlConnection connection = new SqlConnection("Server=ECLIPSE;Database=PostData;Trusted_Connection=True;TrustServerCertificate=True;"))
		{
			connection.Open();
			string query = "SELECT * FROM UserInfo";
			using(SqlCommand command= new SqlCommand(query, connection))
			{
				using(SqlDataReader reader = command.ExecuteReader())
				{
					reader.Read();
					var data = new
					{
						Email = reader["Email"],
						Password = reader["Password"]
					};
					user= "{\"email\":"+'"'+data.Email+'"'+","+"  \"password\": "+'"'+data.Password+'"'+"}";
				}
			}
		}
		return user;
	}

	public static string getToken()
	{
		var data = getUserInfo().ToString();
		//string data = "{\"email\": \"admin@abc.com\", \"password\": \"admin123\"}";
		var json = JsonConvert.SerializeObject(data);
		var content = new StringContent(data, Encoding.UTF8, "application/json");

		using (var client = new HttpClient())
		{

			client.BaseAddress = new Uri("https://localhost:44359/");
			
			var response = client.PostAsync("api/Token", content);
			var body=response.Result.Content.ReadAsStringAsync().Result;
			return body;
		}

	}

	public static string postdata(object data,string token)
	{
		//var bearerToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJKV1RTZXJ2aWNlQWNjZXNzVG9rZW4iLCJqdGkiOiI4MjEwM2UyZS1lYWVkLTQwNDItYTg5NS1hYTU1MWMzYjg3NjciLCJpYXQiOiIyLzE0LzIwMjMgMTE6NTE6MDIgQU0iLCJVc2VySWQiOiIxIiwiRGlzcGxheU5hbWUiOiJBZG1pbiIsIlVzZXJOYW1lIjoiYWRtaW4iLCJFbWFpbCI6ImFkbWluQGFiYy5jb20iLCJleHAiOjE2NzYzNzYwNjIsImlzcyI6IkpXVEF1dGhlbnRpY2F0aW9uU2VydmVyIiwiYXVkIjoiSldUU2VydmljZVBvc3RtYW5DbGllbnQifQ.loiBlmtwW0-VM2WtgcruW34IQOJPwukXWn4Snhy6SuQ";
		var json = JsonConvert.SerializeObject(data);
		var content = new StringContent(json, Encoding.UTF8, "application/json");

		using (var client = new HttpClient())
		{

			client.BaseAddress = new Uri("https://localhost:44359/");
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			var response = client.PostAsync("api/CustomerData", content).Result;

			if (response.IsSuccessStatusCode)
			{
				return "\nData sent successfully. [" + DateTime.Now + "]\n";
				//Console.WriteLine();
			}
			else
			{
				return "\nFailed to send data.";
				//Console.WriteLine();
			}
		}

	}

	

}









