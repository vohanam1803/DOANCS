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

        //
        [HttpGet]

        public ActionResult Index()
        {

            dynamic mymodel = new ExpandoObject();
            mymodel.person = GetPerson();


            mymodel.videoTQ = from ss in data.Videos where ss.loaivideo != "user" && ss.idTheloai == 1 select ss;

            mymodel.videoVN = from ss in data.Videos where ss.loaivideo != "user" && ss.idTheloai == 2 select ss;
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
            var all_check = from ss in data.Videos where ss.loaivideo != "user" select ss;
            if (all_list.Count == 0)
            {
                ViewBag.Message = "You Not Have Anything In Playlist";
                return View(all_check);
            }
            else
            {
                ViewBag.Message = "Your Playlist";
                return View("~/Views/MyMusicProfile/Index.cshtml", all_check);
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
        public ActionResult AddPlaylist(FormCollection form)
        {
            List<Video> vs = new List<Video>();
            var vk = new Video();


            //Get id bài hát kiểm tra có bị trùng trong data ko
            vk.id = form["CheckId"];
            var checkId = data.Videos.Where(m => m.id == vk.id ).FirstOrDefault();

            test = data.Videos.ToList();
            vs = new List<Video>(test);


            for (var item = 0; item < test.Count; item++)
            {

                vs[item].title = test[item].title;
                vs[item].id = test[item].id;
                vs[item].author = test[item].author;
                vs[item].link = test[item].link;
                vs[item].loaivideo = test[item].loaivideo;
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


        public ActionResult TrangChu()
        {

            ViewBag.Message = "You Can Search AnyThing Add Your Playlist And Controll Trang Chủ!!";
            /*var trangchu = from ss in data.Videos where ss.loaivideo != "user" select ss;*/
            dynamic mymodel = new ExpandoObject();
            mymodel.person = GetPerson();

            mymodel.trangchu = from ss in data.Videos where ss.loaivideo != "user" select ss;
            mymodel.videoTQ = from ss in data.Videos where ss.loaivideo != "user" && ss.idTheloai == 1 select ss;

            mymodel.videoVN = from ss in data.Videos where ss.loaivideo != "user" && ss.idTheloai == 2 select ss;
            return View(mymodel);
            /*return View(trangchu);*/

        }


        [HttpPost]
        public async Task<ActionResult> AddPlaylistAdmin(FormCollection form)
        {
            int a = 0;
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

            //Check thể loại video
            var check = new Theloai();
            check.nameTheloai = form["theloai"];



            for (var item = 0; item < test.Count; item++)
            {
                vs[item].id = test[item].id;
                vs[item].title = test[item].title;
                vs[item].author = test[item].author;
                vs[item].link = test[item].link;
                vs[item].vitrivideo = vs[item].id + a;
                vs[item].loaivideo = "admin" + vs[item].id;
                if (check.nameTheloai == "hoa")
                {
                    vs[item].idTheloai = check.idTheloai = 1;

                }
                if (check.nameTheloai == "viet")
                {
                    vs[item].idTheloai = check.idTheloai = 2;

                }
                if (check.nameTheloai == "khac")
                {
                    vs[item].idTheloai = check.idTheloai = 4;

                }
                a++;
                if (checkId == default)
                {
                    if (vk.title == vs[item].title)
                    {
                        data.Videos.InsertOnSubmit(vs[item]);
                    }
                }
                else
                {
                    ViewBag.Message = "This Have In Your PlayList !!";
                    dynamic mymodel = new ExpandoObject();
                    mymodel.person = GetPerson();

                    mymodel.trangchu = from ss in data.Videos where ss.loaivideo != "user" select ss;
                    mymodel.videoTQ = from ss in data.Videos where ss.loaivideo != "user" && ss.idTheloai == 1 select ss;

                    mymodel.videoVN = from ss in data.Videos where ss.loaivideo != "user" && ss.idTheloai == 2 select ss;
                    return View("~/Views/Home/TrangChu.cshtml", mymodel);
                   /* ViewBag.Message = "This Have In Your PlayList !!";
                    return View("~/Views/Home/TrangChu.cshtml", test);*/
                }
            }
            data.SubmitChanges();
            //Sổ danh sách 

            var all_list = data.Videos.ToList();

            /*var all_sach = from ss in data.Videos where ss.loaivideo != "user" select ss;*/

            if (all_list.Count == 0)
            {
                ViewBag.Message = "You Not Have Anything In Playlist";
                dynamic mymodel = new ExpandoObject();
                mymodel.person = GetPerson();

                mymodel.trangchu = from ss in data.Videos where ss.loaivideo != "user" select ss;
                mymodel.videoTQ = from ss in data.Videos where ss.loaivideo != "user" && ss.idTheloai == 1 select ss;

                mymodel.videoVN = from ss in data.Videos where ss.loaivideo != "user" && ss.idTheloai == 2 select ss;
                return View("~/Views/Home/TrangChu.cshtml", mymodel);
                /*ViewBag.Message = "You Not Have Anything In Playlist";
                return View("~/Views/Home/TrangChu.cshtml", all_sach);*/
            }
            else
            {
                ViewBag.Message = "Your Playlist";
                dynamic mymodel = new ExpandoObject();
                mymodel.person = GetPerson();

                mymodel.trangchu = from ss in data.Videos where ss.loaivideo != "user" select ss;
                mymodel.videoTQ = from ss in data.Videos where ss.loaivideo != "user" && ss.idTheloai == 1 select ss;

                mymodel.videoVN = from ss in data.Videos where ss.loaivideo != "user" && ss.idTheloai == 2 select ss;
                return View("~/Views/Home/TrangChu.cshtml", mymodel);
                /*ViewBag.Message = "Your Playlist";
                return View("~/Views/Home/TrangChu.cshtml", all_sach);*/
            }
        }



        /// <summary>
        /// //////////
        /// </summary>
        /// <returns></returns>
        /*private async Task<List<Video>> GetVideoAsyncTQ()
        {
            int a = 0;

            test = await searchObject.RunYouTube("Nhạc Hot Trung Quốc TikTokS 2022");
            List<Video> video = new List<Video>();
            video = test;
            for (var item = 0; item < video.Count; item++)
            {
                video[item].vitrivideo = video[item].id + a;
                video[item].loaivideo = "1" + a;
                a++;
            }
            return video;
        }*/
        /*private async Task<List<Video>> GetVideoAsyncVN()
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
        }*/
        [HttpPost]
        public ActionResult DetelePlaylistAdmin(string id, FormCollection collection)
        {
            var D_playlist = data.Videos.Where(m => m.id == id).First();
            data.Videos.DeleteOnSubmit(D_playlist);
            data.SubmitChanges();
            var AfterD = data.Videos.ToList();
            /*var all_sach = from ss in data.Videos where ss.loaivideo != "user" select ss;*/


            dynamic mymodel = new ExpandoObject();
            mymodel.person = GetPerson();

            mymodel.trangchu = from ss in data.Videos where ss.loaivideo != "user" select ss;
            mymodel.videoTQ = from ss in data.Videos where ss.loaivideo != "user" && ss.idTheloai == 1 select ss;

            mymodel.videoVN = from ss in data.Videos where ss.loaivideo != "user" && ss.idTheloai == 2 select ss;
            
            for(var item = 0;item < AfterD.Count;item++)
            {
                if(AfterD[item].loaivideo != "user")
                {
                    ViewBag.Message = "Your Playlist";
                    return View("~/Views/Home/TrangChu.cshtml", mymodel);
                }
                
            }
            ViewBag.Message = "You Not Have Anything In Playlist";
            return View("~/Views/Home/TrangChu.cshtml", mymodel);
            /*if (AfterD.Count == 0)
            {
                ViewBag.Message = "You Not Have Anything In Playlist";
                return View("~/Views/Home/TrangChu.cshtml", mymodel);
                *//* return View("~/Views/Home/TrangChu.cshtml", all_sach);*//*
            }
            else
            {
                ViewBag.Message = "Your Playlist";
                return View("~/Views/Home/TrangChu.cshtml", mymodel);
                *//*return View("~/Views/Home/TrangChu.cshtml", all_sach);*//*
            }*/
        }
        [HttpPost]
        public ActionResult SearchInPlaylist(FormCollection collection)
        {
            
            string timkiem = collection["SearchInPlaylist"];
            var D_playlist = data.Videos.ToList();
            /*var list = new List<Video>(D_playlist.Count);*/
            for (var item = 0; item < D_playlist.Count; item++)
            {
                if (D_playlist[item].title.Contains(timkiem))
                {
                    D_playlist[item].vitrivideo = "1";
                    /*list[item].id = D_playlist[item].id;
                    list[item].title = D_playlist[item].title;
                    list[item].author = D_playlist[item].author;
                    list[item].link = D_playlist[item].link;
                    list[item].vitrivideo = D_playlist[item].vitrivideo;
                    list[item].loaivideo = D_playlist[item].loaivideo;
                    list[item].idTheloai = D_playlist[item].idTheloai;*/
                }
            }
            var list = from ss in D_playlist where ss.vitrivideo == "1" && ss.loaivideo != "user" select ss;

            dynamic mymodel = new ExpandoObject();
            mymodel.person = GetPerson();

            mymodel.trangchu = list;
            mymodel.videoTQ = from ss in data.Videos where ss.loaivideo != "user" && ss.idTheloai == 1 select ss;

            mymodel.videoVN = from ss in data.Videos where ss.loaivideo != "user" && ss.idTheloai == 2 select ss;
            return View("~/Views/Home/TrangChu.cshtml",mymodel);
            /*return View("~/Views/Home/TrangChu.cshtml", list);*/
        }


        

    }
}
