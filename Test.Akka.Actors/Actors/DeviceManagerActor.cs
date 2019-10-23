using Akka.Actor;
using Akka.Event;
using System.Collections.Generic;

namespace Test.Akka.Actors.Actors
{

	public static partial class MainDeviceGroup
	{
		public sealed class RequestTrackDevice
		{
			public RequestTrackDevice(string groupId, string deviceId)
			{
				GroupId = groupId;
				DeviceId = deviceId;
			}

			public string GroupId { get; }
			public string DeviceId { get; }
		}

		public class DeviceManagerActor : UntypedActor
		{
			private readonly Dictionary<string, IActorRef> groupIdToActor = new Dictionary<string, IActorRef>();
			private readonly Dictionary<IActorRef, string> actorToGroupId = new Dictionary<IActorRef, string>();

			protected ILoggingAdapter Log { get; } = Context.GetLogger();

			protected override void OnReceive(object message)
			{
				switch (message)
				{
					case RequestTrackDevice requestTrackDevice:
						if(!groupIdToActor.TryGetValue(requestTrackDevice.DeviceId, out var actorRef))
						{
							Log.Info($"Creating device group actor for {requestTrackDevice.GroupId}");
							actorRef = Context.ActorOf(DeviceGroupActor.Prop(requestTrackDevice.GroupId));
							Context.Watch(actorRef);
							groupIdToActor.Add(requestTrackDevice.GroupId, actorRef);
							actorToGroupId.Add(actorRef, requestTrackDevice.GroupId);
							
						}
						actorRef.Forward(message);
						break;
					case Terminated terminated:
						var groupId = actorToGroupId[terminated.ActorRef];
						actorToGroupId.Remove(terminated.ActorRef);
						groupIdToActor.Remove(groupId);
						Log.Info($"Device group actor for {groupId} has been terminated");
						break;
				}
			}

			public static Props Prop => Props.Create<DeviceManagerActor>();
		}
	}
}
