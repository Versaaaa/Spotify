using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Exercise0007
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
        }

    }


    public class SongCollectionPlayer
    {

    }

    public class AlbumPlayer
    {

    }

    public class PlaylistPlayer
    {

    }

    public abstract class MusicEntry
    {
        string name;
        List<string> genre;

        public MusicEntry(string name, ICollection<string> genre)
        {
            this.name = name;
            this.genre = new List<string>();
            this.genre.AddRange(genre);
        }        
        public MusicEntry(string name, string genre)
        {
            this.name = name;
            this.genre = new List<string>() { genre };
        }
    }

    public class Artist : MusicEntry
    {
        public Artist(string name, ICollection<string> genre) : base(name, genre)
        {

        }

        public Artist(string name, string genre) : base(name, genre)
        {

        }
    }

    public class Song : MusicEntry
    {
        float duration;

        public Song(string name, ICollection<string> genre, float duration) : base(name, genre)
        {
            this.duration = duration;
        }
        public Song(string name, string genre, float duration) : base(name, genre)
        {
            this.duration = duration;
        }
    }

    public abstract class SongCollection : MusicEntry
    { 
        protected List<Song> songs;

        protected SongCollection(string name, ICollection<string> genre) : base(name, genre)
        {
            songs = new List<Song>();
        }
        protected SongCollection(string name, string genre) : base(name, genre)
        {
            songs = new List<Song>();
        }

        public abstract int Add(Song song);

        public abstract int Remove(int i);

        public Song GetCurrentSong(int i)
        {
            return songs[i];
        }

        public abstract Song GetNextSong(int i, out int res);

        public abstract Song GetPreviousSong(int i, out int res);

    }

    public class Album : SongCollection
    {
        public Album(string name, ICollection<string> genre) : base(name, genre)
        {

        }

        public Album(string name, string genre) : base(name, genre)
        {

        }

        public override int Add(Song song)
        {
            songs.Add(song);
            return songs.Count;
        }

        public override Song GetNextSong(int i,out int res)
        {
            try
            {
                res = i + 1;
                Song song = songs[res];
            }
            catch 
            {
                res = 0;
            }
            return songs[res];
        }

        public override Song GetPreviousSong(int i, out int res)
        {
            try
            {
                res = i - 1;
                Song song = songs[res];
            }
            catch
            {
                res = songs.Count - 1;
            }
            return songs[res];
        }

        public override int Remove(int i)
        {
            songs.RemoveAt(i);
            return songs.Count;
        }
    }

    public class Playlist : SongCollection
    {
        public Playlist(string name, ICollection<string> genre) : base(name, genre)
        {

        }

        public Playlist(string name, string genre) : base(name, genre)
        {

        }

        public override int Add(Song song)
        {
            throw new NotImplementedException();
        }

        public override Song GetNextSong(int i, out int res)
        {
            throw new NotImplementedException();
        }

        public override Song GetPreviousSong(int i, out int res)
        {
            throw new NotImplementedException();
        }

        public override int Remove(int i)
        {
            throw new NotImplementedException();
        }
    }

}
