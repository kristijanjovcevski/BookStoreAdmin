namespace BookStoreAdminApplication.Models
{
    public class Order
    {
        public Guid Id { get; set; }

        public string OwnerId { get; set; }
        public BookStoreApplicationUser Owner { get; set; }
        public ICollection<BookInOrder> BooksInOrder { get; set; }
    }
}
