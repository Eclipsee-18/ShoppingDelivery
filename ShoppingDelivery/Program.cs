using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace producer;
public class Program
{
	static void Main(string[] args)
	{
		

		var config = new ProducerConfig
		{
			BootstrapServers = "localhost:9092"
		};
		
		using (var producer = new ProducerBuilder<string, string>(config).Build())
		{


			while(true) 
			{
				var customerId = Console.ReadLine();
				if (customerId.Equals("q", StringComparison.OrdinalIgnoreCase))
				{
					break;
				}
				var message = new Message<string, string>
				{
					Key = customerId,
					Value = customerId
				};
				var result = producer.ProduceAsync("customer_id", message).GetAwaiter().GetResult();
				Console.WriteLine($"Produced customer Id: {result.Value} to partition {result.Partition}");
				//Console.WriteLine("CustomerId sent to Kafka!"+DateTime.Now);
				Console.Write("ENTER Q TO QUIT!\n");
				
				
			}
			//var customerId = Console.ReadLine();
			//var message = new Message<string, string>
			//{
			//	Key = customerId,
			//	Value = customerId
			//};

			//var result = producer.ProduceAsync("customer_id", message).GetAwaiter().GetResult();
			//Console.WriteLine($"Produced customer Id: {result.Value} to partition {result.Partition}");
		}
	}
}
