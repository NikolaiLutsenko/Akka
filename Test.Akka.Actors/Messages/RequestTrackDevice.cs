namespace Test.Akka.Actors.Messages
{
	public class RequestTrackDevice
	{
		public RequestTrackDevice(string groupId, string deviceId)
		{
			GroupId = groupId;
			DeviceId = deviceId;
		}

		public string GroupId { get; }
		public string DeviceId { get; }
	}
}
