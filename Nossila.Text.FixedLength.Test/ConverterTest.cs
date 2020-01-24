using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nossila.Text.FixedLength.Test
{
    [TestClass]
    public class ConverterTest
    {
        private List<Movie> GetMovies()
        {
            var movies = new List<Movie>();

            movies.Add(new Movie
            {
                Id = 1,
                Title = "Terminator 2: Judgement Day",
                Director = "James Cameron",
                ReleaseYear = 1991,
                Budget = 102000000,
            });

            movies.Add(new Movie
            {
                Id = 2,
                Title = "The Shawshank Redemption",
                Director = "Frank Darabont",
                ReleaseYear = 1994,
                Budget = 25000000,
            });

            movies.Add(new Movie
            {
                Id = 3,
                Title = "The Sixth Sense",
                Director = "M. Night Shyamalan",
                ReleaseYear = 1999,
                Budget = 40000000,
            });

            return movies;
        }

        private string GetSerializedMovies()
        {
            var payload = FixedLengthConverter.Serialize(srlzr =>
            {
                var models = GetMovies();

                foreach (var model in models)
                {
                    var i = models.IndexOf(model);
                    const int expectedLengthPerLine = 102;

                    srlzr.Concat(model.Id, 10);
                    srlzr.Concat(model.Title, 40);
                    srlzr.Concat(model.Director, 30);
                    srlzr.Concat(model.ReleaseYear, 4);
                    srlzr.Concat(model.Budget, 18);

                    srlzr.ConcatLineBreak();

                    srlzr.Validate((i + 1) * expectedLengthPerLine, ignoreLineBreaks: true);
                }
            });

            return payload;
        }

        [TestMethod]
        public void TestSerializer()
        {
            GetSerializedMovies();
        }

        [TestMethod]
        public void TestDeserializer()
        {
            var payload = GetSerializedMovies();
            var models = GetMovies();

            FixedLengthConverter.Parse(payload, psr =>
            {
                if (!psr.HasData)
                    return;

                psr.AutoTrim = true;
                psr.RemoveLineBreaks();

                while (psr.HasData)
                {
                    var movie = new Movie
                    {
                        Id = int.Parse(psr.Cut(10)),
                        Title = psr.Cut(40),
                        Director = psr.Cut(30),
                        ReleaseYear = int.Parse(psr.Cut(4)),
                        Budget = decimal.Parse(psr.Cut(18))
                    };

                    Assert.AreEqual(movie, models.First(a => a.Id == movie.Id));
                }
            });
        }
    }
}
