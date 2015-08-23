namespace Shared
{
    using System;
    using Akka.Actor;
    using Akka.Cluster;

    public class Node2Actor : ReceiveActor
    {
        private readonly string _machineName;
        private readonly Guid _id;

        public Node2Actor(string machineName, Guid id)
        {
            _machineName = machineName;
            _id = id;

            Receive<AuditMessage>(s =>
            {
                Console.WriteLine($"machine: <{_machineName}> id: <{_id}> recevied message: <{s.Message}>");
            });

            Receive<Terminated>(t =>
            {
                
            });

            ReceiveAny((e) =>
            {
                
            });
        }

        protected override void PreRestart(Exception reason, object message)
        {
            base.PreRestart(reason, message);
            Self.Tell(message);
        }
    }
}