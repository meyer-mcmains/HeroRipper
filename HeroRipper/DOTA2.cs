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
            File.Delete(Application.StartupPath + @"\List\HeroList.txt");

            var html = @"http://dota2.gamepedia.com/Heroes";
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);

            var htmlNodes = htmlDoc.DocumentNode.SelectNodes("//tr//td//div//div");

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
                        WriteHeroList(key.Remove(0, 1).Replace('_', ' '));
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

            if (hero == "/Nature%27s_Prophet")
            {
                hero = "/Nature's_Prophet";
            }

            Hero h = new Hero()
            {
                Name = hero.Remove(0, 1).Replace('_', ' ')
            };

            RipBasic(form, h, htmlDoc);

            RipRoles(form, h, htmlDoc);

            BuildJSON(h);
        }

        public void RipBasic(Form1 form, Hero h, HtmlAgilityPack.HtmlDocument doc)
        {
            int counter = 0;
            var htmlNodes = doc.DocumentNode.SelectNodes("//p//a");
            HtmlNodeCollection childNodes = htmlNodes;

            form.SetOutput("Getting Basic Hero Info for " + h.Name + "\n");

            foreach (var node in childNodes)
            {
                if (node.NodeType == HtmlNodeType.Element)
                {
                    form.SetOutput(node.InnerHtml + "\n");
                    counter++;
                }
                if (counter == 1)
                {
                    h.AttackRange = node.InnerHtml;
                }
                else if (counter == 2)
                {
                    h.PrimaryAttribute = node.InnerHtml;
                    break;
                }
            }
        }

        public void RipRoles(Form1 form, Hero h, HtmlAgilityPack.HtmlDocument doc)
        {
            form.SetOutput("Grabbing Roles of " + h.Name + "\n");
            var htmlNodes = doc.DocumentNode.SelectNodes("//tr//td//a[@title='Role']");
            HtmlNodeCollection childNodes = htmlNodes;
            foreach (var node in childNodes)
            {
                if (node.NodeType == HtmlNodeType.Element)
                {
                    form.SetOutput(node.InnerHtml + "\n");
                    h.Roles.Add(node.InnerHtml);
                }
            }
        }

        public void BuildJSON(Hero h)
        {
            JObject heroJson = new JObject(
                new JProperty("Name", h.Name),
                new JProperty("Attack Range", FirstToUppercase(h.AttackRange)),
                new JProperty("Primary Attribute", FirstToUppercase(h.PrimaryAttribute)),
                new JProperty("Roles", h.Roles));


            using (StreamWriter file = File.CreateText(Application.StartupPath + @"\heroes\" + h.Name + ".json"))
            using (JsonTextWriter writer = new JsonTextWriter(file))
            {
                heroJson.WriteTo(writer);
            }
        }

        public void WriteHeroList(string name)
        {
            if (name == "Nature%27s Prophet")
            {
                name = "Nature's Prophet";
            }

            using (StreamWriter file = new StreamWriter(Application.StartupPath + @"\List\HeroList.txt", true))
            {
                file.WriteLine(name);
            }
        }

        public string FirstToUppercase(string lower)
        {
            if (string.IsNullOrEmpty(lower))
            {
                return string.Empty;
            }
            return char.ToUpper(lower[0]) + lower.Substring(1);
        }
    }
}
