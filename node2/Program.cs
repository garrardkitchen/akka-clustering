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
        public static string _machineName = string.Empty;
        public static Guid _id = Guid.NewGuid();

        static void Main(string[] args)
        {
            var eventPath = args.Length > 0 ? args[0] : "e1";

            _machineName = Environment.MachineName;
            _actorSystem = ActorSystem.Create("coach");
            //_event = _actorSystem.ActorOf(Props.Create<Node2Actor>(_machineName, _id), eventPath);
            _actorSystem.ActorOf(Props.Create<EventSupervisor>(_machineName, _id, eventPath),"eventsupervisor");

            Console.WriteLine($"Node 2 started receiving messages on on actor path: <{eventPath}> on machine: <{_machineName}>");
            
            Console.ReadKey();
        }
    }
}
