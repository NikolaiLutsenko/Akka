using Test.Akka.Actors.Messages;

namespace Test.Akka.Actors.Dtos
{
	public sealed class DeviceTimedOut: ITemperatureReading
	{
		public static DeviceTimedOut Instance = new DeviceTimedOut();

		private DeviceTimedOut() { }
	}
}
