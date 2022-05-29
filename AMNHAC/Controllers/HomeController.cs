using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AMNHAC.Models;
using Aspose.Html.DataScraping.MultimediaScraping;
using Google.Apis.Services;

using Google.Apis.YouTube.v3;


namespace AMNHAC.Controllers
{
    ///////////////////////////////////////////////////////

    public class SearchYouTube
    {
        public int ID { get; set; }


        public async Task<List<Video>> RunYouTube(string timkiem)
        {
            List<Video> vk = new List<Video>();


            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyDDefFSi5CM8Ns5PyCsAOGa72qqTrKRRIo",
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
                        var vs = new Video();
                        {
                            vs.title = searchResult.Snippet.Title;
                            vs.id = searchResult.Id.VideoId;
                            vs.link = searchResult.ETag;
                            vs.author = searchResult.Snippet.ChannelTitle;
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


        /////////////////
    }

    public class HomeController : Controller
    {



        DataClasses1DataContext data = new DataClasses1DataContext();

        SearchYouTube searchObject = new SearchYouTube();
        List<Video> test = new List<Video>();
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


            var vk = new Video();
            vk.title = form["Id"];
            //Youtube API


            test = await searchObject.RunYouTube(vk.title);



            if (vk.title != "")
            {
                if (test.Count == 0)
                {
                    ViewBag.Message = "Can't find!!!!!";
                    return View(test);
                }
                else
                {
                    ViewBag.Message = "Your Search Results!!";
                    return View(test);
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

        public async Task<ActionResult> Test(FormCollection form)
        {
            List<Video> vs = new List<Video>();
            var vk = new Video();

            vk.title = form["Id"];
            //Youtube API


            test = await searchObject.RunYouTube(vk.title);
            vs = new List<Video>(test);
            /*for(var item =0;item < test.Count;item++)
            {
                vk.title = test[item].title;
                vk.id = test[item].id;
                vk.author = test[item].author;

            }*/
            for (var item = 0; item < test.Count; item++)
            {
                vs[item].title = test[item].title;
                vs[item].id = test[item].id;
                vs[item].author = test[item].author;
                vs[item].link = test[item].link;
                if (vk.title == vs[item].title)
                {
                    data.Videos.InsertOnSubmit(vs[item]);
                }
            }
            data.SubmitChanges();
            //Sổ danh sách 
            var all_list = data.Videos.ToList();
            return View(all_list);
        }

    }
}