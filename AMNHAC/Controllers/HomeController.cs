using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

    public class SearchYouTube
    {

        public int ID { get; set; }


        public async Task<List<Models.Video>> RunYouTube(string timkiem)
        {
            List<Models.Video> vk = new List<Models.Video>();


            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyA044O5L7G9VVMeAIxytPrqOMoUaa6v6_o",
                ApplicationName = this.GetType().ToString()
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = timkiem; // Replace with your search term.
            searchListRequest.MaxResults = 5;

            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync();

            List<string> videos = new List<string>();
            List<string> channels = new List<string>();
            List<string> playlists = new List<string>();




            // Add each result to the appropriate list, and then display the lists of
            // matching videos, channels, and playlists.

            foreach (var searchResult in searchListResponse.Items)
            {
                switch (searchResult.Id.Kind)
                {
                    case "youtube#video":
                        videos.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.VideoId));
                        var vs = new Models.Video();
                        {
                            vs.title = searchResult.Snippet.Title;
                            vs.id = searchResult.Id.VideoId;
                            vs.link = searchResult.ETag;


                        }

                        vk.Add(vs);

                        break;

                    case "youtube#channel":
                        channels.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.ChannelId));
                        break;

                    case "youtube#playlist":
                        playlists.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.PlaylistId));
                        break;
                }



            }



            Console.WriteLine(String.Format("Videos:\n{0}\n", string.Join("\n", videos)));
            Console.WriteLine(String.Format("Channels:\n{0}\n", string.Join("\n", channels)));
            Console.WriteLine(String.Format("Playlists:\n{0}\n", string.Join("\n", playlists)));


            return vk;
        }

       
    }

    public class HomeController : Controller
    {
        VideoCF cf = new VideoCF();
        Models.Video vk = new Models.Video();
        public ActionResult Index()
        {


            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Create(FormCollection form)
        {

            var all_list = cf.Videos.ToList();


            var vk = new Models.Video();
            vk.title = form["Id"];


            SearchYouTube searchObject = new SearchYouTube();
            all_list = await searchObject.RunYouTube(vk.title);

            

            if (vk.title != "")
            {
                if(all_list.Count == 0)
                {
                    ViewBag.Message = "Can't find!!!!!";
                    return View(all_list);
                }
                else
                {
                    ViewBag.Message = "Your Search Results!!";
                    return View(all_list);
                }
                
            }
            else
            {
                return View("~/Views/Home/Index.cshtml");
            }
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}