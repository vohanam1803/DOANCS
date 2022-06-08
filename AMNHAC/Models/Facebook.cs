using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMNHAC.Models
{
    public class Facebook
    {
        public dynamic jsonObj { get; set; }
        public Facebook(dynamic json)
        {
            jsonObj = json;
            if (jsonObj != null)
            {
                id = jsonObj.id;
                name = jsonObj.name;
                if (jsonObj.picture != null)
                {
                    picture = new picture(jsonObj.picture);
                }
            }
        }
        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public picture picture { get; set; }
    }

    public class picture
    {
        public dynamic jsonObj { get; set; }
        public picture(dynamic json)
        {
            jsonObj = json;
            if (jsonObj != null)
            {
                if (jsonObj.data != null)
                {
                    data = new data(jsonObj.data);
                }
            }
        }
        public data data { get; set; }
    }

    public class data
    {
        public dynamic jsonObj { get; set; }
        public data(dynamic json)
        {
            jsonObj = json;

            url = jsonObj.url;
        }
        public string url { get; set; }
    }
}