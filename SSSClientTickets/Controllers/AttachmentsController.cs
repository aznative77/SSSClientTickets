using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SSSClientTickets.Models;

namespace SSSClientTickets.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentsController : ControllerBase
    {
        private readonly SssclientContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const long MaxFileSize = 50 * 1024 * 1024; // 50 MB
        private static readonly string[] ImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".csv" };

        public AttachmentsController(SssclientContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadAttachment([FromForm] int ticketRec, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Check if ticket exists
            var ticket = await _context.Tickets.FindAsync(ticketRec);
            if (ticket == null)
                return NotFound("Ticket not found.");

            // Validate file size
            if (file.Length > MaxFileSize)
                return BadRequest($"File size exceeds {MaxFileSize / (1024 * 1024)} MB limit.");

            // Validate file extension
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!AllowedExtensions.Contains(fileExtension))
                return BadRequest("File type not allowed. Allowed types: " + string.Join(", ", AllowedExtensions));

            try
            {
                // Create folder structure: wwwroot/uploads/tickets/{ticketId}
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "tickets", ticketRec.ToString());
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Generate unique filename to prevent overwrites
                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var uniqueFileName = $"{fileName}_{DateTime.Now:yyyyMMdd_HHmmss}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Create database record
                var attachment = new TicketAttachment
                {
                    TicketRec = ticketRec,
                    FileName = file.FileName,
                    FileExtension = fileExtension,
                    FileSizeBytes = file.Length,
                    UploadedDate = DateTime.Now,
                    IsImage = ImageExtensions.Contains(fileExtension)
                };

                _context.TicketAttachments.Add(attachment);
                await _context.SaveChangesAsync();

                return Ok(new { 
                    attachmentRec = attachment.AttachmentRec,
                    fileName = attachment.FileName,
                    isImage = attachment.IsImage,
                    uploadedDate = attachment.UploadedDate
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading file: {ex.Message}");
            }
        }

        [HttpDelete("delete/{attachmentRec}")]
        public async Task<IActionResult> DeleteAttachment(int attachmentRec)
        {
            var attachment = await _context.TicketAttachments.FindAsync(attachmentRec);
            if (attachment == null)
                return NotFound();

            try
            {
                // Delete physical file
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "tickets", attachment.TicketRec.ToString());
                var filePath = Path.Combine(uploadsFolder, $"*{attachment.FileExtension}");
                
                // Find and delete the file (we're searching by extension since we store original filename)
                var files = Directory.GetFiles(uploadsFolder, Path.GetFileNameWithoutExtension(attachment.FileName) + "_*" + attachment.FileExtension);
                foreach (var f in files)
                {
                    System.IO.File.Delete(f);
                }

                // Delete database record
                _context.TicketAttachments.Remove(attachment);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting file: {ex.Message}");
            }
        }

        [HttpGet("ticket/{ticketRec}")]
        public async Task<IActionResult> GetTicketAttachments(int ticketRec)
        {
            var attachments = await _context.TicketAttachments
                .Where(a => a.TicketRec == ticketRec)
                .OrderByDescending(a => a.UploadedDate)
                .ToListAsync();

            return Ok(attachments.Select(a => new
            {
                a.AttachmentRec,
                a.FileName,
                a.FileExtension,
                a.IsImage,
                a.UploadedDate,
                imagePath = a.IsImage ? $"/uploads/tickets/{ticketRec}/*{a.FileExtension}" : null
            }));
        }

        [HttpGet("file/{ticketRec}/{attachmentRec}")]
        public async Task<IActionResult> GetAttachmentFile(int ticketRec, int attachmentRec)
        {
            var attachment = await _context.TicketAttachments
                .FirstOrDefaultAsync(a => a.AttachmentRec == attachmentRec && a.TicketRec == ticketRec);

            if (attachment == null)
                return NotFound();

            try
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "tickets", ticketRec.ToString());
                var files = Directory.GetFiles(uploadsFolder, Path.GetFileNameWithoutExtension(attachment.FileName) + "_*" + attachment.FileExtension);

                if (files.Length == 0)
                    return NotFound("File not found on disk.");

                var filePath = files[0];
                var fileStream = System.IO.File.OpenRead(filePath);
                var contentType = GetContentType(attachment.FileExtension);
                var ext = attachment.FileExtension.ToLower();

                // For viewable documents and images, use inline disposition; for others use attachment
                bool isViewable = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp", ".txt", ".csv" }.Contains(ext);
                
                if (isViewable)
                {
                    // Return inline for viewing in browser
                    Response.Headers["Content-Disposition"] = $"inline; filename=\"{attachment.FileName}\"";
                    return File(fileStream, contentType);
                }
                else
                {
                    // Return attachment for download
                    return File(fileStream, contentType, attachment.FileName);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving file: {ex.Message}");
            }
        }

        private string GetContentType(string extension)
        {
            return extension.ToLower() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".bmp" => "image/bmp",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".txt" => "text/plain",
                ".csv" => "text/csv",
                _ => "application/octet-stream"
            };
        }
    }
}
