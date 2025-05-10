namespace Cloudot.Shared.Constants;

public static class AppPolicies
{
    private const string Base = "Permissions.";

    public static class Users
    {
        private const string Prefix = $"{Base}Users.";
        public const string View = $"{Prefix}View";
        public const string Edit = $"{Prefix}Edit";
        public const string Delete = $"{Prefix}Delete";
    }

    public static class Procurement
    {
        private const string Prefix = $"{Base}Procurement.";
        public const string Request = $"{Prefix}Request";
        public const string Approve = $"{Prefix}Approve";
        public const string Order = $"{Prefix}Order";
    }
}
