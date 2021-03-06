﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


/*
    TUTORIAL MIT VIDEO siehe:

    http://frank-it-beratung.com/blog/2016/12/15/tutorial-ein-social-bot-im-eigenbau-mit-csharp/
        
    ACHTUNG: nicht alles, was technisch möglich ist, ist bei Facebook
    erlaubt oder erwünscht - bitte genau die Plattformrichtlinien studieren:

    https://developers.facebook.com/policy
    
    */


namespace SocialBotTutorial
{
    class Program
    {
        static void Main(string[] args)
        {

            string[] stadt = { "Stuttgart", "Berlin", "Hamburg" };
            string wetter, temperatur;
            string[] aktion = { "ins Kino", "ins Museum", "was essen" };
            string[] frage = { "Was macht ihr so?", "Wer hat einen Tipp für mich?", "Wer kommt mit?" };

            Random r = new Random();
            int zufallStadt = r.Next(0, 3);
 
            string urlWertherApi 
                = "http://api.openweathermap.org/data/2.5/weather?mode=xml&units=metric&lang=de";

            // kostenlos zu erhalten unter http://openweathermap.org/
            string appIdWeatherApi = "???";

            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(urlWertherApi 
                   + "&q=" + stadt[zufallStadt]
                   + "&APPID=" + appIdWeatherApi);                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            wetter=doc.GetElementsByTagName("weather")[0]
                .Attributes.GetNamedItem("value").Value;
            temperatur=doc.GetElementsByTagName("temperature")[0]
                .Attributes.GetNamedItem("value").Value;
                        
            int zufallAktion = r.Next(0, 3);
            int zufallFrage = r.Next(0, 3);

            string ausgabe = "Ich bin derzeit in " + stadt[zufallStadt]
                + " und hier ist es gerade " + wetter
                + " bei " + temperatur + "°C."
                + " Ich glaub ich geh jetzt gleich " + aktion[zufallAktion]
                + ". " + frage[zufallFrage];

            Console.WriteLine(ausgabe);

            //kann z. B. mit dem Access-Token-Manager erstellt werden:
            //http://frank-it-beratung.com/blog/access-token-manager/
            string accessToken = "???";

            byte[] nachricht;
            byte[] antwort;

            //auf eigene FB-Seite posten
            string graphURL ="https://graph.facebook.com/me/feed";
            
            //oder auf eine eigene Seite:
            //string graphURL = "https://graph.facebook.com/Eine.lustige.Testseite/feed";

            WebClient myWebClient = new WebClient();

            //Except100 ausschalten, sonst gibt es u. U. Fehlermeldungen
            ServicePointManager.Expect100Continue = false;

            nachricht = Encoding.UTF8.GetBytes("message=" + ausgabe
              + "&access_token=" + accessToken);

            try
            {
                antwort = myWebClient.UploadData(graphURL, nachricht);
                //Erfolgsmeldung (ID des Posts) in JSON
                Console.WriteLine("JSON-Response: \n\n" 
                    + System.Text.Encoding.ASCII.GetString(antwort));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fehler: \n\n" + ex.Message);
            }

            Console.ReadLine();
            
        }
    }
}
