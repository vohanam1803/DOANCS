using System;
using System.Collections.Generic;
using System.Dynamic;
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
            searchListRequest.MaxResults = 6;

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
                            vs.link = searchResult.Snippet.Description;
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



        //
        private List<Person> GetPerson()
        {
            List<Person> person = new List<Person>();
            person = data.Persons.ToList();
            return person;
        }
        private async Task<List<Video>> GetVideoAsyncTQ()
        {
            int a = 0;
            test = await searchObject.RunYouTube("火爆的中国抖音音乐 2022");
            List<Video> video = new List<Video>();
            video = test;
            for (var item = 0; item < video.Count; item++)
            {
                video[item].vitrivideo = video[item].id + a;
                video[item].loaivideo = "1" + a;
                a++;
            }
            return video;

        }
        private async Task<List<Video>> GetVideoAsyncVN()
        {
            int a = 0;

            test = await searchObject.RunYouTube("Nhạc Hot Việt Nam 2022");
            List<Video> video = new List<Video>();
            video = test;
            for (var item = 0; item < video.Count; item++)
            {
                video[item].vitrivideo = video[item].id + a;
                video[item].loaivideo = "2" + a;
                a++;
            }
            return video;
        }
        //
        [HttpGet]

        public async Task<ActionResult> Index()
        {

            dynamic mymodel = new ExpandoObject();
            mymodel.person = GetPerson();
            mymodel.videoTQ = await GetVideoAsyncTQ();
            mymodel.videoVN = await GetVideoAsyncVN();
            return View(mymodel);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View("~/Views/Home/Index.cshtml");
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
            //Get tên bài hát muốn thêm vào
            vk.title = form["Id"];

            //Get id bài hát kiểm tra có bị trùng trong data ko
            vk.id = form["CheckId"];
            var checkId = data.Videos.Where(m => m.id == vk.id).FirstOrDefault();

            //Youtube API
            test = await searchObject.RunYouTube(vk.title);
            vs = new List<Video>(test);

            for (var item = 0; item < test.Count; item++)
            {

                vs[item].title = test[item].title;
                vs[item].id = test[item].id;
                vs[item].author = test[item].author;
                vs[item].link = test[item].link;
                vs[item].loaivideo = "user";
                if (checkId == default)
                {
                    if (vk.title == vs[item].title)
                    {
                        data.Videos.InsertOnSubmit(vs[item]);
                    }
                }
                else
                {
                    ViewBag.Check = "This Have In Your PlayList !!";
                    return View("~/Views/Home/Create.cshtml", test);
                }
            }
            data.SubmitChanges();
            //Sổ danh sách 
            var all_list = data.Videos.ToList();

            if (all_list.Count == 0)
            {
                ViewBag.Message = "You Not Have Anything In Playlist";
                return View(all_list);
            }
            else
            {
                ViewBag.Message = "Your Playlist";
                return View(all_list);
            }
        }

        [HttpGet]
        public ActionResult DetelePlaylist()
        {
            var AfterD = data.Videos.ToList();
            return View("~/Views/MyMusicProfile/Index.cshtml", AfterD);
        }
        [HttpPost]
        public ActionResult DetelePlaylist(string id, FormCollection collection)
        {
            var D_playlist = data.Videos.Where(m => m.id == id).First();
            data.Videos.DeleteOnSubmit(D_playlist);
            data.SubmitChanges();
            var AfterD = data.Videos.ToList();
            if (AfterD.Count == 0)
            {
                ViewBag.Message = "You Not Have Anything In Playlist";
                return View("~/Views/MyMusicProfile/Index.cshtml", AfterD);
            }
            else
            {
                ViewBag.Message = "Your Playlist";
                return View("~/Views/MyMusicProfile/Index.cshtml", AfterD);
            }
        }
        [HttpPost]
        public async Task<ActionResult> AddPlaylist(FormCollection form)
        {
            List<Video> vs = new List<Video>();
            var vk = new Video();


            //Get id bài hát kiểm tra có bị trùng trong data ko
            vk.id = form["CheckId"];
            var checkId = data.Videos.Where(m => m.id == vk.id).FirstOrDefault();

            test = await GetVideoAsyncTQ();
            vs = new List<Video>(test);


            for (var item = 0; item < test.Count; item++)
            {

                vs[item].title = test[item].title;
                vs[item].id = test[item].id;
                vs[item].author = test[item].author;
                vs[item].link = test[item].link;
                vs[item].loaivideo = "admin";
                if (checkId == default)
                {
                    if (vk.id == vs[item].id)
                    {
                        data.Videos.InsertOnSubmit(vs[item]);
                    }
                }
                else
                {
                    ViewBag.Check = "This Have In Your PlayList !!";
                    return View("~/Views/Home/Create.cshtml", test);
                }

            }
            data.SubmitChanges();
            //Sổ danh sách 
            var all_list = data.Videos.ToList();
            ViewBag.Message = "Your Playlist Have Been Update";
            return View("~/Views/MyMusicProfile/Index.cshtml", all_list);

        }

        public ActionResult TrangAdmin()
        {
           
            return View();
        }

        [HttpGet]
        public ActionResult TrangChu()
        {
            List<Video> acc = new List<Video>();
            return View("~/Views/Home/TrangChu.cshtml", acc);
        }
        [HttpPost]
        public async Task<ActionResult> TrangChu(FormCollection form)
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

                return View("~/Views/Home/TrangChu.cshtml");
            }

        }
    }
}