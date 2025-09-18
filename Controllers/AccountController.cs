using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

[Route("Account")]
public class AccountController : Controller
{
    private readonly UserRepository _repo;
    public AccountController(UserRepository repo) { _repo = repo; }

    [HttpPost("Register")]
    public IActionResult RegisterPost([FromForm] string username, [FromForm] string password)
    {
        // Validate username and password
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return BadRequest("Username and password required.");
        if (username.Length < 4 || username.Length > 32 || !Regex.IsMatch(username, "^[a-zA-Z0-9_]+$"))
            return BadRequest("Invalid username format.");
        if (password.Length < 8)
            return BadRequest("Password must be at least 8 characters.");

        // Hash password before storing
        var hashedPassword = HashPassword(password);
        _repo.AddUser(username, hashedPassword); // Secure: store hashed password
        return Ok("Registered");
    }

    [HttpPost("Login")]
    public IActionResult LoginPost([FromForm] string username, [FromForm] string password)
    {
        var user = _repo.GetByUserName(username);
        // Use generic error message to prevent user enumeration
        if (user == null)
            return Unauthorized("Invalid username or password.");

        // Use constant-time comparison for password hashes
        if (!VerifyPassword(password, user.Password))
            return Unauthorized("Invalid username or password.");

        // Secure: Create claims and sign in user with cookie authentication
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return Ok(new { message = "Logged in" }); // Session cookie is now set
    }

    // Hash password using SHA256 (for demo; use bcrypt/PBKDF2 in production)
    private string HashPassword(string password)
    {
        using (var sha = SHA256.Create())
        {
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }

    // Constant-time comparison of password hashes
    private bool VerifyPassword(string password, string storedHash)
    {
        var hash = HashPassword(password);
        return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(hash), Encoding.UTF8.GetBytes(storedHash));
    }
}
