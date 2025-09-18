using Microsoft.AspNetCore.Mvc;

[Route("Account")]
public class AccountController : Controller
{
    private readonly UserRepository _repo;
    public AccountController(UserRepository repo) { _repo = repo; }

    [HttpPost("Register")]
    public IActionResult RegisterPost([FromForm] string username, [FromForm] string password)
    {
        _repo.AddUser(username, password); // INSECURE: no validation, plaintext password
        return Ok("Registered");
    }

    [HttpPost("Login")]
    public IActionResult LoginPost([FromForm] string username, [FromForm] string password)
    {
        var user = _repo.GetByUserName(username);
        if (user == null) return Unauthorized("No such user");
        if (user.Password != password) return Unauthorized("Bad password");
        return Ok(new { message = "Logged in", userId = user.Id }); // INSECURE: no session
    }
}
