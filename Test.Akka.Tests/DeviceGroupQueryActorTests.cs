using Akka.Actor;
using Akka.TestKit.Xunit;
using Akka.Util.Internal;
using System;
using System.Collections.Generic;
using Test.Akka.Actors.Actors;
using Test.Akka.Actors.Dtos;
using Test.Akka.Actors.Messages;
using Xunit;

namespace Test.Akka.Tests
{
	public class DeviceGroupQueryActorTests: TestKit
	{
		[Fact]
		public void Must_return_temperature_value_for_working_devices()
		{
			var requester = CreateTestProbe();

			var device1 = CreateTestProbe();
			var device2 = CreateTestProbe();
			var device3 = CreateTestProbe();
			var actorToDeviceId = new Dictionary<IActorRef, string>
			{
				[device1] = "device1",
				[device2] = "device2",
				[device3] = "device3"
			};


			var queryActor = Sys.ActorOf(DeviceGroupQueryActor.Prop(
				requestId: 1,
				actorToDeviceId: actorToDeviceId,
				requester: requester,
				timeout: TimeSpan.FromSeconds(10)));

			device1.ExpectMsg<ReadTemperature>(request => request.RequestId == 0);
			device2.ExpectMsg<ReadTemperature>(request => request.RequestId == 0);
			device3.ExpectMsg<ReadTemperature>(request => request.RequestId == 0);

			queryActor.Tell(new RespondTemperature(requestId: 0, value: 1.0), device1.Ref);
			queryActor.Tell(new RespondTemperature(requestId: 0, value: 2.0), device2.Ref);
			queryActor.Tell(new RespondTemperature(requestId: 0, value: 3.0), device3.Ref);
			
			requester.ExpectMsg<RespondAllTemperatures>(msg =>
				msg.Temperatures["device1"].AsInstanceOf<Temperature>().Value == 1.0 &&
				msg.Temperatures["device2"].AsInstanceOf<Temperature>().Value == 2.0 &&
				msg.Temperatures["device3"].AsInstanceOf<Temperature>().Value == 3.0
			);
		}

		[Fact]
		public void Must_return_TemperatureNotAvailable_for_devices_with_no_readings()
		{
			var requester = CreateTestProbe();

			var device1 = CreateTestProbe();
			var device2 = CreateTestProbe();
			var device3 = CreateTestProbe();
			var actorToDeviceId = new Dictionary<IActorRef, string>
			{
				[device1] = "device1",
				[device2] = "device2",
				[device3] = "device3"
			};

			var queryActor = Sys.ActorOf(DeviceGroupQueryActor.Prop(
				requestId: 1,
				actorToDeviceId: actorToDeviceId,
				requester: requester,
				timeout: TimeSpan.FromSeconds(10)));

			device1.ExpectMsg<ReadTemperature>(request => request.RequestId == 0);
			device2.ExpectMsg<ReadTemperature>(request => request.RequestId == 0);
			device3.ExpectMsg<ReadTemperature>(request => request.RequestId == 0);

			queryActor.Tell(new RespondTemperature(requestId: 0, value: 1.0), device1.Ref);
			queryActor.Tell(new RespondTemperature(requestId: 0, value: null), device2.Ref);
			queryActor.Tell(new RespondTemperature(requestId: 0, value: 3.0), device3.Ref);

			requester.ExpectMsg<RespondAllTemperatures>(msg =>
				msg.Temperatures["device1"].AsInstanceOf<Temperature>().Value == 1.0 &&
				msg.Temperatures["device2"] is TemperatureNotAvailable &&
				msg.Temperatures["device3"].AsInstanceOf<Temperature>().Value == 3.0
			);
		}

		[Fact]
		public void Must_return_return_DeviceNotAvailable_if_device_stops_before_answering()
		{
			var requester = CreateTestProbe();

			var device1 = CreateTestProbe();
			var device2 = CreateTestProbe();
			var device3 = CreateTestProbe();
			var actorToDeviceId = new Dictionary<IActorRef, string>
			{
				[device1] = "device1",
				[device2] = "device2",
				[device3] = "device3"
			};

			var queryActor = Sys.ActorOf(DeviceGroupQueryActor.Prop(
				requestId: 1,
				actorToDeviceId: actorToDeviceId,
				requester: requester,
				timeout: TimeSpan.FromSeconds(10)));

			device1.ExpectMsg<ReadTemperature>(request => request.RequestId == 0);
			device2.ExpectMsg<ReadTemperature>(request => request.RequestId == 0);
			device3.ExpectMsg<ReadTemperature>(request => request.RequestId == 0);

			queryActor.Tell(new RespondTemperature(requestId: 0, value: 1.0), device1.Ref);
			device2.Tell(PoisonPill.Instance);
			device3.Tell(PoisonPill.Instance);

			requester.ExpectMsg<RespondAllTemperatures>(msg =>
				msg.Temperatures["device1"].AsInstanceOf<Temperature>().Value == 1.0 &&
				msg.Temperatures["device2"] is DeviceNotAvailable &&
				msg.Temperatures["device3"] is DeviceNotAvailable &&
				msg.RequestId == 1
			, timeout: TimeSpan.FromSeconds(20));
		}

		[Fact]
		public void Must_return_DeviceTimedOut_if_device_does_not_answer_in_time() 
		{
			var requester = CreateTestProbe();

			var device1 = CreateTestProbe();
			var device2 = CreateTestProbe();
			var device3 = CreateTestProbe();
			var actorToDeviceId = new Dictionary<IActorRef, string>
			{
				[device1] = "device1",
				[device2] = "device2",
				[device3] = "device3"
			};

			var queryActor = Sys.ActorOf(DeviceGroupQueryActor.Prop(
				requestId: 1,
				actorToDeviceId: actorToDeviceId,
				requester: requester,
				timeout: TimeSpan.FromSeconds(5)));

			device1.ExpectMsg<ReadTemperature>(request => request.RequestId == 0);
			device2.ExpectMsg<ReadTemperature>(request => request.RequestId == 0);
			device3.ExpectMsg<ReadTemperature>(request => request.RequestId == 0);

			queryActor.Tell(new RespondTemperature(requestId: 0, value: 1.0), device1.Ref);

			requester.ExpectMsg<RespondAllTemperatures>(msg =>
				msg.Temperatures["device1"].AsInstanceOf<Temperature>().Value == 1.0 &&
				msg.Temperatures["device2"] is DeviceTimedOut &&
				msg.Temperatures["device3"] is DeviceTimedOut &&
				msg.RequestId == 1
			, timeout: TimeSpan.FromSeconds(20));
		}
	}
}
