using Microsoft.AspNetCore.Mvc;
using Infogroup.IDMS.Web.Controllers;

namespace Infogroup.IDMS.Web.Public.Controllers
{
    public class AboutController : IDMSControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}