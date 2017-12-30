﻿using System;
using System.Collections.Generic;
using System.IO;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Newtonsoft.Json;

namespace asiye {
    class Program {
        static void Main(string[] args) {
            if (args.Length == 0) {
                Console.WriteLine("Please enter argument");
                Console.WriteLine("Usage: asiye [--export|--import|--help]");
                return;
            }
            switch (args[0]) {
                case "-h":
                case "help":
                case "--help":
                    Console.WriteLine("Help");
                    break;
                case "-e":
                case "export":
                case "--export":
                    Export();
                    break;
                case "-i":
                case "import":
                case "--import":
                    Console.WriteLine("Not finished!");
                    Import("out.json");
                    break;
                default:
                    Console.WriteLine("Invalid argument!");
                    break;
            }
        }

        static void Import(string filename) {
            List<Channel> Channels = ReadJsonFile(filename);
            PrintChannels(Channels);
        }

        static void Export() {
            Console.WriteLine("Hello Universe");

            IWebDriver driver = new ChromeDriver();

            LoginYoutube(driver);

            List<Channel> Channels = GetChannels(driver);

            PrintChannels(Channels);

            //WriteTextFile(channels, "out.txt");
            WriteJsonFile(Channels, "out.json");

            Console.ReadKey();
            driver.Close();

            Console.WriteLine("Okey!");
        }
        static void LoginYoutube(IWebDriver driver) {
            string message = "Please Login to Youtube, then press any key on console.";

            driver.Navigate().GoToUrl("https://www.youtube.com");

            Console.WriteLine(message);
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            js.ExecuteScript($"alert('{message}');");

            Console.ReadKey();
        }

        static List<Channel> GetChannels(IWebDriver driver) {
            string channels_url = "https://www.youtube.com/feed/channels";
            string classnames = "yt-simple-endpoint style-scope ytd-channel-renderer";
            List<Channel> channels = new List<Channel>();

            driver.Navigate().GoToUrl(channels_url);

            string url, name;
            IWebElement tempElement;
            foreach(IWebElement item in driver.FindElements(By.XPath($"//a[contains(@class, '{classnames}')]"))) {
                url = item.GetAttribute("href");
                name = item.FindElement(By.TagName("h3")).FindElement(By.TagName("span")).Text;
                Channel tempChannel = new Channel(name, url);
                channels.Add(tempChannel);
            }
            Console.WriteLine(channels.Count);
            return channels;
        }

        static void PrintChannels(List<Channel> channels) {
            foreach(Channel channel in channels) {
                Console.WriteLine($"{channel.name} {channel.url}");
            }
        }

        static void WriteJsonFile(List<Channel> channels, string filename) {
            string output = JsonConvert.SerializeObject(channels, Formatting.Indented);  
            Console.WriteLine(output);
            System.IO.File.WriteAllText(filename, output);
        }

        static List<Channel> ReadJsonFile(string filename) {
            List<Channel> channels;
            using (StreamReader r = new StreamReader(filename)) {
                string json = r.ReadToEnd();
                channels = JsonConvert.DeserializeObject<List<Channel>>(json);
            }
            return channels;
        }

        static void WriteTextFile(List<Channel> channels, string filename) {
            using (StreamWriter sw = new StreamWriter(filename)) {

                foreach (Channel channel in channels) {
                    sw.WriteLine($"{channel.name}|{channel.url}");
                }
            }
        }

        static List<Channel> ReadTextFile(string filename) {
            List<Channel> channels = new List<Channel>();
            char[] pipeSeparator = new char[] { '|' };
            string[] result;

            string line = "";
            using (StreamReader sr = new StreamReader(filename)) {
                while ((line = sr.ReadLine()) != null) {
                    result = line.Split(pipeSeparator, StringSplitOptions.None);
                    Channel tempChannel = new Channel(result[0], result[1]);
                    channels.Add(tempChannel);
                }
            }
            return channels;
        }
    }

    class Channel {
        public string name { get; set; }
        public string url { get; set; }
        public Channel(string name, string url) {
            this.name = name;
            this.url = url;
        }
    }
}
