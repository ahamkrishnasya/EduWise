using Microsoft.AspNetCore.Mvc;

namespace EduWise.Controllers
{
    public class VideoController : Controller
    {
        public IActionResult Index()
        {
            // Show educational videos or YouTube embeds
            var videos = new List<string>
            {
                "https://www.youtube.com/embed/kxKjvJjc7aU",
                "https://www.youtube.com/embed/6EtuY41mMfs"
            };
            return View(videos);
        }
    }
}
