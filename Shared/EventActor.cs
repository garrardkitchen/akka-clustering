namespace Shared
{
    using System;
    using Akka.Actor;

    public class EventActor : ReceiveActor
    {

        public EventActor()
        {
            //var actor = Context.System.ActorSelection("/user/audit");

            Receive<AuditMessage>(s =>
            {
                Console.WriteLine("Recevied! - " + s.Message);
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