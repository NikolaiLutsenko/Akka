namespace Test.Akka.Actors.Messages
{
	public class DeviceRegistered
	{
		public static DeviceRegistered Instance { get; } = new DeviceRegistered();
		private DeviceRegistered() { }
	}
}
