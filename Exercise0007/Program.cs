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

    public class SongPlayer
    {
        List<Song> songs;
        int index = 0;
        bool currentlyPlaying = false;
        int time;

        public bool PlayAndStop()
        {
            currentlyPlaying = currentlyPlaying ? false : true;
            return currentlyPlaying;
        }

        public SongPlayer(List<Song> songs, int index)
        {
            this.songs = songs;
            this.index = index;
        }

        public Song Current()
        {
            return songs[index];
        }

        public Song Next()
        {
            try
            {
                index++;
                return songs[index];
            }
            catch (IndexOutOfRangeException)
            {
                index = 0;
                return songs[index];
            }
        }

        public Song Previous()
        {
            try
            {
                index--;
                return songs[index];
            }
            catch (IndexOutOfRangeException)
            {
                index = songs.Count - 1;
                return songs[index];
            }
        }
    }


    public class Song
    {
        string artist;
        string album;
        string title;
        string genre;
        int duration;

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
            return $"{title} by {artist}";
        }

    }

}
