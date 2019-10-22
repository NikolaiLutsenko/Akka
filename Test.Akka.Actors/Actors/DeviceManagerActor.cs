using Akka.Actor;
using Akka.Event;
using System;
using System.Collections.Generic;
using System.Text;

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
			private Dictionary<string, IActorRef> groupIdToActor = new Dictionary<string, IActorRef>();
			private Dictionary<IActorRef, string> actorToGroupId = new Dictionary<IActorRef, string>();

			protected ILoggingAdapter Log { get; } = Context.GetLogger();

			protected override void OnReceive(object message)
			{
				switch (message)
				{
					case RequestTrackDevice requestTrackDevice:
						if(!groupIdToActor.TryGetValue(requestTrackDevice.DeviceId, out var actorRef))
						{
							Log.Info($"Creating device group actor for {requestTrackDevice.GroupId}");
							var groupActor = Context.ActorOf(DeviceGroupActor.Prop(requestTrackDevice.GroupId));
							Context.Watch(groupActor);
							groupIdToActor.Add(requestTrackDevice.GroupId, groupActor);
							actorToGroupId.Add(groupActor, requestTrackDevice.GroupId);
							
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
