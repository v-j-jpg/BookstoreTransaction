using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Model
{
    [DataContract]
    public class BookCustomer 
    {
        [DataMember]
        public long BookID { get; set; }

        [DataMember]
        public string? BookTitle { get; set; }

        [DataMember]
        public string? BookDescription { get; set; }

        [DataMember]
        public string? BookAuthor { get; set; }

        [DataMember]
        public double? Price { get; set; }

        [DataMember]
        public uint Quantity { get; set; }

        [DataMember]
        public uint Count { get; set; } = 1;

        [DataMember]
        public string? FirstName { get; set; }

        [DataMember]
        public string? LastName { get; set; }

        [DataMember]
        public string? Address { get; set; }

        [DataMember]
        public long BankCardNumber { get; set; }

        [DataMember]
        public int? CVV { get; set; }

        [DataMember]
        public string? ExpirationDate { get; set; }

        [DataMember]
        public double? Money { get; set; }

        [DataMember]
        public string? NameOnCard { get; set; }

        public BookCustomer(long bookID, string? bookTitle, string? bookDescription, string? bookAuthor, double? price, uint count, string? firstName, string? lastName, string? address, long bankCardNumber, int? cVV, string? expirationDate, string? nameOnCard)
        {
            BookID = bookID;
            BookTitle = bookTitle;
            BookDescription = bookDescription;
            BookAuthor = bookAuthor;
            Price = price;
            Count = count;
            FirstName = firstName;
            LastName = lastName;
            Address = address;
            BankCardNumber = bankCardNumber;
            CVV = cVV;
            ExpirationDate = expirationDate;
            NameOnCard = nameOnCard;
        }

        public BookCustomer() { }

        public override string? ToString()
        {
            return base.ToString();
        }

    }
}
