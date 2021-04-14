using HtmlAgilityPack;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите адрес страницы сайта: ");
            string url = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(url)) return;

            // declare html document
            var document = new HtmlWeb().Load(url);

            // now using LINQ to grab/list all images from website
            var ImageURLs = document.DocumentNode.Descendants("img")
                                            .Select(e => e.GetAttributeValue("src", null))
                                            .Where(s => !String.IsNullOrEmpty(s))
                                            .Take(5)
                                            .ToList();

            // Get path
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // Create a file
            using (StreamWriter writer = new StreamWriter(path + "\\ImageURLs.txt"))
            {
                for (var i = 0; i < ImageURLs.Count; i++)
                {
                    if (!ImageURLs[i].StartsWith("http")) continue;
                   
                    // now showing all images from web page one by one
                    Console.WriteLine(ImageURLs[i]);

                    // now writing all images from web page one by one
                    writer.WriteLine(ImageURLs[i]);

                    string extension = (string.IsNullOrWhiteSpace(Path.GetExtension(ImageURLs[i])) ? ".jpg" : Path.GetExtension(ImageURLs[i]));
                    string saveLocation = Path.Combine(path, "images", i + extension);
                    
                    // Download imagies from urls one by one
                    DownloadImageAsync(saveLocation, ImageURLs[i]);
                }
            }  

            Console.ReadKey();
        }

        private static void DownloadImageAsync(string saveLocation, string imageUrl)
        {
            byte[] imageBytes;
            HttpWebRequest imageRequest = (HttpWebRequest)WebRequest.Create(imageUrl);
            WebResponse imageResponse = imageRequest.GetResponse();

            Stream responseStream = imageResponse.GetResponseStream();

            using (BinaryReader br = new BinaryReader(responseStream))
            {
                imageBytes = br.ReadBytes(500000);
                br.Close();
            }
            responseStream.Close();
            imageResponse.Close();

            System.IO.FileInfo file = new System.IO.FileInfo(saveLocation);
            file.Directory.Create(); // If the directory already exists, this method does nothing.
            FileStream fs = new FileStream(saveLocation, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            try
            {
                bw.Write(imageBytes);
            }
            finally
            {
                fs.Close();
                bw.Close();
            }
        }
    }
}
