using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System.ComponentModel.DataAnnotations;

[Route("Account")]
public class AccountController : Controller
{
    private readonly UserRepository _repo;
    private readonly IPasswordHasher<User> _hasher;
    public AccountController(UserRepository repo, IPasswordHasher<User> hasher) { _repo = repo; _hasher = hasher; }

    [HttpPost("Register")]
    public IActionResult RegisterPost([FromForm] AccountModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        _repo.AddUser(model.username.Trim(), model.password); // password hashed in repository
        return Ok("Registered");
    }

    [HttpPost("Login")]
    public IActionResult LoginPost([FromForm] AccountModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = _repo.GetByUserName(model.username.Trim());
        if (user == null) return Unauthorized("No such user");
        try
        {
            var result = _hasher.VerifyHashedPassword(user, user.Password, model.password);
            if (result == PasswordVerificationResult.Failed) return Unauthorized("Bad password");
            return Ok(new { message = "Logged in", userId = user.Id }); // still no session/token
        }
        catch (Exception)
        {
            return Unauthorized("invalid username or password");
        }

    }
}

public class AccountModel
{
    [Required]
    [StringLength(500, MinimumLength = 1, ErrorMessage = "Content length 1-500 chars.")]
    public string username { get; set; }

    [Required]
    [StringLength(500, MinimumLength = 1, ErrorMessage = "Content length 1-500 chars.")]
    public string password { get; set; } = string.Empty;
}
