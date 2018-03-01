using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace APIs.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
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

        public ActionResult GetWeatherData()
        {
            //Make a request but don't send it yet

            //With this movie API, you put certain parameters into the URL, pay close attention to the documentation 
            //https://forecast.weather.gov/MapClick.php?lat=42.335722&lon=-83.049944&FcstType=json
            //HttpWebRequest request = WebRequest.CreateHttp("http://www.omdbapi.com/?apikey=48e02a80&s=star wars");
            HttpWebRequest request = WebRequest.CreateHttp("https://forecast.weather.gov/MapClick.php?lat=42.335722&lon=-83.049944&FcstType=json");

            //Tell it the list of browsers we're using
            request.UserAgent = @"User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.116 Safari/537.36";

            //If you need to use OAuth or Keys there will be a few extra steps right around here you go on to 
            //grab a response.
            //push the request over to the remote server 
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            //Parse the response data (this looks a lot like reading in a text file, file I/O)
            StreamReader rd = new StreamReader(response.GetResponseStream());

            //Return the data in string format 
            String data = rd.ReadToEnd();

            //This is where things change based upon whether we're using XML or Json
            //Personally I prefer JSON, but they're equivalent to each other
            JObject o = JObject.Parse(data);

            //Now we can step through the JSON data 
            //the way to approach this is to think of every tag either contains a string array or points 
            //to another list. As you try to construct this always always have the JSON viewer open
            //With the array portion you can use  the .ToList() or ToArray() methods to make a collection
            //of JTokens

            //You can step through data just like an array
            //Json works as a series of nested tags 
            //So for example on our weather data has a time header tag 
            //if we want the first item from inside of there we got o["weather"][0]
            List<JToken> times = o["time"]["startPeriodName"].ToList();
            List<string> temps = new List<string>();
            ////https://stackoverflow.com/questions/9198426/mvc3-putting-a-newline-in-viewbag-text
            ////You want the front end to care about presenting data, so we do our newlines there

            for (int i = 0; i < o["data"]["temperature"].Count(); i++)
            {
                string timeLabel = times[i].ToString();
                string input = o["time"]["startPeriodName"][i] + " " + o["time"]["tempLabel"][i] + " " + o["data"]["temperature"][i].ToString();

                temps.Add(input);
            }
            ViewBag.AllTemps = temps;

            return View("Data");
        }

        public ActionResult Data()
        {
            return GetWeatherData();
        }

        public ActionResult SearchByNumber(int pokeNum)
        {
            HttpWebRequest request = WebRequest.CreateHttp("http://pokeapi.co/api/v2/pokemon/" + pokeNum);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader rd = new StreamReader(response.GetResponseStream());

            String data = rd.ReadToEnd();

            JObject o = JObject.Parse(data);

            ViewBag.name = o["name"].ToString();
            ViewBag.Data = o["sprites"]["front_default"].ToString();
            ViewBag.Type1 = o["types"][0]["type"]["name"].ToString();
            try
            {
                ViewBag.Type2 = o["types"][1]["type"]["name"].ToString();
            }
            catch (Exception e)
            {

            }
            return View("PokeResult");
        }

        public ActionResult SearchByName(string pokeName)
        {
            HttpWebRequest request = WebRequest.CreateHttp("http://pokeapi.co/api/v2/pokemon/" + pokeName + "/");
            HttpWebResponse response;
            Session["pokeName"] = pokeName;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception e)
            {

                return RedirectToAction("Error");
            }
            StreamReader rd = new StreamReader(response.GetResponseStream());

            String data = rd.ReadToEnd();

            JObject o = JObject.Parse(data);

            ViewBag.name = o["name"].ToString();
            ViewBag.Data = o["sprites"]["front_default"].ToString();
            ViewBag.Type1 = o["types"][0]["type"]["name"].ToString();
            try
            {
                ViewBag.Type2 = o["types"][1]["type"]["name"].ToString();
            }
            catch(Exception e)
            {
                
            }
            return View("PokeResult");
        }

        public ActionResult PokePicker()
        {
            return View();
        }

        public ActionResult PokeResult()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}