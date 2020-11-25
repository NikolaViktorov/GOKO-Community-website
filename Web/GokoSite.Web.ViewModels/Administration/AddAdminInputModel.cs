namespace GokoSite.Web.ViewModels.Administration
{
    using System.ComponentModel.DataAnnotations;

    public class AddAdminInputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
