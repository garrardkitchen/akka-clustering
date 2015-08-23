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

            Console.WriteLine("Node 1 started...");
            Console.WriteLine("Press [ESC] to stop, [L] 1k messages, [H] for 10k, [M] for 100k or any other key to send a single message");
            while (true)
            {
                ConsoleKeyInfo result = Console.ReadKey();
                if (result.Key == ConsoleKey.Escape)
                {
                    break;
                }

                switch (result.Key)
                {
                    case ConsoleKey.L:
                    {
                        counter = TransmitMessageManyTimes(counter, start, 1000);
                        break;
                    }
                    case ConsoleKey.H:
                    {
                        counter = TransmitMessageManyTimes(counter, start, 10000);
                        break;
                    }
                    case ConsoleKey.M:
                    {
                        counter = TransmitMessageManyTimes(counter, start, 100000);
                        break;
                    }
                    default:
                    {
                        counter = TransmitMessageManyTimes(counter, start, 1);
                        break;
                    }
                }

                
                //actor.Tell(new AuditMessage("Hi - " + counter.ToString()));
            }

            Console.ReadKey();
        }

        private static int TransmitMessageManyTimes(int counter, IActorRef start, int amount)
        {
            Console.Write($"Transmitting {amount:##,###} message(s) -> ");
            for (int i = 0; i < amount; i++)
            {
                counter++;
                start.Tell(new AuditMessage("Message no. - " + counter.ToString()));
            }
            Console.WriteLine(" [x] <- Transmitted message(s)");
            return counter;
        }
    }
}
