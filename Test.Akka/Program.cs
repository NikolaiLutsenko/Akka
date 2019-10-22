using Akka.Actor;
using System;
using Test.Akka.Actors.Actors;
using Test.Akka.Actors.Messages;

namespace Test.Akka
{
	class Program
	{
		static void Main(string[] args)
		{
			using var sys = ActorSystem.Create("TestAkka");
			var groupName = "MyGroupId";
			var firstActor = sys.ActorOf(DeviceGroupActor.Prop(groupName), $"DeviceGroupActor-{groupName}");
			firstActor.Tell(new RequestTrackDevice(groupName, "FirstDevice"), firstActor);
			Console.WriteLine($"First: {firstActor}");
			sys.WhenTerminated.Wait();
		}
	}
}
