using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AMNHAC.Models;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace AMNHAC.Controllers
{
    public class DiscoverController : Controller
    {
        // GET: Discover

        DataClasses1DataContext data = new DataClasses1DataContext();
        VideoCF cf = new VideoCF();
        public ActionResult IndexDiscover()
        {
            /*var all_list = cf.Videos.ToList();
            SearchYouTube searchObject = new SearchYouTube();*/
            

            var all_playlist  = from ss in data.Playlists select ss;
            
            return View(all_playlist);
        }
       

    }
}