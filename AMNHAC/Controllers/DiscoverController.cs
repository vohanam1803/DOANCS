using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AMNHAC.Models;

namespace AMNHAC.Controllers
{
    public class DiscoverController : Controller
    {
        // GET: Discover


        DataClasses1DataContext data = new DataClasses1DataContext();
        public ActionResult IndexDiscover()
        {
            var all_playlist  = from ss in data.Playlists select ss;
            
            return View(all_playlist);
        }
    }
}