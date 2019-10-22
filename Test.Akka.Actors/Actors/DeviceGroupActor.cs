using Akka.Actor;
using Akka.Event;
using System.Collections.Generic;
using Test.Akka.Actors.Messages;

namespace Test.Akka.Actors.Actors
{
	public class DeviceGroupActor: UntypedActor
	{
		private Dictionary<string, IActorRef> deviceIdToActor = new Dictionary<string, IActorRef>();
		private Dictionary<IActorRef, string> actorToDeviceId = new Dictionary<IActorRef, string>();

		public DeviceGroupActor(string groupId)
		{
			GroupId = groupId;
		}

		public string GroupId { get; }
		protected ILoggingAdapter Log { get; } = Context.GetLogger();

		protected override void OnReceive(object message)
		{
			switch (message)
			{
				case RequestTrackDevice trackMsg when trackMsg.GroupId.Equals(GroupId):
					if (!deviceIdToActor.TryGetValue(trackMsg.DeviceId, out var deviceActor))
					{
						Log.Info($"Creating device actor for {trackMsg.DeviceId}");
						deviceActor = Context.ActorOf(DeviceActor.Prop(trackMsg.GroupId, trackMsg.DeviceId), $"device-{trackMsg.DeviceId}");
						Context.Watch(deviceActor);
						deviceIdToActor.Add(trackMsg.DeviceId, deviceActor);
						actorToDeviceId.Add(deviceActor, trackMsg.DeviceId);
					}
					deviceActor.Forward(message);
					break;
				case RequestTrackDevice trackMsg:
					Log.Warning("Ignoring TrackDevice request for {0}. This actor is responsible for {1}.", trackMsg.GroupId, trackMsg.DeviceId);
					break;
				case Terminated terminated:
					var deviceId = actorToDeviceId[terminated.ActorRef];
					Log.Info($"Device actor for {deviceId} has been terminated");
					actorToDeviceId.Remove(terminated.ActorRef);
					deviceIdToActor.Remove(deviceId);
					break;
			}
		}

		public static Props Prop(string groupId) => Props.Create(() => new DeviceGroupActor(groupId));
	}
}
