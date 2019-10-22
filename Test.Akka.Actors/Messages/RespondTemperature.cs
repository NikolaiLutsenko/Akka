namespace Test.Akka.Actors.Messages
{
	public class RespondTemperature
	{
		public RespondTemperature(int requestId, double? value)
		{
			RequestId = requestId;
			Value = value;
		}

		public int RequestId { get; }
		public double? Value { get; }
	}
}
