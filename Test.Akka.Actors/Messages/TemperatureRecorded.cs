namespace Test.Akka.Actors.Messages
{
	public class TemperatureRecorded
	{
		public TemperatureRecorded(int requestId)
		{
			RequestId = requestId;
		}

		public int RequestId { get; }
	}
}
