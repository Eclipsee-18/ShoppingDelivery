using Confluent.Kafka;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using consumer;
using System.IO;

namespace ShoppingDelivery.testcases
{
	public class ConsumerTests
	{
		[Fact]
		public void TestPostData()
		{
			// Arrange
			var data1 = new { OrderId = 1, Name="John",Product="shirt",City="London",Zip="1000" };
			var data2 = new { };
			var expected1 = "\nData sent successfully. [" + DateTime.Now + "]\n";
			var expected2 = "\nFailed to send data.";
			// Act
			var output1=consumer.Program.postdata(data1);
			var output2 = consumer.Program.postdata(data2);
			// Assert

			Assert.Contains(expected1, output1);
			Assert.Contains(expected2, output2);
		}
	}
}