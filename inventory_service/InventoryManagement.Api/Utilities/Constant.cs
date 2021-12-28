using System;

namespace InventoryManagement.Api
{
    public static class CorsPolicyName
    {
        public const string AllowAny = "AllowAny";
    }

    public static class StatusCode
    {
        public const string Ok = "00000";
        public const string BadRequest = "40000";
        public const string BadRequest40010 = "40010";
        public const string Unauthorized = "40100";
        public const string Forbidden = "40300";
        public const string NotFound = "40400";
        public const string InternalServerError = "50000";
    }

    public static class StatusMessage
    {
        public const string Ok = "Success";
        public const string BadRequest = "Invalid Request";
        public const string BadRequest40010 = "Some parts have not enough stocks";
        public const string Unauthorized = "Unauthorized Access";
        public const string Forbidden = "Access Forbidden";
        public const string NotFound = "Data Not Found";
        public const string InternalServerError = "Internal Server Error";
    }
}
