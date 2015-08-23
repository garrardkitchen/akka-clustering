using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace node2
{
    using System.Threading;
    using Akka.Actor;
    using Akka.Routing;
    using Shared;

    class Program
    {
        public static ActorSystem _actorSystem;

        public static IActorRef _event;

        static void Main(string[] args)
        {
            _actorSystem = ActorSystem.Create("coach");

            //ActorSystem.ActorOf(Props.Create(() => new EventActor()),"audit");
            //_event = _actorSystem.ActorOf<EventActor>("event");

            var eventPath = args.Length > 0 ? args[0] : "e1";

            _event = _actorSystem.ActorOf<Node2Actor>(eventPath);
            

            Console.WriteLine("Hi from node 2 - " + eventPath);

            while (true)
            {
                Thread.Sleep(1000);
                //Console.WriteLine("Waiting");
            }


            Console.ReadKey();

            

        }
    }
}
