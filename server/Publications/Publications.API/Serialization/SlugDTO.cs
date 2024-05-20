using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Publications.API.Serialization;

public record SlugDTO
{
    [RegularExpression("^(?:[a-zA-Z0-9]+-)*\\d+$", ErrorMessage = "Invalid id format.")]
    [BindProperty(Name = "id", SupportsGet = true)]
    public string Slug { get; init; } = string.Empty;

    private int? _id; 
    public int GetId()
    {
        return _id ??= ParseId(Slug);
    }
    
    private static int ParseId(string slug)
    {
        return int.Parse(slug
            .Split('-', StringSplitOptions.RemoveEmptyEntries)
            .Last());
    }
}