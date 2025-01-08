
using Eve.Application.StaticDataLoaders.ConvertFromYaml.Blueprints;
using Eve.Application.StaticDataLoaders.ConvertFromYaml.Categories;
using Eve.Application.StaticDataLoaders.ConvertFromYaml.Group;
using Eve.Application.StaticDataLoaders.ConvertFromYaml.MarketGroup;
using Eve.Application.StaticDataLoaders.ConvertFromYaml.Matterials;
using Eve.Application.StaticDataLoaders.ConvertFromYaml.Type;
using Eve.Application.StaticDataLoaders.ConvertFromYaml.Universe;
using System.Diagnostics;
using System.Text.Json;


var names = new NamesReadFromYaml(@"D:\WorkAndLearning\beckendLearning\Projects\Eve\EveApi\Eve.Application\StaticDataLoaders\sde\bsd\invNames.yaml");
//names.Initial();

//names.PrintAll();
//var stopWatch = Stopwatch.StartNew();
//var materials = new ReprocessMatterialsFileReader("D:\\WorkAndLearning\\beckendLearning\\Projects\\Eve\\EveApi\\Eve.Application\\StaticDataLoaders\\sde\\fsd\\typeMaterials.yaml");
//var types = new TypesFileReader("D:\\WorkAndLearning\\beckendLearning\\Projects\\Eve\\EveApi\\Eve.Application\\StaticDataLoaders\\sde\\fsd\\types.yaml");
//var types1 = new TypesFileReader("D:\\WorkAndLearning\\beckendLearning\\Projects\\Eve\\EveApi\\Eve.Application\\StaticDataLoaders\\sde\\fsd\\types.yaml");
//var types2 = new TypesFileReader("D:\\WorkAndLearning\\beckendLearning\\Projects\\Eve\\EveApi\\Eve.Application\\StaticDataLoaders\\sde\\fsd\\types.yaml");
//var types3 = new TypesFileReader("D:\\WorkAndLearning\\beckendLearning\\Projects\\Eve\\EveApi\\Eve.Application\\StaticDataLoaders\\sde\\fsd\\types.yaml");
//var market = new MarketGroupFlleReader("D:\\WorkAndLearning\\beckendLearning\\Projects\\Eve\\EveApi\\Eve.Application\\StaticDataLoaders\\sde\\fsd\\marketGroups.yaml");
//var groups = new GroupFileReader("D:\\WorkAndLearning\\beckendLearning\\Projects\\Eve\\EveApi\\Eve.Application\\StaticDataLoaders\\sde\\fsd\\groups.yaml");
//var category = new CategoryFileReader("D:\\WorkAndLearning\\beckendLearning\\Projects\\Eve\\EveApi\\Eve.Application\\StaticDataLoaders\\sde\\fsd\\categories.yaml");
//var blue = new BlueprintsFileReader("D:\\WorkAndLearning\\beckendLearning\\Projects\\Eve\\EveApi\\Eve.Application\\StaticDataLoaders\\sde\\fsd\\blueprints.yaml");
//var univers = new UniverseReadFromFile("D:\\WorkAndLearning\\beckendLearning\\Projects\\Eve\\EveApi\\Eve.Application\\StaticDataLoaders\\sde\\universe\\eve");

////univers.Initialize();
////materials.Initial();
////market.Initial();
////category.Initial();
////blue.Initial();
////groups.Initial();
////types.Initial();
////types1.Initial();
////types2.Initial();
////types3.Initial();

////Parallel.Invoke(
////    types.Initial,
////    types1.Initial,
////    types2.Initial,
////    types3.Initial);

//Parallel.Invoke(
//    univers.Initialize,
//    types.Initial,
//    materials.Initial,
//    market.Initial,
//    category.Initial,
//    blue.Initial);


//stopWatch.Stop();
//var milsec = stopWatch.ElapsedMilliseconds;

//Console.WriteLine($"milliseconds : {milsec}");

////Console.WriteLine($"region : {univers.Regions.Count}");
////Console.WriteLine($"constellations : {univers.Constellations.Count}");
////Console.WriteLine($"systems : {univers.Systems.Count}");
///

//List<Package> packages =
//    new List<Package>
//        { new Package { Company = "Coho Vineyard", Weight = 25.2, TrackingNumber = 1 },
//              new Package { Company = "Lucerne Publishing", Weight = 18.7, TrackingNumber = 2 },
//              new Package { Company = "Wingtip Toys", Weight = 6.0, TrackingNumber = 1 },
//              new Package { Company = "Adventure Works", Weight = 33.8, TrackingNumber = 2 } };

//// Create a Dictionary of Package objects,
//// using TrackingNumber as the key.
//var dictionary =
//    packages
//    .GroupBy(x => x.TrackingNumber)
//    .ToDictionary(p => p.Key, x => x.Select(o => o);

//foreach (var kvp in dictionary)
//{
//    foreach (var item in kvp.Value)
//    {
//        Console.WriteLine(
//            "Key {0}: {1}, {2} pounds track :{3}",
//            kvp.Key,
//            item.Company,
//            item.Weight,
//            item.TrackingNumber);
//    }
//}

Console.ReadLine();

//class Package
//{
//    public string Company { get; set; }
//    public double Weight { get; set; }
//    public long TrackingNumber { get; set; }
//}