using System.ComponentModel.DataAnnotations;

namespace Publications.Application.DTOs.Request;

public record AuthorFilterDTO
{
    [RegularExpression(@"^\d+(?:-\d+)*$", ErrorMessage = "Invalid authors format.")]
    public string Authors { get; init; } = string.Empty;

    private int[]? _authorIds;

    public int[] GetParsedAuthorsId()
    {
        return _authorIds ??= ParseAuthors(Authors);
    }

    private static int[] ParseAuthors(string authors)
    {
        return authors.Split('-', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
    }
}
