namespace Shared
{
    using System;
    using Akka.Actor;
    using Akka.Cluster;

    public class Node2Actor : ReceiveActor
    {

        //protected Akka.Cluster.Cluster Cluster = Akka.Cluster.Cluster.Get(Context.System);

        public Node2Actor()
        {
            //var actor = Context.System.ActorSelection("/user/audit");

            Receive<AuditMessage>(s =>
            {
                Console.WriteLine("Recevied! - " + s.Message);
            });

            Receive<Terminated>(t =>
            {
                
            });

            //Receive<ClusterEvent.MemberUp>(t =>
            //{

            //});

            ReceiveAny((e) =>
            {
                
            });
        }


        protected override void PreRestart(Exception reason, object message)
        {
            base.PreRestart(reason, message);
            Self.Tell(message);
        }

        //protected override void PreStart()
        //{
        //    // subscribe to IMemberEvent and UnreachableMember events
        //    Cluster.Subscribe(Self, ClusterEvent.InitialStateAsEvents,
        //        new[] { typeof(ClusterEvent.IMemberEvent), typeof(ClusterEvent.UnreachableMember) });
        //}

        //protected override void PostStop()
        //{
        //    Cluster.Unsubscribe(Self);
        //}

    }
}