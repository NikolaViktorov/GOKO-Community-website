namespace GokoSite.Web.ViewModels.Products
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Http;

    public class AddProductInputModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public IFormFile MainImage { get; set; }

        [Required]
        public string DownloadLink { get; set; }
    }
}
