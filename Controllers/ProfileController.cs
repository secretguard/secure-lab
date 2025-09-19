using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net; // for HtmlEncode
using System.Text;

[Route("Profile")]
public class ProfileController : Controller
{
    private readonly UserRepository _repo;
    public ProfileController(UserRepository repo) { _repo = repo; }

    [HttpGet("Notes/{userId}")]
    public IActionResult Notes(int userId)
    {
        if (userId <= 0) return BadRequest("Invalid user id.");
        var notes = _repo.GetNotes(userId); // still requires auth in future
        return Ok(notes);
    }



    [HttpPost("Notes")]
    public IActionResult AddNote([FromForm] AddNoteModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var trimmed = model.Content.Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            return BadRequest("Content cannot be empty.");
        }

        // Remove nulls and other control chars except newline & tab
        var sb = new StringBuilder(trimmed.Length);
        foreach (var ch in trimmed)
        {
            if (ch == '\n' || ch == '\t' || !char.IsControl(ch))
                sb.Append(ch);
        }
        var cleaned = sb.ToString();

        // Basic length re-check after cleaning
        if (cleaned.Length > 500)
        {
            cleaned = cleaned[..500];
        }

        // HTML encode to neutralize any markup / script
        var sanitized = WebUtility.HtmlEncode(cleaned);

        _repo.AddNote(model.UserId, sanitized);
        return Ok("Note saved");
    }
}


public class AddNoteModel
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "UserId must be positive.")]
    public int UserId { get; set; }

    [Required]
    [StringLength(500, MinimumLength = 1, ErrorMessage = "Content length 1-500 chars.")]
    public string Content { get; set; } = string.Empty;
}