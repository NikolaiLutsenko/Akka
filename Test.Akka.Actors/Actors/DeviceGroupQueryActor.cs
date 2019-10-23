using Akka.Actor;
using Akka.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using Test.Akka.Actors.Dtos;
using Test.Akka.Actors.Messages;

namespace Test.Akka.Actors.Actors
{
	public class DeviceGroupQueryActor : UntypedActor
	{
		private ICancelable queryTimeoutTimer;

		public DeviceGroupQueryActor(Dictionary<IActorRef, string> actorToDeviceId, int requestId, IActorRef requester, TimeSpan timeout)
		{
			ActorToDeviceId = actorToDeviceId;
			RequestId = requestId;
			Requester = requester;
			Timeout = timeout;

			queryTimeoutTimer = Context.System.Scheduler.ScheduleTellOnceCancelable(timeout, Self, CollectionTimeOut.Instance, Self);
			Become(WaitingForReplies(new Dictionary<string, ITemperatureReading>(), new HashSet<IActorRef>(ActorToDeviceId.Keys)));
		}

		private UntypedReceive WaitingForReplies(Dictionary<string, ITemperatureReading> repliesSoFar, HashSet<IActorRef> stillWaiting)
		{
			return message =>
			{
				switch (message)
				{
					case RespondTemperature response when response.RequestId == 0:
						var deviceActor = Sender;
						var reading = response.Value.HasValue
							? (ITemperatureReading)new Temperature(response.Value.Value)
							: TemperatureNotAvailable.Instance;
						ReceivedResponse(deviceActor, reading, stillWaiting, repliesSoFar);
						break;
					case Terminated t:
						ReceivedResponse(t.ActorRef, DeviceNotAvailable.Instance, stillWaiting, repliesSoFar);
						break;
					case CollectionTimeOut _:
						var replies = new Dictionary<string, ITemperatureReading>(repliesSoFar);
						foreach (var actor in stillWaiting)
						{
							var deviceId = ActorToDeviceId[actor];
							replies.Add(deviceId, DeviceTimedOut.Instance);
						}
						Requester.Tell(new RespondAllTemperatures(RequestId, replies));
						Context.Stop(Self);
						break;
				}
			};
		}

		private void ReceivedResponse(IActorRef deviceActor, ITemperatureReading reading, HashSet<IActorRef> stillWaiting, Dictionary<string, ITemperatureReading> repliesSoFar)
		{
			Context.Unwatch(deviceActor);
			var deviceId = ActorToDeviceId[deviceActor];
			repliesSoFar[deviceId] = reading;
			stillWaiting.Remove(deviceActor);

			if (!stillWaiting.Any())
			{
				Requester.Tell(new RespondAllTemperatures(RequestId, repliesSoFar));
				Context.Stop(Self);
			}
			else
			{
				Context.Become(WaitingForReplies(repliesSoFar, stillWaiting));
			}
		}

		public Dictionary<IActorRef, string> ActorToDeviceId { get; }
		public int RequestId { get; }
		public IActorRef Requester { get; }
		public TimeSpan Timeout { get; }

		protected ILoggingAdapter Log { get; } = Context.GetLogger();

		protected override void PreStart()
		{
			foreach (var deviceActor in ActorToDeviceId.Keys)
			{
				Context.Watch(deviceActor);
				deviceActor.Tell(new ReadTemperature(0));
			}
		}

		protected override void PostStop() => queryTimeoutTimer.Cancel();

		protected override void OnReceive(object message) { }

		public static Props Prop(int requestId, Dictionary<IActorRef, string> actorToDeviceId, IActorRef requester, TimeSpan timeout)
			=> Props.Create(() => new DeviceGroupQueryActor(actorToDeviceId, requestId, requester, timeout));
	}
}
