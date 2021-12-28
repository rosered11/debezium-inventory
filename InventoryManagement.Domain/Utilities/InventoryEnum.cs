namespace InventoryManagement.Domain.Utilities
{
    public static class InputTransactionExtension
    {
        public static bool IsReceive(this InputTransactionType type)
        {
            switch (type)
            {
                case InputTransactionType.RC_SUBMITTED:
                case InputTransactionType.RC_ACCEPTED:
                case InputTransactionType.RC_COMPLETED:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsRequest(this InputTransactionType type)
        {
            switch (type)
            {
                case InputTransactionType.RQ_DELIVERED:
                case InputTransactionType.RQ_SUBMITTED:
                    return true;
                default:
                    return false;
            }
        }
    }
    public enum InputTransactionType
    {
        // PO Qty
        RC_SUBMITTED

        // Receiving Qty
        , RC_ACCEPTED

        // Balance Qty
        , RC_COMPLETED

        // Request Qty
        , RQ_SUBMITTED

        // Remove Balance Qty and Request Qty
        , RQ_DELIVERED
    }

    public enum InventoryTransactionType
    {
        // PO Qty
        STOCK_INCOMING

        // Receiving Qty
        , STOCK_RECEIVING

        // Balance Qty
        , STOCK_INCREASED

        // Request Qty
        // Success
        , STOCK_BOOKED
        // Failed
        , STOCK_BOOKING_FAILED

        // Remove Balance Qty and Request Qty
        , STOCK_DECREASED
    }

    public enum QuantityTypeEnum
    {
        AVAILABLE,

        BALANCE
    }
}
