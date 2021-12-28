using System;

namespace InventoryManagement.Domain.Utilities
{
    public static class InventoryTypeUtility
    {
        public static string GetInventoryType(Utilities.InputTransactionType inputType)
        {
            switch (inputType)
            {
                case Utilities.InputTransactionType.RC_SUBMITTED:
                    return nameof(Utilities.InventoryTransactionType.STOCK_INCOMING);
                case Utilities.InputTransactionType.RC_ACCEPTED:
                    return nameof(Utilities.InventoryTransactionType.STOCK_RECEIVING);
                case Utilities.InputTransactionType.RC_COMPLETED:
                    return nameof(Utilities.InventoryTransactionType.STOCK_INCREASED);
                case Utilities.InputTransactionType.RQ_SUBMITTED:
                    return nameof(Utilities.InventoryTransactionType.STOCK_BOOKED);
                case Utilities.InputTransactionType.RQ_DELIVERED:
                    return nameof(Utilities.InventoryTransactionType.STOCK_DECREASED);
                default:
                    throw new ArgumentException($"Invalid mapping transaction type: {nameof(inputType)}");
            }
        }
    }
}
