using Akka.TestKit.Xunit;
using System;
using Test.Akka.Actors.Actors;
using Test.Akka.Actors.Messages;
using Xunit;

namespace Test.Akka.Tests
{
	public class DeviceActorTests: TestKit
	{
		[Fact]
		public void RecordTemperature_Should_Return_TemperatureRecorded()
		{
			var probe = CreateTestProbe();

			var deviceActor = Sys.ActorOf(DeviceActor.Prop("Group", "Device"));
			deviceActor.Tell(new RecordTemperature(requestId: 1, value: 20.0), probe.Ref);
			probe.ExpectMsg<TemperatureRecorded>(msg => msg.RequestId == 1);
		}

		[Fact]
		public void RequestTrackDevice_Should_Return_DeviceRegistered()
		{
			var probe = CreateTestProbe();

			var deviceActor = Sys.ActorOf(DeviceActor.Prop("Group", "Device"));
			deviceActor.Tell(new RequestTrackDevice("Group", "Device"), probe);
			probe.ExpectMsg<DeviceRegistered>();

			deviceActor.Tell(new RequestTrackDevice("wrongGroup", "device"), probe.Ref);
			probe.ExpectNoMsg(TimeSpan.FromMilliseconds(500));
		}
	}
}
