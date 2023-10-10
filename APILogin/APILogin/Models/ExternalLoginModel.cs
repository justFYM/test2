namespace APILogin.Models
{
    public class ExternalLoginModel
    {
        public string Provider { get; set; }
        public string ProviderKey { get; set; }
        public string Email { get; set; }
        public string ProviderDisplayName { get; set; }
    }
}
