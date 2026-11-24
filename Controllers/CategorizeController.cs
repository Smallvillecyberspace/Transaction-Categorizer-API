using Microsoft.AspNetCore.Mvc;

namespace Transaction_Categorizer_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategorizeController : ControllerBase
{
    // Order matters: the first category with a matching keyword wins.
    private static readonly (string Category, string[] Keywords)[] Rules =
    {
        ("Transport",     new[] { "uber", "bolt", "taxi", "fuel" }),
        ("Food",          new[] { "shoprite", "market", "food", "grocery" }),
        ("Entertainment", new[] { "netflix", "spotify", "dstv", "movie" }),
        ("Education",     new[] { "school", "tuition", "course" })
    };

    /// <summary>
    /// Categorizes a transaction based on keywords in its description.
    /// </summary>
    /// <param name="request">The transaction description and amount.</param>
    /// <returns>One of: Transport, Food, Entertainment, Education, Other.</returns>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public ActionResult<string> Categorize([FromBody] TransactionRequest request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Description))
        {
            return BadRequest("Description is required");
        }

        foreach (var (category, keywords) in Rules)
        {
            if (keywords.Any(k => request.Description.Contains(k, StringComparison.OrdinalIgnoreCase)))
            {
                return Ok(category);
            }
        }

        return Ok("Other");
    }
}

public class TransactionRequest
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}