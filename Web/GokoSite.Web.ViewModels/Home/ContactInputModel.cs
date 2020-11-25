namespace GokoSite.Web.ViewModels.Home
{
    using System.ComponentModel.DataAnnotations;

    public class ContactInputModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        [MinLength(30)]
        public string Message { get; set; }
    }
}
