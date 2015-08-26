namespace Shared
{
    using System;
    using Akka.Actor;
    using Akka.Cluster;

    public class EventSupervisor : ReceiveActor
    {

        public EventSupervisor(string machineName, Guid id, string name)
        {
            Context.ActorOf(Props.Create<Node2Actor>(machineName, id), name);
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(10, TimeSpan.FromSeconds(30),
            x =>
            {
                //Maybe we consider ArithmeticException to not be application critical
                //so we just ignore the error and keep going.
                if (x is ArithmeticException) return Directive.Resume;

                //Error that we cannot recover from, stop the failing actor
                else if (x is NotSupportedException) return Directive.Restart;

                //In all other cases, just restart the failing actor
                else return Directive.Restart;
            });
        }
    }
}