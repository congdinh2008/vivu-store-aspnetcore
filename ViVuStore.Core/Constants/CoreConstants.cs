namespace ViVuStore.Core.Constants;

public class CoreConstants
{
    public static readonly Guid SystemAdministratorId = Guid.Parse("93f8139d-3195-4a4e-647b-08dd5c670130");

    public struct Schemas
    {
        public const string Common = "Common";
        public const string Security = "Security";
    }

    public struct UserRoles
    {
        public const string SystemAdministrator = "systemadministrator";
    }
}
