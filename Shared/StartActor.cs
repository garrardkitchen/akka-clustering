namespace Shared
{
    using System;
    using System.Linq;
    using Akka.Actor;
    using Akka.Cluster;
    using Akka.Dispatch.SysMsg;

    public class StartActor : ReceiveActor
    {
        private readonly IActorRef _remote;
        protected Akka.Cluster.Cluster Cluster = Akka.Cluster.Cluster.Get(Context.System);

        public StartActor(IActorRef remote)
        {
            _remote = remote;

           Become(NotReady);
        }

        public void Ready()
        {
            Util.WriteLine(ConsoleColor.Magenta, "/user/supervisor/start Ready");

            Receive<AuditMessage>(s =>
            {
                _remote.Tell(s);
                Sender.Tell("", Self);
            });

            Receive<Terminated>(t =>
            {
                
            });

            Receive<ClusterEvent.MemberUp>(t =>
            {
                Util.WriteLine(ConsoleColor.Magenta, $"/user/supervisor/start MemberUp roles: <{string.Join(",", t.Member.Roles)}> port: <{t.Member.Address.Port}>");
            });

            Receive<ClusterEvent.MemberRemoved>(t =>
            {
                Util.WriteLine(ConsoleColor.Magenta, $"/user/supervisor/start MemberDown roles: <{string.Join(",", t.Member.Roles)}> port: <{t.Member.Address.Port}>");
                Become(NotReady);
            });
        }

        public void NotReady()
        {
            Receive<ClusterEvent.MemberUp>(t =>
            {
                Util.WriteLine(ConsoleColor.Magenta, $"/user/supervisor/start MemberUp roles: <{string.Join(",", t.Member.Roles)}> port: <{t.Member.Address.Port}>");
                Become(Ready);
            });
        }

        protected override void PreRestart(Exception reason, object message)
        {
            Util.WriteLine(ConsoleColor.Magenta,"/user/supervisor/start PreRestart");
            
            base.PreRestart(reason, message);
            Self.Tell(message);
        }

        protected override void PreStart()
        {
            Util.WriteLine(ConsoleColor.Magenta, "/user/supervisor/start PreStart");

            Cluster.Subscribe(Self, ClusterEvent.InitialStateAsEvents,
                new[] { typeof(ClusterEvent.IMemberEvent), typeof(ClusterEvent.UnreachableMember) });
        }

        protected override void PostStop()
        {
            Util.WriteLine(ConsoleColor.Magenta, "/user/supervisor/start PostStep");
            Cluster.Unsubscribe(Self);
        }
      

        protected override void PostRestart(Exception reason)
        {
            Util.WriteLine(ConsoleColor.Magenta, "/user/supervisor/start PostRestart");
            base.PostRestart(reason);
        }
    }
}