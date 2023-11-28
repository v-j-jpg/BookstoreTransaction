using CommonLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public class BookDatabase
    {
        public List<Book> Books { get; set; }

        public BookDatabase()
        {
            Books = new List<Book>() {
            new Book(0,"The Enigma Code","A thrilling espionage novel set during World War II, where a brilliant codebreaker races against time to decipher an enemy code that could change the course of history.", "Rebecca Sterling", 14.00, 50 ),
            new Book(1, "Beyond the Stars", "A captivating science fiction epic that explores the mysteries of the universe as a group of astronauts embarks on a perilous journey to a distant galaxy.","Jonathan Nova", 19.99,30 ),
            new Book(2, "The Art of Silence", "A poignant exploration of a mute artist's journey to express emotions through his paintings, delving into the power of art in communication.", "Olivia Serene", 12.50, 5),
            new Book(3,"Shadows of the Past", "In this gripping historical mystery, a detective unravels a web of secrets and conspiracies that link back to a forgotten chapter of the city's history.", "Marcus Shadows", 16.75, 25 ),
            new Book(4, "The Quantum Paradox", "A mind-bending journey into the world of quantum physics, challenging readers to rethink their understanding of reality and the nature of existence.", "Dr. Evelyn Quantum", 22.50,  20)
            };
        }

        public bool BookExists(Book book)
        {
            foreach (var item in Books)
            {
                if(item.Equals(book)) return true;
            };

            return false;
        }

    }
}
