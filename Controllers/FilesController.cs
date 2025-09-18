using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Text.RegularExpressions;

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

        // Restrict file size (e.g., max 5MB)
        const long maxFileSize = 5 * 1024 * 1024;
        if (file.Length > maxFileSize)
            return BadRequest("File too large");

        // Restrict file types (allow only .jpg, .png, .txt for demo)
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".txt", ".pdf", ".docx", ".xlsx", ".pptx" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (Array.IndexOf(allowedExtensions, ext) < 0)
            return BadRequest("Invalid file type");

        // Sanitize filename to prevent path traversal
        var safeFileName = Regex.Replace(Path.GetFileName(file.FileName), "[^a-zA-Z0-9_.-]", "_");
        if (string.IsNullOrWhiteSpace(safeFileName))
            return BadRequest("Invalid filename");

        var uploads = Path.Combine(_env.WebRootPath, "uploads");
        Directory.CreateDirectory(uploads);
        var filePath = Path.Combine(uploads, safeFileName); // Secure: sanitized filename

        // Prevent overwriting existing files
        if (System.IO.File.Exists(filePath))
            return Conflict("File already exists");

        // TODO: Scan file for malware/viruses before saving (not implemented)

        using var fs = System.IO.File.Create(filePath);
        await file.CopyToAsync(fs);

       // return Ok(new { path = $"/uploads/{safeFileName}" });
        // ...existing code...
        return Ok(new { message = "File uploaded successfully" });
    }
}
