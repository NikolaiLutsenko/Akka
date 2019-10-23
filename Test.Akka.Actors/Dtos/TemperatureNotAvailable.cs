using Test.Akka.Actors.Messages;

namespace Test.Akka.Actors.Dtos
{
	public sealed class TemperatureNotAvailable: ITemperatureReading
	{
		public static TemperatureNotAvailable Instance = new TemperatureNotAvailable();

		private TemperatureNotAvailable() { }
	}
}
