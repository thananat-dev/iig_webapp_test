namespace iig_webapp_test.Models
{
    public class UpdateProfileRequest
    {
        public string? NewPassword { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfileImage { get; set; }
    }
}
