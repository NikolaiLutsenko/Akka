namespace Test.Akka.Actors.Messages
{
	public sealed class RequestAllTemperatures
	{
		public RequestAllTemperatures(int requestId)
		{
			RequestId = requestId;
		}

		public int RequestId { get; }
	}
}
