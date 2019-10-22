using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Akka.Actors.Messages
{
	public class RecordTemperature
	{
		public RecordTemperature(int requestId, double value)
		{
			RequestId = requestId;
			Value = value;
		}

		public int RequestId { get; }
		public double Value { get; }
	}
}
