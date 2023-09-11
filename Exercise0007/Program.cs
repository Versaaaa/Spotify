using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Exercise0007
{
    internal class Program
    {
        static void Main(string[] args)
        {
            #region Template
            //var records = new List<Song>();

            //Random rng = new Random();

            //int limit = rng.Next(50, 101);

            //int artistindex = 1;
            //int albumindex = 1;
            //int canzoniindex = 1;

            //for (int i = 0; i < limit; i++)
            //{
            //    int canzoni = rng.Next(1, 11);
            //    for (int j = 0; j < canzoni; j++)
            //    {
            //        int albumi = rng.Next(1, 6);
            //        for (int k = 0; k < albumi; k++)
            //        {
            //            records.Add(new Song($"Artist {artistindex}", $"Album {albumindex}", $"Song {canzoniindex}", $"Genere {rng.Next(1, 26)}", rng.Next(90, 301)));
            //            canzoniindex++; 
            //        }
            //        albumindex++;
            //    }
            //    artistindex++;
            //}

            //using (var writer = new StreamWriter(Directory.GetCurrentDirectory() + @"\..\..\..\..\songs.csv"))
            //using (var x = new CsvWriter(writer,CultureInfo.InvariantCulture))
            //{
            //    x.WriteRecords(records);
            //}


            #endregion

            Configuration.DefaultConfiguration();
            //UserInterface.CategorySelectionMenu();

        }
    }

    public class Configuration
    {
        public string CsvPath { get; set; }

        private static Configuration instance;
        public static Configuration Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Configuration();
                }
                return instance;
            }
        }

        Configuration()
        {
            CsvPath = "";
        }

        public static void DefaultConfiguration() 
        {
            Instance.CsvPath = $@"{Directory.GetCurrentDirectory()}\..\..\..\..\songs.csv";
            DataBank.Instance.LoadFromCsv();
        }

    }

    public class DataBank
    {
        
        protected static DataBank instance;

        public List<Song> Songs {  get; set; }
        
        public List<Album> Albums {  get; set; }
        
        public List<Artist> Artists {  get; set; }

        public static DataBank Instance 
        {
            get
            {
                if(instance == null)
                {
                    instance = new DataBank();
                }
                return instance;
            } 
        }
        
        private DataBank() { }

        public void LoadFromCsv()
        {
            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Encoding = Encoding.UTF8, 
                Delimiter = "," 
            };
            List<Song> records;

            using (var reader = new StreamReader(Configuration.Instance.CsvPath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();
                records = csv.GetRecords<Song>().ToList();
            }
            
            Songs = new List<Song>();
            Songs.AddRange(records);

            GenerateAlbums();
            GenerateArtists();

        }

        public void GenerateAlbums()
        {
            Albums = new List<Album>();
            //Songs.Select(x => new { x.album, x.artist }).Distinct().ToList().ForEach(x => Albums.Add(new Album(x.album, x.artist)));

            var x = Songs.GroupBy(x => x.album).ToList();
            foreach (var item in x)
            {
                
            }
        }

        public void GenerateArtists()
        {
            Artists = new List<Artist>();
            Songs.Select(x => new {x.artist}).Distinct().ToList().ForEach(x => Artists.Add(new Artist(x.artist)));
        }
    }

    public static class UserInterface
    {

        public static void ContentDisplay(ICollection<string> contents)
        {
            int i = 1;
            foreach (var content in contents)
            {
                Console.WriteLine($"{i} - {content}");
                i++;
            }
        }

        public static int ContentSelectionMenu(ICollection<string> contents)
        {
            int input = -1;
            do
            {
                Console.Clear();
                Console.Clear();
                ContentDisplay(contents);
                SongPlayerDisplay();
                Console.WriteLine("Seleziona indice corrispondente");
                try
                {
                    input = int.Parse(Console.ReadLine());
                    input--;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
            while (input > contents.Count - 1 || input < 0);

            return input;
        }

        public static void CategorySelectionMenu()
        {
            ConsoleKey input;
            do
            {
                Console.Clear();
                SongPlayerDisplay();
                Console.WriteLine("A - per Visualizzare tutti gli Artisti");
                Console.WriteLine("B - per Visualizzare tutti gli Album");
                Console.WriteLine("C - per Visualizzare tutti le Playlist");
                Console.WriteLine("D - per Visualizzare tutte le Canzoni");
                input = Console.ReadKey().Key;
            }
            while ( input != ConsoleKey.A && 
                    input != ConsoleKey.B && 
                    input != ConsoleKey.C &&
                    input != ConsoleKey.D);
            UserInterfaceCommand(input);
        }

        public static void SongPlayerDisplay()
        {
            try
            {
                string text = $"     Currently Playing : {SongPlayer.Instance.GetCurrentSong().toString()}     ";
                string spacing = "--------------------------------------------------------------------------------------------------------------------------------";
                spacing = spacing.Remove(text.Length);
                Console.WriteLine($"{spacing}\n{text}\n{spacing}");
            }
            catch (NullReferenceException)
            {
                Console.WriteLine(  "--------------------------------------------------------------------\n" +
                                    "                   Currently not playing any song                   \n" +
                                    "--------------------------------------------------------------------\n");
            }
        }

        public static void SongPlayerMenu()
        {
            ConsoleKey input;
            List<string> contents = new List<string>();
            contents.AddRange(SongPlayer.Instance.songs.Select(x => new string(x.title)).ToList());
            do
            {
                Console.Clear();
                ContentDisplay(contents);
                SongPlayerDisplay();
                Console.WriteLine("N - per Selezionare la Canzone Successiva");
                Console.WriteLine("P - per Selezionare la Canzone Precedente");
                Console.WriteLine($"O - Per Mettere in {(SongPlayer.Instance.currentlyPlaying ? "Pausa" : "Play")} la Canzone");
                input = Console.ReadKey().Key;
            }
            while ( input != ConsoleKey.N && 
                    input != ConsoleKey.P && 
                    input != ConsoleKey.O );
            UserInterfaceCommand(input);
        }

        public static void UserInterfaceCommand(ConsoleKey input)
        {
            switch (input)
            {   
                
                // 
                case ConsoleKey.F1:
                    break;

                
                // All Artist Selection Menu
                case ConsoleKey.A:
                    int i = ContentSelectionMenu(DataBank.Instance.Artists.Select(x => new string(x.name)).ToList());
                    i = ContentSelectionMenu(DataBank.Instance.Albums.Where(x => x.artist.Equals(DataBank.Instance.Artists[i].name)).Select(x => new string(x.title)).ToList());
                    SongPlayer.Instance.LoadMusic(DataBank.Instance.Songs.Where(x => x.album.Equals(DataBank.Instance.Albums[i].title)).ToList());
                    SongPlayerMenu();

                    break;
                
                // All Album Selection Menu
                case ConsoleKey.B:
                    ContentSelectionMenu(DataBank.Instance.Albums.Select(x => new string(x.title)).ToList());
                    break;
                
                // All Playlist Selection Menu
                case ConsoleKey.C:
                    break;

                // All Music Selection Menu
                case ConsoleKey.D:
                    
                    break;

                // Song Player Menu
                case ConsoleKey.E:
                    SongPlayerMenu();
                    break;

                // Next
                case ConsoleKey.N:
                    SongPlayer.Instance.Next();
                    break;
                
                // Previous
                case ConsoleKey.P:
                    SongPlayer.Instance.Previous();
                    break;
                
                // Pause and Play
                case ConsoleKey.O:
                    SongPlayer.Instance.PlayAndStop();
                    break;
            }
        }

    }

    public class SongPlayer
    {
        static SongPlayer instance;
        public List<Song> songs;
        int index = 0;
        public bool currentlyPlaying = false;
        int time;

        public static SongPlayer Instance
        { 
            get
            {
                if (instance == null)
                {
                    instance = new SongPlayer();
                }
                return instance;
            }
        }

        private SongPlayer() {}

        public void LoadMusic(ICollection<Song> songs)
        {
            index = 0;
            time = 0;
            currentlyPlaying = true;
            this.songs = new List<Song>() { };
            this.songs.AddRange(songs);
        }

        public bool PlayAndStop()
        {
            currentlyPlaying = currentlyPlaying ? false : true;
            return currentlyPlaying;
        }

        public Song GetCurrentSong()
        {
            return songs[index];
        }

        public Song Next()
        {
            try
            {
                time = 0;
                currentlyPlaying = true;
                index++;
                return songs[index];
            }
            catch (ArgumentOutOfRangeException)
            {
                index = 0;
                return songs[index];
            }
        }

        public Song Previous()
        {
            try
            {
                time = 0;
                currentlyPlaying = true;
                index--;
                return songs[index];
            }
            catch (ArgumentOutOfRangeException)
            {
                index = songs.Count - 1;
                return songs[index];
            }
        }
    }

    public class Song
    {
        public string title { get; set; }
        public string artist { get; set; }
        public string album { get; set; }
        public string genre { get; set; }
        public int duration { get; set; }

        public Song(string artist, string album, string title, string genre, int duration)
        {
            this.artist = artist;
            this.album = album;
            this.title = title;
            this.genre = genre;
            this.duration = duration;
        }

        public string toString()
        {
            return $"{title} from {album} by {artist}";
        }

    }

    public class Album
    {
        public string title { get; set; }
        
        public string artist { get; set; }
        
        public List<Song> songs { get; set; }

        public Album(string title, string artist)
        {
            this.title = title;
            this.artist = artist;
        }

        public string toString()
        {
            return $"{title} by {artist}";
        }

    }

    public class Artist
    {
        public string name { get; set; }

        public List<Album> albums { get; set; }

        public Artist(string name)
        {
            this.name = name;
        }
    }
}
