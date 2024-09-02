using System.ComponentModel.DataAnnotations;

namespace BookStoreAdminApplication.Models
{
    public class Book
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string ISBN { get; set; }

        [Required]
        public string BookImage { get; set; }

        [Required]
        public string AuthorFullName { get; set; }


        [Required]
        public string PublisherName { get; set; }

        [Required]
        public float Price { get; set; }

        [Required]
        public DateTime PublishedDate { get; set; }
    }
}
