namespace GokoSite.Data.Models.Products
{
    using System;

    public class Product
    {
        public Product()
        {
            this.ProductId = Guid.NewGuid().ToString();
        }

        public string ProductId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string MainPhoto { get; set; }

        public string DownloadLink { get; set; }
    }
}
