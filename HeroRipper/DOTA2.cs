using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

namespace HeroRipper
{
    public class DOTA2
    {
        public void Rip(Form1 form)
        {
            form.SetOutput("Starting to Rip Heroes... \n");
            GetHeroes(form);
        }

        public void GetHeroes(Form1 form)
        {
            var html = @"http://dota2.gamepedia.com/Heroes";
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);

            var htmlNodes = htmlDoc.DocumentNode.SelectNodes("//tr/td/div/div");

            HtmlNodeCollection childNodes = htmlNodes;

            foreach (var node in childNodes)
            {
                if (node.NodeType == HtmlNodeType.Element)
                {
                    Regex regex = new Regex(@"<a\s+(?:[^>]*?\s+)?href=([""'])(.*?)\1");
                    Match match = regex.Match(node.InnerHtml);
                    if (match.Success)
                    {
                        string key = match.Groups[2].Value;
                        form.SetOutput(key.Remove(0, 1).Replace('_', ' ') + "\n");
                        CrawlPage(key, form);
                        if (key == "/Zeus")
                        {
                            break;
                        }
                    }
                }
            }

        }

        public void CrawlPage(string hero, Form1 form)
        {
            var html = @"http://dota2.gamepedia.com" + hero;
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);
            var htmlNodes = htmlDoc.DocumentNode.SelectNodes("//p//a");
            HtmlNodeCollection childNodes = htmlNodes;
            int counter = 0;

            string heroName = hero.Remove(0, 1).Replace('_', ' ');
            string attackType = null;
            string mainAttrib = null;

            foreach (var node in childNodes)
            {
                if (node.NodeType == HtmlNodeType.Element)
                {
                    form.SetOutput(node.InnerHtml + "\n");
                    counter++;
                }
                if (counter == 1)
                {
                    attackType = node.InnerHtml;
                }
                else if (counter == 2)
                {
                    mainAttrib = node.InnerHtml;
                    counter = 0;
                    break;
                }
            }

            JObject heroJson = new JObject(
                new JProperty(heroName, attackType, mainAttrib));

            using (StreamWriter file = File.CreateText(Application.StartupPath + @"\heroes\" + heroName + ".json"))
            using (JsonTextWriter writer = new JsonTextWriter(file))
            {
                heroJson.WriteTo(writer);
            }
        }
    }
}
