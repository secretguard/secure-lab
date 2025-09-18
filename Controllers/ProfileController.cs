using Microsoft.AspNetCore.Mvc;

[Route("Profile")]
public class ProfileController : Controller
{
    private readonly UserRepository _repo;
    public ProfileController(UserRepository repo) { _repo = repo; }

    [HttpGet("Notes/{userId}")]
    public IActionResult Notes(int userId)
    {
        var notes = _repo.GetNotes(userId); // INSECURE: no auth check
        return Ok(notes);
    }

    [HttpPost("Notes")]
    public IActionResult AddNote([FromForm] int userId, [FromForm] string content)
    {
        _repo.AddNote(userId, content); // INSECURE: no validation
        return Ok("Note saved (plaintext)");
    }
}
