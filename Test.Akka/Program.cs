using Akka.Actor;
using System;
using Test.Akka.Actors;

namespace Test.Akka
{
	class Program
	{
		static void Main(string[] args)
		{
			using var sys = ActorSystem.Create("TestAkka");
			var firstActor = sys.ActorOf(Props.Create<PrintMyActorRefActor>(), "first-actor");
			firstActor.Tell("print");
			sys.WhenTerminated.Wait();
		}
	}
}
