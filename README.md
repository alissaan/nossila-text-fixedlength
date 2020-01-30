
# Nossila Text Fixed Length
A small, short and direct .NET library to serialize and parse fixed length content.
**Nuget package:** https://www.nuget.org/packages/Nossila.Text.FixedLength/

## Serializing

````c#
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
````

## Parsing (deserializing)
````c#
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

````    

