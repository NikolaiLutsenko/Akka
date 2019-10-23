using System.Collections.Generic;

namespace Test.Akka.Actors.Messages
{
	public sealed class RespondAllTemperatures
	{
		public RespondAllTemperatures(int requestId, Dictionary<string, ITemperatureReading> temperatures)
		{
			RequestId = requestId;
			Temperatures = temperatures;
		}

		public int RequestId { get; }
		public Dictionary<string, ITemperatureReading> Temperatures { get; }
	}
}
