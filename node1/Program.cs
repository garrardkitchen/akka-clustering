using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace node1
{
    using Akka.Actor;
    using Akka.Routing;
    using Shared;

    class Program
    {

        public static ActorSystem _actorSystem;

        static void Main(string[] args)
        {
            _actorSystem = ActorSystem.Create("coach");

            //var actor = _actorSystem.ActorOf(Props.Create(() => new EventActor()).WithRouter(FromConfig.Instance), "event");
            var actor = _actorSystem.ActorOf(Props.Empty.WithRouter(FromConfig.Instance), "event");
            //var start = _actorSystem.ActorOf(Props.Create(() => new StartActor(actor)), "start");
            var start = _actorSystem.ActorOf(Props.Create<StartActor>(actor), "start");
            
            var counter = 0;
            Console.WriteLine("Press ESC to stop");
            while (true)
            {
                counter++;
                ConsoleKeyInfo result = Console.ReadKey();
                if (result.Key == ConsoleKey.Escape)
                {
                    break;
                }
               
                    Console.WriteLine("Sending...");

                start.Tell(new AuditMessage("Hi - " + counter.ToString()));
                //actor.Tell(new AuditMessage("Hi - " + counter.ToString()));
               
            }

            Console.ReadKey();
        }
    }
}
