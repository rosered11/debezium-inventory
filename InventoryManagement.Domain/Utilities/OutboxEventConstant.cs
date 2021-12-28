namespace InventoryManagement.Domain.Utilities
{
    public static class OutboxEventConstant
    {
        public const string RECEIVE_ORDER = "ReceiveOrder";
        public const string REQUEST_ORDER = "RequestOrder";
        public const string RECEIVE_PART = "ReceivePart";
        public const string REQUEST_PART = "RequestPart";
    }

    public static class EventConstant
    {
        public const string EVENT_ID = "EventId";
        public const string EVENT_TYPE = "EventType";
    }
}
