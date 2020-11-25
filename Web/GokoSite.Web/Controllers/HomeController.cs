namespace GokoSite.Web.Controllers
{
    using System.Diagnostics;
    using System.Threading.Tasks;

    using GokoSite.Services.Messaging;
    using GokoSite.Web.ViewModels;
    using GokoSite.Web.ViewModels.Home;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        private readonly IEmailSender emailSender;

        public HomeController(IEmailSender emailSender)
        {
            this.emailSender = emailSender;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        public IActionResult Contact()
        {
            return this.View();
        }

        public IActionResult Error404()
        {
            return this.View();
        }

        public IActionResult ThankYou()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> SendFeedback(ContactInputModel input)
        {
            await this.emailSender.SendEmailAsync(input.Email, $"{input.FirstName} {input.LastName}", "gokocompany@abv.bg", input.Subject, input.Message);

            return this.Redirect("/Home/ThankYou");
        }
    }
}
