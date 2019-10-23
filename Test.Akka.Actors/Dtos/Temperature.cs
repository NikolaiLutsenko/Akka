using Test.Akka.Actors.Messages;

namespace Test.Akka.Actors.Dtos
{
	public class Temperature: ITemperatureReading
	{
		public Temperature(double value)
		{
			Value = value;
		}

		public double Value { get; }
	}
}
