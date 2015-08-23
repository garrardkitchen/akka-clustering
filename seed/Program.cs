namespace seed
{
    using System;
    using Akka.Actor;

    class Program
    {
        public static ActorSystem _actorSystem;

        static void Main(string[] args)
        {
            _actorSystem = ActorSystem.Create("coach");
            
            Console.WriteLine("Seed node running....");
            Console.ReadKey();
        }
    }
}
