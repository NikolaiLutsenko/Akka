namespace Test.Akka.Actors.Messages
{
	public sealed class CollectionTimeOut
	{
		public static CollectionTimeOut Instance = new CollectionTimeOut();
		private CollectionTimeOut() { }
	}
}
