namespace Test.Akka.Actors.Messages
{
	public class ReadTemperature
	{
		public ReadTemperature(int requestId)
		{
			RequestId = requestId;
		}

		public int RequestId { get; }
	}
}
