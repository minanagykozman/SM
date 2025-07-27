namespace SM.API.Authorization
{
    public class UserPermissionsDto
    {
        public Dictionary<string, bool> Permissions { get; set; } = new Dictionary<string, bool>();
    }
}
