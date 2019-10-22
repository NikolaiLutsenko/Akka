using Akka.TestKit.Xunit;
using FluentAssertions;
using System;
using Test.Akka.Actors.Actors;
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
	}
}
