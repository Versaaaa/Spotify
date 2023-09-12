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
            UserInterface.CategorySelectionMenu();

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
            /* Old
            Songs.Select
                (
                    x => new { x.album, x.artist }
                )
                .Distinct()
                .Select
                (
                    x => new Album(x.album, x.artist)
                )
                .ToList()
                .ForEach
                (
                    x =>
                    { 
                        x.songs.AddRange
                        (
                            Songs.Where
                            (
                                y => y.album == x.title
                            )
                            .ToList()
                        ); 
                        Albums.Add(x); 
                    }
                );
            */

            Albums = new List<Album>();
            Albums.AddRange
            (
                Songs.GroupBy(x => x.album)
                .Select
                (
                    group => new Album
                    (
                        group.Key,
                        group.Select(x => x.artist).Distinct().FirstOrDefault()
                    )
                    {
                        songs = group.Select(x => x).ToList() 
                    }
                )
                .ToList()
            );
        }

        public void GenerateArtists()
        {
            /* Old
            Songs.Select
            (
                x => new { x.artist }
            )
            .Distinct()
            .Select
            (
                x => new Artist(x.artist)
            )
            .ToList()
            .ForEach
            (
                x => 
                { 
                    x.albums.AddRange
                    (
                        Albums.Where
                        (
                            y => y.artist == x.name)
                            .ToList()
                        ); 
                    Artists.Add(x); 
                }
            );
            */

            Artists = new List<Artist>();
            Artists.AddRange
                (
                    Albums.GroupBy
                    (
                        x => x.artist
                    )
                    .Select
                    (
                        group => new Artist(group.Key) { albums = group.Select(x => x).ToList() }
                    ).ToList()
                );
        }
    }

    public abstract class ChoiceMenu
    {
        public abstract ChoiceMenu Menu();
    }

    public static class UserInterface
    {
        public static void ContentDisplay<T>(ICollection<T> contents)
        {
            int i = 1;
            foreach (var content in contents)
            {
                Console.WriteLine($"{i} - {content.ToString()}");
                i++;
            }
        }

        //public static int ContentSelectionMenu(ICollection<string> contents)
        //{
        //    int input = -1;
        //    do
        //    {
        //        Console.Clear();
        //        ContentDisplay(contents);
        //        SongPlayerDisplay();
        //        Console.WriteLine("Seleziona indice corrispondente");
        //        try
        //        {
        //            input = int.Parse(Console.ReadLine());
        //            input--;
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine(ex.ToString());
        //        }
        //    }
        //    while (input > contents.Count - 1 || input < 0);
        //    return input;
        //}

        public static T ContentSelectionMenu<T>(ICollection<T> contents)
        {
            int input = -1;
            do
            {
                Console.Clear();
                ContentDisplay(contents);
                SongPlayerDisplay();
                Console.WriteLine("Seleziona indice corrispondente");
                try
                {
                    input = int.Parse(Console.ReadLine());
                    input--;
                }
                catch
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine("Valore inserito non è numerico");
                    Console.WriteLine("Premi qualsiasi tasto per continuare");
                    Console.ResetColor();
                    Console.ReadKey();
                }
            }
            while (input > contents.Count - 1 || input < 0);
            return contents.ElementAt(input);
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
                Console.WriteLine("E - per Visualizzare il Player");

                input = Console.ReadKey().Key;
            }
            while ( input != ConsoleKey.A && 
                    input != ConsoleKey.B && 
                    input != ConsoleKey.C &&
                    input != ConsoleKey.D &&
                    input != ConsoleKey.E &&
                    input != ConsoleKey.D);
            UserInterfaceCommand(input);
        }

        public static void SongPlayerDisplay()
        {
            try
            {
                string text = $"     { (SongPlayer.Instance.currentlyPlaying ? "Currently" : "Paused") } Playing : {SongPlayer.Instance.GetCurrentSong().FullInfo()}     ";
                string spacing = "--------------------------------------------------------------------------------------------------------------------------------";
                spacing = spacing.Remove(text.Length);
                Console.WriteLine($"{spacing}\n{text}\n{spacing}");
            }
            catch
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
            try
            {
                contents.AddRange(SongPlayer.Instance.songs.Select(x => new string(x.title)).ToList());
                do
                {
                    Console.Clear();
                    ContentDisplay(contents);
                    SongPlayerDisplay();
                    Console.WriteLine("N - per Selezionare la Canzone Successiva");
                    Console.WriteLine("P - per Selezionare la Canzone Precedente");
                    Console.WriteLine($"O - Per Mettere in {(SongPlayer.Instance.currentlyPlaying ? "Pausa" : "Play")} la Canzone");
                    Console.WriteLine("BACK - Per Tornare al Menu Iniziale");
                    input = Console.ReadKey().Key;
                }
                while ( input != ConsoleKey.N && 
                        input != ConsoleKey.P && 
                        input != ConsoleKey.O &&
                        input != ConsoleKey.Backspace);
                UserInterfaceCommand(input);
            }
            catch
            {
                Console.Clear();
                SongPlayerDisplay();
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("Nessuna Canzone nel Player");
                Console.WriteLine("Premi qualsiasi tasto per continuare");
                Console.ResetColor();
                Console.ReadKey();
                CategorySelectionMenu();
            }
        }

        public static void ContentSelectionArtist(ICollection<Artist> artists)
        {
            ContentSelectionAlbum(ContentSelectionMenu(artists).albums);
        }

        public static void ContentSelectionAlbum(ICollection<Album> albums)
        {
            ContentSelectionMusic(ContentSelectionMenu(albums).songs);
        }

        public static void ContentSelectionMusic(ICollection<Song> songs)
        {
            Song selectedSong = ContentSelectionMenu(songs);
            SongPlayer.Instance.LoadMusic(songs);
            SongPlayer.Instance.PlaySpecific(selectedSong);
            SongPlayerMenu();
        }       

        public static void UserInterfaceCommand(ConsoleKey input)
        {
            switch (input)
            {

                // 
                case ConsoleKey.F1:
                    {

                    }
                    break;
                
                // Escape program
                case ConsoleKey.Escape:
                    {

                    }
                    break;
                
                // Back to defualt menu
                case ConsoleKey.Backspace:
                    {
                        CategorySelectionMenu();
                    }
                    break;

                // All Artist Selection Menu
                case ConsoleKey.A:
                    {
                        ContentSelectionArtist(DataBank.Instance.Artists);
                    }
                    break;

                // All Album Selection Menu
                case ConsoleKey.B:
                    { 
                        ContentSelectionAlbum(DataBank.Instance.Albums);
                    }
                    break;
                
                // All Playlist Selection Menu
                case ConsoleKey.C:
                    {

                    }
                    break;

                // All Music Selection Menu
                case ConsoleKey.D:
                    {
                        Song selectedSong = ContentSelectionMenu(DataBank.Instance.Songs);
                        SongPlayer.Instance.LoadMusic(DataBank.Instance.Albums.Where(x => x.title.Equals(selectedSong.album)).FirstOrDefault().songs);
                        SongPlayer.Instance.PlaySpecific(selectedSong);
                        SongPlayerMenu();
                    }
                    break;

                // Song Player Menu
                case ConsoleKey.E:
                    {
                        SongPlayerMenu();
                    }
                    break;

                // Next
                case ConsoleKey.N:
                    {
                        SongPlayer.Instance.Next();
                        SongPlayerMenu();
                    }
                    break;
                
                // Previous
                case ConsoleKey.P:
                    {
                        SongPlayer.Instance.Previous();
                        SongPlayerMenu();
                    }
                    break;
                
                // Pause and Play
                case ConsoleKey.O:
                    {
                        SongPlayer.Instance.PlayAndStop();
                        SongPlayerMenu();
                    }
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
            index = -1;
            time = 0;
            this.songs = new List<Song>() { };
            this.songs.AddRange(songs);
        }

        public bool PlayAndStop()
        {
            currentlyPlaying = currentlyPlaying ? false : true;
            return currentlyPlaying;
        }

        public int PlaySpecific(Song song)
        {
            currentlyPlaying = true;
            index = songs.IndexOf(song);
            return index;
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

        public override string ToString()
        {
            return title;
        }

        public string FullInfo()
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
            this.songs = new List<Song>();
        }

        public override string ToString()
        {
            return title;
        }

    }

    public class Artist
    {
        public string name { get; set; }

        public List<Album> albums { get; set; }

        public Artist(string name)
        {
            this.name = name;
            this.albums = new List<Album>();
        }

        public override string ToString()
        {
            return name;
        }

    }
}
