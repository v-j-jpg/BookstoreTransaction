using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Model
{
    [DataContract]
    public class Book 
    {
        [DataMember]
        public long BookID { get; set; }

        [DataMember]
        public string? BookTitle { get; set; }

        [DataMember]
        public string? BookDescription { get; set;}

        [DataMember] 
        public string? BookAuthor { get;set; }

        [DataMember]
        public double? Price { get; set; }

        [DataMember]
        public uint Quantity { get; set; }

        public Book(long bookID, string? bookTitle, string? bookDescription, string? bookAuthor, double? price, uint quantity)
        {
            BookID = bookID;
            BookTitle = bookTitle;
            BookDescription = bookDescription;
            BookAuthor = bookAuthor;
            Price = price;
            Quantity = quantity;
        }

        public Book() { }

        public override string? ToString()
        {
            return base.ToString();
        }
    }
}
