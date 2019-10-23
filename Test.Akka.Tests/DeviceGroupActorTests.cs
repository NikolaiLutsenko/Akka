using Akka.TestKit.Xunit;
using Akka.Util.Internal;
using FluentAssertions;
using System;
using Test.Akka.Actors.Actors;
using Test.Akka.Actors.Dtos;
using Test.Akka.Actors.Messages;
using Xunit;

namespace Test.Akka.Tests
{
	public class DeviceGroupActorTests: TestKit
	{
		[Fact]
		public void RequestTrackDevice_Should_Return_DeviceRegistered()
		{
			var prob = CreateTestProbe();
			var deviceGroupActor = Sys.ActorOf(DeviceGroupActor.Prop("TestGroup"));

			deviceGroupActor.Tell(new RequestTrackDevice("TestGroup", "DeviceId1"), prob.Ref);
			prob.ExpectMsg<DeviceRegistered>();
			var deviceActor1 = prob.LastSender;

			deviceGroupActor.Tell(new RequestTrackDevice("WrongGroup", "DeviceId"), prob.Ref);
			prob.ExpectNoMsg(TimeSpan.FromSeconds(2));

			deviceGroupActor.Tell(new RequestTrackDevice("TestGroup", "DeviceId2"), prob.Ref);
			prob.ExpectMsg<DeviceRegistered>();
			var deviceActor2 = prob.LastSender;

			deviceActor1.Should().NotBe(deviceActor2);
		}

		[Fact]
		public void RequestTrackDevice_With_Same_DeviceId()
		{
			var prob = CreateTestProbe();
			var deviceGroupActor = Sys.ActorOf(DeviceGroupActor.Prop("TestGroup"));

			deviceGroupActor.Tell(new RequestTrackDevice("TestGroup", "DeviceId1"), prob.Ref);
			prob.ExpectMsg<DeviceRegistered>(TimeSpan.FromSeconds(10));
			var deviceActor1 = prob.LastSender;

			deviceGroupActor.Tell(new RequestTrackDevice("TestGroup", "DeviceId1"), prob.Ref);
			prob.ExpectMsg<DeviceRegistered>(TimeSpan.FromSeconds(10));
			var deviceActor2 = prob.LastSender;

			deviceActor1.Should().Be(deviceActor2);
		}

		[Fact]
		public void Must_be_able_to_collect_temperatures_from_all_active_devices()
		{
			var prob = CreateTestProbe();
			var groupId = "TestGroup";

			var deviceGroupActor = Sys.ActorOf(DeviceGroupActor.Prop(groupId));

			deviceGroupActor.Tell(new RequestTrackDevice(groupId, "device1"), prob.Ref);
			prob.ExpectMsg<DeviceRegistered>();
			var device1 = prob.LastSender;

			deviceGroupActor.Tell(new RequestTrackDevice(groupId, "device2"), prob.Ref);
			prob.ExpectMsg<DeviceRegistered>();
			var device2 = prob.LastSender;

			deviceGroupActor.Tell(new RequestTrackDevice(groupId, "device3"), prob.Ref);
			prob.ExpectMsg<DeviceRegistered>();
			var device3 = prob.LastSender;

			device1.Tell(new RecordTemperature(requestId: 1, 1.0), prob.Ref);
			prob.ExpectMsg<TemperatureRecorded>(msg => msg.RequestId == 1);

			device2.Tell(new RecordTemperature(requestId: 2, 2.0), prob.Ref);
			prob.ExpectMsg<TemperatureRecorded>(msg => msg.RequestId == 2);

			deviceGroupActor.Tell(new RequestAllTemperatures(4), prob.Ref);

			prob.ExpectMsg<RespondAllTemperatures>(msg => 
				msg.RequestId == 4 &&
				msg.Temperatures["device1"].AsInstanceOf<Temperature>().Value == 1.0 &&
				msg.Temperatures["device2"].AsInstanceOf<Temperature>().Value == 2.0 &&
				msg.Temperatures["device3"] is TemperatureNotAvailable
			);
		}
	}
}
