using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;

[Route("Files")]
public class FilesController : Controller
{
    private readonly IWebHostEnvironment _env;
    public FilesController(IWebHostEnvironment env) { _env = env; }

    [HttpPost("Upload")]
    public async Task<IActionResult> Upload()
    {
        var file = Request.Form.Files.Count > 0 ? Request.Form.Files[0] : null;
        if (file == null) return BadRequest("No file");

        var uploads = Path.Combine(_env.WebRootPath, "uploads");
        Directory.CreateDirectory(uploads);
        var filePath = Path.Combine(uploads, file.FileName); // INSECURE: original filename
        using var fs = System.IO.File.Create(filePath);
        await file.CopyToAsync(fs);

        return Ok(new { path = $"/uploads/{file.FileName}" });
    }
}
