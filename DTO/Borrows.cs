using System;

namespace DTO
{
    public class Borrows
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime BorrowReturnDate { get; set; }
        public string State { get; set; }

    }
}
