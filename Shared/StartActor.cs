namespace Shared
{
    using System;
    using Akka.Actor;
    using Akka.Cluster;

    public class StartActor : ReceiveActor
    {
        protected Akka.Cluster.Cluster Cluster = Akka.Cluster.Cluster.Get(Context.System);


        public StartActor(IActorRef remote)
        {
            //var actor = Context.System.ActorSelection("/user/audit");
            
            Receive<AuditMessage>(s =>
            
            {
                remote.Tell(s);
            });

            Receive<Terminated>(t =>
            {

            });

            Receive<ClusterEvent.MemberUp>(t =>
            {

            });
        }

        protected override void PreRestart(Exception reason, object message)
        {
            base.PreRestart(reason, message);
            Self.Tell(message);
        }


        protected override void PreStart()
        {
            // subscribe to IMemberEvent and UnreachableMember events
            Cluster.Subscribe(Self, ClusterEvent.InitialStateAsEvents,
                new[] { typeof(ClusterEvent.IMemberEvent), typeof(ClusterEvent.UnreachableMember) });
        }

        protected override void PostStop()
        {
            Cluster.Unsubscribe(Self);
        }

        //protected override void PreStart()
        //{
        //}

        //protected override void PreRestart(Exception reason, object message)
        //{
        //    foreach (IActorRef each in Context.GetChildren())
        //    {
        //        Context.Unwatch(each);
        //        Context.Stop(each);
        //    }
        //    PostStop();
        //}

        //protected override void PostRestart(Exception reason)
        //{
        //    PreStart();
        //}

        //protected override void PostStop()
        //{
        //}

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy( 10, TimeSpan.FromSeconds(30),
            x =>
            {
                //Maybe we consider ArithmeticException to not be application critical
                //so we just ignore the error and keep going.
                if (x is ArithmeticException) return Directive.Resume;

                //Error that we cannot recover from, stop the failing actor
                else if (x is NotSupportedException) return Directive.Stop;

                //In all other cases, just restart the failing actor
                else return Directive.Restart;
            });
        }
    }
}