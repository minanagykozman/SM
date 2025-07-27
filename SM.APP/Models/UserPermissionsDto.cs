namespace SM.APP.Models
{
    public class UserPermissionsDto
    {
        public Dictionary<string, bool> Permissions { get; set; } = new Dictionary<string, bool>();

    }
}
