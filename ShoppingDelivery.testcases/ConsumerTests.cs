using Confluent.Kafka;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using consumer;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data.SQLite;

namespace ShoppingDelivery.testcases
{
	public class ConsumerTests
	{

		[Fact]
		public void TestGetUserInfo_returnString()
		{
			var connectionString = "Data Source=:memory:";
			using (var connection = new SQLiteConnection(connectionString))
			{
				
				connection.Open();
				var createTable = "CREATE TABLE my_table (Email TEXT, Password TEXT)";
				using (var command = new SQLiteCommand(createTable, connection))
				{
					command.ExecuteNonQuery();
				}

				var insertData = "INSERT INTO my_table (Email, Password) VALUES ('testuser@example.com','testpassword123')";
				using (var command = new SQLiteCommand(insertData, connection))
				{
					command.ExecuteNonQuery();
				}

				
				string userInfo = null;
				var selectCommand = connection.CreateCommand();
				selectCommand.CommandText = "SELECT * FROM my_table";
				using (var reader = selectCommand.ExecuteReader())
				{
					reader.Read();
					var data = new
					{
						Email = reader["Email"],
						Password = reader["Password"]
					};
					userInfo = "{\"email\":" + '"' + data.Email + '"' + "," + "\"password\":" + '"' + data.Password + '"' + "}";
				}

				// Assert
				Assert.Equal("{\"email\":\"testuser@example.com\",\"password\":\"testpassword123\"}", userInfo);

			}
		}


		[Fact]
		public void TestPostData()
		{
			// Arrange
			string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJKV1RTZXJ2aWNlQWNjZXNzVG9rZW4iLCJqdGkiOiJkOTU3NTI1My00MDA2LTRiNzItOTM0OS1mYmE0MDliMTdiMGYiLCJpYXQiOiIyLzE1LzIwMjMgMTE6MjU6MDggQU0iLCJVc2VySWQiOiIxIiwiRGlzcGxheU5hbWUiOiJBZG1pbiIsIlVzZXJOYW1lIjoiYWRtaW4iLCJFbWFpbCI6ImFkbWluQGFiYy5jb20iLCJleHAiOjE2NzY0NjA5MDgsImlzcyI6IkpXVEF1dGhlbnRpY2F0aW9uU2VydmVyIiwiYXVkIjoiSldUU2VydmljZVBvc3RtYW5DbGllbnQifQ.VUZJ9jZTF19-umqMoz-g2texA6WYPVqV3UdOVCgIjeo";
			var data1 = new { OrderId = 1, Name="John",Product="shirt",City="London",Zip="1000" };
			var data2 = new { };
			var expected1 = "\nData sent successfully.";
			var expected2 = "\nFailed to send data.";
			// Act
			var output1=consumer.Program.postdata(data1,token);
			var output2 = consumer.Program.postdata(data2,token);
			// Assert

			Assert.Contains(expected1, output1);
			Assert.Contains(expected2, output2);
		}
	}
}