using System;
using Akka.Actor;
using Akka.Event;
using Test.Akka.Actors.Messages;

namespace Test.Akka.Actors.Actors
{
	public class DeviceActor : UntypedActor
	{
		public string GroupId { get; }
		public string DeviceId { get; }
		protected ILoggingAdapter Log { get; } = Context.GetLogger();

		protected double? Temperature = null;

		public DeviceActor(string groupId, string deviceId)
		{
			GroupId = groupId;
			DeviceId = deviceId;
		}

		protected override void OnReceive(object message)
		{
			switch (message)
			{
				case RequestTrackDevice req when req.DeviceId.Equals(DeviceId) && req.GroupId.Equals(GroupId):
					Sender.Tell(DeviceRegistered.Instance);
					break;
				case RequestTrackDevice req:
					Log.Warning("Ignoring TrackDevice request for GroupId: {0}, DeviceId: {1}", req.GroupId, req.DeviceId);
					break;
				case RecordTemperature recordTemperature:
					Temperature = recordTemperature.Value;
					Log.Info("Temperature: {0} recorded for GroupId: {1}, DeviceId: {2}", recordTemperature.Value, GroupId, DeviceId);
					Sender.Tell(new TemperatureRecorded(recordTemperature.RequestId));
					break;
				case ReadTemperature readTemperature:
					Sender.Tell(new RespondTemperature(readTemperature.RequestId, Temperature));
					break;
				case "print":
					IActorRef secondRef = Context.ActorOf(Props.Empty, "second-actor");
					Console.WriteLine($"GroupId: {GroupId}, DeviceId: {DeviceId}, Second: {secondRef}");
					break;
			}
		}

		public static Props Prop(string groupId, string deviceId) => Props.Create(() => new DeviceActor(groupId, deviceId));
	}
}
