namespace GokoSite.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GokoSite.Services.Data.Contracts;
    using GokoSite.Web.ViewModels.Products;
    using Microsoft.AspNetCore.Mvc;

    public class ProductsController : BaseController
    {
        private readonly IProductsService productsService;

        public ProductsController(IProductsService productsService)
        {
            this.productsService = productsService;
        }

        public async Task<IActionResult> Home()
        {
            var viewModel = await this.productsService.GetProducts();
            return this.View(viewModel);
        }

        public async Task<IActionResult> AddProduct()
        {
            if (!this.User.IsInRole("Administrator"))
            {
                return this.Redirect("/Products/Home");
            }

            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(AddProductInputModel input)
        {
            if (!this.User.IsInRole("Administrator"))
            {
                return this.Redirect("/Products/Home");
            }

            await this.productsService.AddProduct(input);

            return this.Redirect("/Products/Home");
        }

        public async Task<IActionResult> Product(string id)
        {
            var viewModel = await this.productsService.GetProduct(id);
            return this.View(viewModel);
        }
    }
}
