using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AMNHAC.Controllers
{
    [Authorize]
    public class MyMusicProfileController : Controller
    {
       private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: MyMusicProfile
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Post()
        {
            var currentClaims = await UserManager.GetClaimsAsync(HttpContext.User.Identity.GetUserId());

            var accesstoken = currentClaims.FirstOrDefault(x => x.Type == "urn:tokens:facebook");  //currentClaims.FirstOrDefault(x => x.Type == "urn:tokens:facebook")

            if (accesstoken == null)
            {
                return (new HttpStatusCodeResult(HttpStatusCode.NotFound, "Token not found"));
            }
            string url = String.Format("https://graph.facebook.com/v14.0/me?fields=id,name,picture,email&access_token={0}", accesstoken) ;
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

            request.Method = "GET";

            using (HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());

                string result = await reader.ReadToEndAsync();

                dynamic jsonObj = System.Web.Helpers.Json.Decode(result);
                ViewBag.JSON = result;
            }
                return View();
        }
    }
}