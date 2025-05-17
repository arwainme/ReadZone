using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadZone.Data;
using ReadZone.DTO;
using ReadZone.Models;

namespace ReadZone.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserLibraryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserLibraryController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Add to Favorites
        [HttpPost("favorite")]
        public async Task<IActionResult> AddToFavorites([FromBody] UserLibraryDto dto)
        {
            if (await _context.UserFavoriteBooks.AnyAsync(x => x.UserId == dto.UserId && x.BookId == dto.BookId))
                return BadRequest("Book already in favorites");

            _context.UserFavoriteBooks.Add(new UserFavoriteBook { UserId = dto.UserId, BookId = dto.BookId });
            await _context.SaveChangesAsync();
            return Ok("Added to favorites");
        }

        // ✅ Add to Bookmarks
        [HttpPost("bookmark")]
        public async Task<IActionResult> AddToBookmarks([FromBody] UserLibraryDto dto)
        {
            if (await _context.UserBookmarkBooks.AnyAsync(x => x.UserId == dto.UserId && x.BookId == dto.BookId))
                return BadRequest("Book already bookmarked");

            _context.UserBookmarkBooks.Add(new UserBookmarkBook { UserId = dto.UserId, BookId = dto.BookId });
            await _context.SaveChangesAsync();
            return Ok("Bookmarked");
        }

        // ✅ Add to Downloads
        [HttpPost("download")]
        public async Task<IActionResult> AddToDownloads([FromBody] UserLibraryDto dto)
        {
            if (await _context.UserDownloadedBooks.AnyAsync(x => x.UserId == dto.UserId && x.BookId == dto.BookId))
                return BadRequest("Book already downloaded");

            _context.UserDownloadedBooks.Add(new UserDownloadedBook { UserId = dto.UserId, BookId = dto.BookId });
            await _context.SaveChangesAsync();
            return Ok("Downloaded");
        }

        // ✅ Get All User Library (Grouped)
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserLibrary(int userId)
        {
            var favorites = await _context.UserFavoriteBooks
                .Where(x => x.UserId == userId)
                .Select(x => x.BookId)
                .ToListAsync();

            var bookmarks = await _context.UserBookmarkBooks
                .Where(x => x.UserId == userId)
                .Select(x => x.BookId)
                .ToListAsync();

            var downloads = await _context.UserDownloadedBooks
                .Where(x => x.UserId == userId)
                .Select(x => x.BookId)
                .ToListAsync();

            return Ok(new
            {
                Favorites = favorites,
                Bookmarks = bookmarks,
                Downloads = downloads
            });
        }

        // ✅ Delete from Favorites
        [HttpDelete("favorite")]
        public async Task<IActionResult> RemoveFromFavorites([FromBody] UserLibraryDto dto)
        {
            var record = await _context.UserFavoriteBooks.FirstOrDefaultAsync(x => x.UserId == dto.UserId && x.BookId == dto.BookId);
            if (record == null) return NotFound("Not in favorites");

            _context.UserFavoriteBooks.Remove(record);
            await _context.SaveChangesAsync();
            return Ok("Removed from favorites");
        }

        // ✅ Delete from Bookmarks
        [HttpDelete("bookmark")]
        public async Task<IActionResult> RemoveFromBookmarks([FromBody] UserLibraryDto dto)
        {
            var record = await _context.UserBookmarkBooks.FirstOrDefaultAsync(x => x.UserId == dto.UserId && x.BookId == dto.BookId);
            if (record == null) return NotFound("Not bookmarked");

            _context.UserBookmarkBooks.Remove(record);
            await _context.SaveChangesAsync();
            return Ok("Removed from bookmarks");
        }

        // ✅ Delete from Downloads
        [HttpDelete("download")]
        public async Task<IActionResult> RemoveFromDownloads([FromBody] UserLibraryDto dto)
        {
            var record = await _context.UserDownloadedBooks.FirstOrDefaultAsync(x => x.UserId == dto.UserId && x.BookId == dto.BookId);
            if (record == null) return NotFound("Not downloaded");

            _context.UserDownloadedBooks.Remove(record);
            await _context.SaveChangesAsync();
            return Ok("Removed from downloads");
        }
    }
}

