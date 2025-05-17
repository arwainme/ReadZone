using Microsoft.EntityFrameworkCore;
using ReadZone.Models;

namespace ReadZone.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets لكل الجداول
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<BookAuthor> BookAuthors { get; set; }
        public DbSet<AudioBook> AudioBooks { get; set; }
        public DbSet<BookRating> BookRatings { get; set; }
        public DbSet<BookReview> BookReviews { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<SpecialOffer> SpecialOffers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<PromoCode> PromoCodes { get; set; }
        public DbSet<UserFavoriteBook> UserFavoriteBooks { get; set; }
        public DbSet<UserBookmarkBook> UserBookmarkBooks { get; set; }
        public DbSet<UserDownloadedBook> UserDownloadedBooks { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. تكوين العلاقة Many-to-Many بين Book و Author
            modelBuilder.Entity<BookAuthor>()
                .HasKey(ba => new { ba.BookId, ba.AuthorId });

            modelBuilder.Entity<BookAuthor>()
                .HasOne(ba => ba.Book)
                .WithMany(b => b.BookAuthors)
                .HasForeignKey(ba => ba.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookAuthor>()
                .HasOne(ba => ba.Author)
                .WithMany(a => a.BookAuthors)
                .HasForeignKey(ba => ba.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            // 2. تكوين علاقة Post مع User (Cascade Delete)
            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 3. إصلاح مشكلة PostLike (منع Cascade Delete من User)
            modelBuilder.Entity<PostLike>()
                  .HasOne(pl => pl.User)
                  .WithMany(u => u.PostLikes)
                  .HasForeignKey(pl => pl.UserId)
                  .OnDelete(DeleteBehavior.Restrict); // التغيير هنا

            modelBuilder.Entity<PostLike>()
                .HasOne(pl => pl.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(pl => pl.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // 4. تكوين علاقة Comment
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // 5. تكوين الخصائص الرقمية بدقة
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalCost)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PromoCode>()
                .Property(p => p.DiscountAmount)
                .HasColumnType("decimal(18,2)");

            // 6. تجاهل الخصائص غير المرتبطة بقاعدة البيانات
            modelBuilder.Entity<Book>()
                .Ignore(b => b.Formats)
                .Ignore(b => b.Subjects)
                .Ignore(b => b.CoverImageUrl);

            // 7. تكوين علاقات UserFavoriteBook
            modelBuilder.Entity<UserFavoriteBook>()
                .HasOne(ufb => ufb.User)
                .WithMany(u => u.FavoriteBooks)
                .HasForeignKey(ufb => ufb.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserFavoriteBook>()
                .HasOne(ufb => ufb.Book)
                .WithMany()
                .HasForeignKey(ufb => ufb.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            // 8. تكوين علاقات UserBookmarkBook
            modelBuilder.Entity<UserBookmarkBook>()
                .HasOne(ubb => ubb.User)
                .WithMany(u => u.BookmarkedBooks)
                .HasForeignKey(ubb => ubb.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserBookmarkBook>()
                .HasOne(ubb => ubb.Book)
                .WithMany()
                .HasForeignKey(ubb => ubb.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            // 9. تكوين علاقات UserDownloadedBook
            modelBuilder.Entity<UserDownloadedBook>()
                .HasOne(udb => udb.User)
                .WithMany(u => u.DownloadedBooks)
                .HasForeignKey(udb => udb.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserDownloadedBook>()
                .HasOne(udb => udb.Book)
                .WithMany()
                .HasForeignKey(udb => udb.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.RecipientUser)
                .WithMany()
                .HasForeignKey(n => n.RecipientUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.SourceUser)
                .WithMany()
                .HasForeignKey(n => n.SourceUserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}