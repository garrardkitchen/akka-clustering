namespace Shared
{
    using System;
    using Akka.Actor;
    using Akka.Cluster;

    public class StartSupervisorActor : ReceiveActor
    {
        private readonly IActorRef _remote;
        protected Akka.Cluster.Cluster Cluster = Akka.Cluster.Cluster.Get(Context.System);

        public StartSupervisorActor(IActorRef remote)
        {
            _remote = remote;
            Context.ActorOf(Props.Create<StartActor>(remote), "start");
        }
        
        protected override void PreStart()
        {
            Util.WriteLine(ConsoleColor.Gray, "/user/supervisor PreStart");
            // subscribe to IMemberEvent and UnreachableMember events
            Cluster.Subscribe(Self, ClusterEvent.InitialStateAsEvents,
                new[] { typeof(ClusterEvent.IMemberEvent), typeof(ClusterEvent.UnreachableMember) });
        }

        protected override void PostStop()
        {
            Util.WriteLine(ConsoleColor.Gray, "/user/supervisor PostStop");

            Cluster.Unsubscribe(Self);
        }

        protected override void PreRestart(Exception reason, object message)
        {
            Util.WriteLine(ConsoleColor.Gray, "/user/supervisor PreRestart");

            base.PreRestart(reason, message);
            Self.Tell(message);
            //foreach (IActorRef each in Context.GetChildren())
            //{
            //    Context.Unwatch(each);
            //    Context.Stop(each);
            //}
            //PostStop();
        }

        protected override void PostRestart(Exception reason)
        {
            Util.WriteLine(ConsoleColor.Gray, "/user/supervisor PostRestart");
            PreStart();
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy( 10, TimeSpan.FromSeconds(30),
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