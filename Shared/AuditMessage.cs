namespace Shared
{
    public class AuditMessage
    {
        public string Message { get; set; }

        public AuditMessage(string message)
        {
            Message = message;
        }
    }
}