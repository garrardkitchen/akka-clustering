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
    
    class Program
    {
        public static ActorSystem _actorSystem;

        public static IActorRef _event;

        static void Main(string[] args)
        {
            _actorSystem = ActorSystem.Create("coach");
            
            Console.WriteLine("SEED waiting....");
            Console.ReadKey();
        }
    }
}
