using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace TheComputer
{
    class Program
    {
        static void Main(string[] args)
        {
            const int randMovieCount = 10000;
            /*Lists*/
            string[] rawRandomNumberlist;
            List<string> randomAtomsphereNumbers = new List<string>();
            List<Movie> movies = new List<Movie>();

            /*Load movies and assign them a number (1-X)*/
            int movieNumber = 1;
            Console.WriteLine("Loading movie list...");
            using (StreamReader sr = new StreamReader("c:\\data\\movies.txt"))
            {
                while (sr.Peek() >= 0)
                {
                    movies.Add(new Movie(sr.ReadLine(), movieNumber));
                    movieNumber++;
                }
            }
            Console.WriteLine("Finished loading movie list...");
            Console.WriteLine(movies.Count + " movies loaded...");

            /*Connect to random.org and pull down 10000 random numbers between 1 and the total number of movies loaded)*/
            Console.WriteLine("Connecting to the atmospheric noise randomizer and pulling " + randMovieCount + " random numbers..");
            WebRequest request = WebRequest.Create("https://www.random.org/integers/?num=" + randMovieCount + "&min=1&max=" + (movies.Count) + "&col=1&base=10&format=plain&rnd=new");
            WebResponse response = request.GetResponse();
            Console.WriteLine("Server Response: " + ((HttpWebResponse)response).StatusDescription);
            using (Stream dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();

                /*Clean the data stream into an array*/
                rawRandomNumberlist = responseFromServer.Split("\n");
            }
            response.Close();
            Console.WriteLine("Loaded " + rawRandomNumberlist.Length + " random numbers");

            /* Go through each element in the random number array
             * Try to parse the data into an integer
             * Find the movie that has the same movie number
             * Increase its movie counter
             */
            foreach (string rawRandomNumber in rawRandomNumberlist)
            {
                int convertedRandomAtomsphereNumber;
                bool success = Int32.TryParse(rawRandomNumber, out convertedRandomAtomsphereNumber);
                if (success)
                {
                    var movie = movies.Find(x => x.movieNumber == convertedRandomAtomsphereNumber);
                    movie.IncreaseCount();
                }
            }

            /*Sort the list and order it from lowest to highest based on the movie counter*/
            movies.Sort((x, y) => x.GetMovieCount().CompareTo(y.GetMovieCount()));

            /*List all movies and their counter to console for review*/
            foreach (Movie movie in movies)
            {
                Console.WriteLine("Movie " + movie.GetMovieName() + " had a total of " + movie.GetMovieCount() + " random hits.");
            }
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }
    }
    public class Movie
    {
        public int movieNumber { get; set; }
        private int movieCount;
        private string movieName;
        public Movie(string movieName, int movieNumber)
        {
            this.movieNumber = movieNumber;
            this.movieName = movieName;
            this.movieCount = 0;
        }
        public void IncreaseCount()
        {
            this.movieCount++;
        }
        public string GetMovieName()
        {
            return this.movieName;
        }
        public int GetMovieCount()
        {
            return this.movieCount;
        }
    }
}
