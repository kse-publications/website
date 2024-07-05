using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using StackExchange.Redis;

const string baseRelativePath = "../../../data/";
var jsonOptions = new JsonSerializerOptions { WriteIndented = true };

ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379");
IDatabase db = redis.GetDatabase();
SearchCommands ft = db.FT();
JsonCommands json = db.JSON();

int totalPublications = int.Parse((await db.StringGetAsync("TotalPublicationsCount"))!);
var searchResult = await ft.SearchAsync("publication-idx", 
    new Query("*")
        .ReturnFields("Id", "Language")
        .Limit(0, totalPublications));

(int, string)[] ids = searchResult.Documents
    .Select(d => ((int)d["Id"], (string)d["Language"]!))
    .ToArray();

// var pairsDistances = await GetDistances(ids);
// float[] scoreDistances = pairsDistances.Select(kvp => kvp.Value).ToArray();;
// Console.WriteLine($"Total distances: {scoreDistances.Length}");
// Console.WriteLine($"Average distance: {pairsDistances.Average(p => p.Distance)}");
// await File.WriteAllBytesAsync("scores-all-pairs-triple-title.json",
//     JsonSerializer.SerializeToUtf8Bytes(pairsDistances, new JsonSerializerOptions(){ WriteIndented = true}));

PublicationNeighboursStats[] stats = await GetNeighboursStats(ids, topThreshold: 0.6m);
Console.WriteLine($"Total stats: {stats.Length}");
await File.WriteAllBytesAsync(baseRelativePath + "scores-average-per-threshold-lt-0.6_new.json",
    JsonSerializer.SerializeToUtf8Bytes(stats, jsonOptions));
    
return;

async Task<PublicationNeighboursStats[]> GetNeighboursStats(
    (int, string)[] publicationIds, decimal topThreshold = 1, decimal thresholdStep = 0.04m)
{
    PublicationNeighboursStats[] neighbourStats = publicationIds
        .Select(p => new PublicationNeighboursStats { Id = p.Item1, Language = p.Item2})
        .ToArray();
    
    decimal[] thresholds = Enumerable.Range(1, (int)(topThreshold / thresholdStep))
        .Select(i => i * thresholdStep)
        .ToArray();

    foreach (var stat in neighbourStats)
    {
        byte[]? byteArray = await GetPublicationEmbedding(stat.Id);
        if (byteArray is null)
            continue;
        
        string query = GetFilteredByIdsQuery(publicationIds.Select(p => p.Item1).ToArray(), stat.Id);

        foreach (var threshold in thresholds)
        {
            Query vectorRangeQuery = new Query(
                $"({query}) @SimilarityVector:[VECTOR_RANGE {threshold} $vec_param]=>{{$yield_distance_as: dist}}");
            
            SearchResult result = await ft.SearchAsync("publication-idx", 
                vectorRangeQuery
                    .AddParam("vec_param", byteArray)
                    .SetSortBy("dist")
                    .ReturnFields("Id", "Title", "AbstractText")
                    .Limit(0, publicationIds.Length - 1)
                    .Dialect(3));
            
            stat.NeighboursPerDistance.Add(threshold, (int)result.TotalResults);
            
            // if (threshold is > 0.2f and < 0.25f)
            // {
            //     stat.Neighbours.AddRange(searchResult.Documents.Select(d => new RedisPublication
            //     {
            //         Id = (int)d["Id"],
            //         Title = Regex.Unescape((string)d["Title"]!),
            //         AbstractText = Regex.Unescape((string)d["AbstractText"]!)
            //     }));
            // }
        }
    }
    
    return neighbourStats;
}

async Task<IList<Pair>> GetDistances(int[] publicationIds)
{
    Dictionary<(int, int), float> dict = new();
    
    foreach (var id in publicationIds)
    {
        byte[]? byteArray = await GetPublicationEmbedding(id);
        if (byteArray is null)
            continue;
        
        string query = GetFilteredByIdsQuery(publicationIds, id);
        SearchResult result = await ft.SearchAsync("publication-idx",
            new Query($"({query})=>[KNN $K @SimilarityVector $BLOB as similarity_score]")
                .AddParam("K", publicationIds.Length - 1)
                .AddParam("BLOB", byteArray)
                .ReturnFields("Id", "similarity_score")
                .Limit(0, publicationIds.Length - 1)
                .Dialect(3));

        var scores = result.Documents.Select(d => new Score
        {
            Id = (int)d["Id"],
            SimilarityScore = (float)d["similarity_score"]
        });
    
        foreach (var score in scores)
        {
            if (dict.ContainsKey((id, score.Id)) || dict.ContainsKey((score.Id, id)))
                continue;
            
            dict.Add((id, score.Id), score.SimilarityScore);
        }
        
    }

    return dict.Select(kvp => new Pair(kvp.Key.Item1, kvp.Key.Item2)
    {
        Distance = kvp.Value
    }).ToList();
}

async Task<byte[]?> GetPublicationEmbedding(int id)
{
    float[]? currentEmbedding = await json.GetAsync<float[]>(
        key: $"publication:{id}",
        path: "$.SimilarityVector.Vector");
    
    if (currentEmbedding is null)
        return null;
    
    List<byte> bytes = [];
    foreach (float f in currentEmbedding)
    {
        bytes.AddRange(BitConverter.GetBytes(f));
    }

    return bytes.ToArray();
}

string GetFilteredByIdsQuery(int[] idsToGet, int currentId)
{
    StringBuilder sb = new();
    foreach (var id in idsToGet)
    {
        if (id == currentId)
            continue;
        
        if (sb.Length > 0)
            sb.Append(" | ");
        sb.Append($"(@Id:[{id},{id}])");
    }

    return sb.ToString();
}    

public class Score
{
    public int Id { get; set; }
    
    [JsonPropertyName("similarity_score")]
    public float SimilarityScore { get; set; }
}

public record Pair(int Id1, int Id2)
{
    public float Distance { get; set; }
}

public class PublicationNeighboursStats
{
    public int Id { get; set; }
    public string Language { get; set; } = string.Empty;
    public Dictionary<decimal, int> NeighboursPerDistance { get; set; } = [];
    public List<RedisPublication> Neighbours { get; set; } = [];
}

public class RedisPublication
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string AbstractText { get; set; } = string.Empty;
}