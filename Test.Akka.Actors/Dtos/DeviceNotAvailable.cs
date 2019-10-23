using Test.Akka.Actors.Messages;

namespace Test.Akka.Actors.Dtos
{
	public sealed class DeviceNotAvailable: ITemperatureReading
	{
		public static DeviceNotAvailable Instance = new DeviceNotAvailable();

		private DeviceNotAvailable() { }
	}
}
