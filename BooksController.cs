using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReadZone.DTO;
using ReadZone.Models;
using ReadZone.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;

using ReadZone.Services;

namespace ReadZone.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly GutenbergService _service;

        public BooksController(GutenbergService service)
        {
            _service = service;
        }

        // ✅ GET: api/books/new
        [HttpGet("new")]
        public async Task<IActionResult> GetNewArrivals()
        {
            var books = await _service.GetNewArrivalsAsync();
            return Ok(books);
        }

        // ✅ GET: api/books/grouped
        [HttpGet("grouped")]
        public async Task<IActionResult> GetGroupedBooks()
        {
            var grouped = await _service.GetBooksGroupedByCategoryAsync();
            return Ok(grouped);
        }

        // ✅ GET: api/books/popular
        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularBooks([FromServices] AppDbContext db)
        {
            var allBooks = await _service.GetBooksAsync();

            // جلب متوسط التقييمات من قاعدة البيانات
            var bookRatings = await db.BookRatings
                .GroupBy(r => r.BookId)
                .Select(g => new
                {
                    BookId = g.Key,
                    Average = g.Average(r => r.Value)
                })
                .ToDictionaryAsync(x => x.BookId, x => x.Average);

            // اختيار أول 10 كتب وعرض التقييم الحقيقي إن وُجد
            var popularBooks = allBooks
                .Take(10)
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    AuthorName = b.Authors?.FirstOrDefault()?.Name ?? "Unknown",
                    CoverImageUrl = b.CoverImageUrl ?? "",
                    Subjects = b.Subjects ?? new List<string>(),
                    Rating = bookRatings.TryGetValue(b.Id, out var avg) ? Math.Round(avg, 1) : 4.0
                })
                .ToList();

            return Ok(popularBooks);
        }

        [HttpGet("{id}/content")]
        public async Task<IActionResult> GetBookContent(int id)
        {
            try
            {
                var allBooks = await _service.GetBooksAsync();
                var book = allBooks.FirstOrDefault(b => b.Id == id);

                if (book == null)
                    return NotFound("Book not found");

                // نحاول نجيب رابط نصي مش zip
                var textUrl = book.Formats?.FirstOrDefault(x =>
                    x.Key.Contains("text/plain") && !x.Key.Contains(".zip")).Value;

                if (string.IsNullOrEmpty(textUrl))
                    return NotFound("Book content not available");

                var handler = new HttpClientHandler
                {
                    AllowAutoRedirect = false
                };

                using var http = new HttpClient(handler);
                var request = new HttpRequestMessage(HttpMethod.Get, textUrl);
                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

                var response = await http.SendAsync(request);

                if ((int)response.StatusCode == 302 && response.Headers.Location != null)
                {
                    var redirectUrl = response.Headers.Location.ToString();

                    using var http2 = new HttpClient();
                    http2.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

                    var redirectedResponse = await http2.GetAsync(redirectUrl);

                    if (!redirectedResponse.IsSuccessStatusCode)
                        return StatusCode((int)redirectedResponse.StatusCode, $"Failed after redirect. Status: {redirectedResponse.StatusCode}");

                    var content = await redirectedResponse.Content.ReadAsStringAsync();
                    return Ok(new { Title = book.Title, Content = content });
                }

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, $"Failed to fetch book content. Status: {response.StatusCode}");
                }

                var normalContent = await response.Content.ReadAsStringAsync();
                return Ok(new { Title = book.Title, Content = normalContent });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server Error: {ex.Message}");
            }
        }








        // ✅ POST: api/books/rate


        [HttpPost("rate")]
        public async Task<IActionResult> RateBook(
            [FromServices] AppDbContext db,
            [FromBody] BookRating rating)
        {
            if (rating.Value < 1 ||  rating.Value > 5 )
            {
                return BadRequest("Rating must be between 1 and 5.");
            }

            db.BookRatings.Add(rating);
            await db.SaveChangesAsync();
            return Ok(new { message = "Rating submitted successfully." });
        }



        [HttpGet("{bookId}/ratings-distribution")]
        public async Task<IActionResult> GetRatingsDistribution(int bookId, [FromServices] AppDbContext db)
        {
            var distribution = await db.BookRatings
                .Where(r => r.BookId == bookId)
                .GroupBy(r => r.Value)
                .Select(g => new { Stars = g.Key, Count = g.Count() })
                .ToListAsync();

            return Ok(distribution);
        }

        [HttpGet("{bookId}/reviews")]
        public async Task<IActionResult> GetBookReviews(int bookId, [FromServices] AppDbContext db)
        {
            var reviews = await db.BookReviews
                .Where(r => r.BookId == bookId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return Ok(reviews);
        }
        [HttpPost("{bookId}/reviews")]
        public async Task<IActionResult> AddBookReview(int bookId, [FromBody] BookReview review, [FromServices] AppDbContext db)
        {
            if (review.Rating < 1 ||  review.Rating > 5)
            {
                return BadRequest("Rating must be between 1 and 5.");
            }

            review.BookId = bookId;
            db.BookReviews.Add(review);
            await db.SaveChangesAsync();

            
return Ok(new { message = "Review added successfully." });
        }
        // ✅ POST: api/notes
        [HttpPost("notes")]
        public async Task<IActionResult> AddNote([FromBody] Note note, [FromServices] AppDbContext db)
        {
            // تأكد من أن الملاحظة تحتوي على محتوى
            if (string.IsNullOrWhiteSpace(note.Content))
            {
                return BadRequest("Note content cannot be empty.");
            }

            // إضافة الملاحظة إلى قاعدة البيانات
            db.Notes.Add(note);
            await db.SaveChangesAsync();

            return Ok(new { message = "Note added successfully." });
        }
        // ✅ GET: api/notes/{userId}
        [HttpGet("notes/{userId}")]
        public async Task<IActionResult> GetNotes(int userId, [FromServices] AppDbContext db)
        {
            // جلب جميع الملاحظات الخاصة بالمستخدم المحدد
            var notes = await db.Notes.Where(n => n.UserId == userId).OrderByDescending(n => n.CreatedAt).ToListAsync();

            return Ok(notes);
        }

        [HttpGet("offers")]
        public async Task<IActionResult> GetSpecialOffers([FromServices] AppDbContext db)
        {
            var offerIds = await db.SpecialOffers
                .OrderByDescending(o => o.CreatedAt)
                .Take(10)
                .Select(o => o.BookId)
                .ToListAsync();

            var allBooks = await _service.GetBooksAsync();

            var offerBooks = allBooks
                .Where(b => offerIds.Contains(b.Id))
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    AuthorName = b.Authors?.FirstOrDefault()?.Name ?? "Unknown",
                    CoverImageUrl = b.CoverImageUrl ?? "",
                    Subjects = b.Subjects ?? new List<string>(),
                    Rating = 4.0
                }).ToList();

            return Ok(offerBooks);
        }
        [HttpGet("store")]
        public async Task<IActionResult> GetStoreSections([FromServices] AppDbContext db)
        {
            var newArrivals = await _service.GetNewArrivalsAsync();

            var topSellers = await GetPopularBooks(db) as OkObjectResult;
            var specialOffers = await GetSpecialOffers(db) as OkObjectResult;

            return Ok(new
            {
                TopSellers = topSellers?.Value,
                NewArrivals = newArrivals,
                SpecialOffers = specialOffers?.Value
            });
        }
        [HttpPost("offers/{bookId}")]
        public async Task<IActionResult> AddBookToOffers(int bookId, [FromServices] AppDbContext db)
        {
            if (!await db.SpecialOffers.AnyAsync(o => o.BookId == bookId))
            {
                db.SpecialOffers.Add(new SpecialOffer { BookId = bookId });
                await db.SaveChangesAsync();
            }

            return Ok(new { message = "Book added to special offers." });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            var allBooks = await _service.GetBooksAsync();
            var book = allBooks.FirstOrDefault(b => b.Id == id);

            if (book == null)
                return NotFound();

            var dto = new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                AuthorName = book.Authors?.FirstOrDefault()?.Name ?? "Unknown",
                CoverImageUrl = book.CoverImageUrl ?? "",
                Subjects = book.Subjects ?? new List<string>(),
                Rating = 4.0,
                Price = Math.Round((decimal)(new Random().NextDouble() * 10 + 1), 2),
                Description = $"This is a brief description of the book '{book.Title}' by {book.Authors?.FirstOrDefault()?.Name ?? "Unknown"}."
            };

            return Ok(dto);
        }

        
        [HttpPost("checkout")]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderDto order, [FromServices] AppDbContext db)
        {
            if (order == null||  order.BookIds == null|| !order.BookIds.Any())
                return BadRequest("Invalid order.");

            var newOrder = new Order
            {
                UserId = order.UserId,
                BookIds = string.Join(",", order.BookIds),
                DeliveryMethod = order.DeliveryMethod,
                PaymentMethod = order.PaymentMethod,
                PromoCode = order.PromoCode,
                TotalCost = order.TotalCost,
                CreatedAt = DateTime.UtcNow
            };

            db.Orders.Add(newOrder);
            await db.SaveChangesAsync();

            return Ok(new { message = "Order placed successfully." });
        }

        [HttpGet("promo/{code}")]
        public async Task<IActionResult> ApplyPromoCode(string code, [FromServices] AppDbContext db)
        {
            var promo = await db.PromoCodes.FirstOrDefaultAsync(p => p.Code == code);
            if (promo == null)
                return NotFound("Promo code not valid.");

            return Ok(new { DiscountAmount = promo.DiscountAmount });
        }

        [HttpGet]
        public async Task<IActionResult> GetAudioBooks()
        {
            var books = await _service.GetBooksAsync();
            var audioBooks = books
                .Where(b => b.Formats != null && b.Formats.Keys.Any(k => k.Contains("audio/mpeg")))
                .Select(b => new AudioBook
                {
                    Id = b.Id,
                    Title = b.Title,
                    AuthorName = b.Authors?.FirstOrDefault()?.Name ?? "Unknown",
                    AudioUrl = b.Formats.FirstOrDefault(x => x.Key.Contains("audio/mpeg")).Value,
                    CoverImageUrl = b.CoverImageUrl ?? "",
                    Narrator = "Unknown", // مفيش بيانات قارئ غالبًا من API
                    Duration = TimeSpan.Zero // تقدر تسيبها فاضية لو مش موجودة
                })
                .ToList();

            return Ok(audioBooks);
        }


    }
}