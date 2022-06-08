using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
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


            var all_playlist = data.Videos.ToList();

            return View(all_playlist);
        }

        
       
        public ActionResult VideoPlay()
        {
            var video = data.Videos.ToList();
            return View(video);
        }

        [HttpPost]
        public ActionResult VideoPlay(FormCollection form)
        {
            string id = form["Id"];
            string value = form["Id1"];
            var video = data.Videos.ToList();
            for(var item = 0;item<video.Count;item++)
            {
                if(video[item].id == value)
                {
                    if (id == "")
                    {

                        video[item].link = id;

                    }
                    else
                    {
                        video[item].link = id;
                    }
                }
            }
            
           
            return View(video);
        }
    }
}