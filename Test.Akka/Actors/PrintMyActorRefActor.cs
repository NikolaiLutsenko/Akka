using Akka.Actor;
using System;

namespace Test.Akka.Actors
{
	class PrintMyActorRefActor : UntypedActor
	{
		protected override void OnReceive(object message)
		{
			switch (message)
			{
				case "print":
					IActorRef secondRef = Context.ActorOf(Props.Empty, "second-actor");
					Console.WriteLine($"Second: {secondRef}");
					break;
			}
		}
	}
}
