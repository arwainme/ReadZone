using System.ComponentModel.DataAnnotations.Schema;

namespace ReadZone.Models;
public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string AuthorName { get; set; }
    public string? CoverImageUrl { get; set; }
    public List<string> Subjects { get; set; }
    public double Rating { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
}
