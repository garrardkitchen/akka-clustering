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
            
            var actor = _actorSystem.ActorOf(Props.Empty.WithRouter(FromConfig.Instance), "event");
            _actorSystem.ActorOf(Props.Create<StartSupervisorActor>(actor), "supervisor");
            var startActor = _actorSystem.ActorSelection("/user/supervisor/start");

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
                        counter = TransmitMessageManyTimes(counter, startActor, 1000);
                        break;
                    }
                    case ConsoleKey.H:
                    {
                        counter = TransmitMessageManyTimes(counter, startActor, 10000);
                        break;
                    }
                    case ConsoleKey.M:
                    {
                        counter = TransmitMessageManyTimes(counter, startActor, 100000);
                        break;
                    }
                    default:
                    {
                        counter = TransmitMessageManyTimes(counter, startActor, 1);
                        break;
                    }
                }

                
                //actor.Tell(new AuditMessage("Hi - " + counter.ToString()));
            }

            Console.ReadKey();
        }

        private static int TransmitMessageManyTimes(int counter, ActorSelection start, int amount)
        {
            Console.Write($"Transmitting {amount:##,###} message(s) -> ");
            for (int i = 0; i < amount; i++)
            {
                counter++;

                //start.Tell(new AuditMessage("Message no. - " + counter.ToString()));
                start.Ask(new AuditMessage("Message no. - " + counter.ToString())).ContinueWith((m) =>
                {
                    
                });
            }
            Console.WriteLine(" [x] <- Transmitted message(s)");
            return counter;
        }
    }
}
