using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Model
{
    [DataContract]
    public class Customer
    {
        [DataMember]
        public string? FirstName { get; set; }

        [DataMember]
        public string? LastName { get; set; }

        [DataMember]
        public string? Address { get; set; }

        [DataMember]
        public long BankCardNumber { get; set; }

        [DataMember]
        public int? CVV { get; set;}

        [DataMember]
        public string? ExpirationDate { get; set; }

        [DataMember]
        public double? Money { get; set; }

        [DataMember]
        public string? NameOnCard { get; set; }


        public Customer(string? firstName, string? lastName, string? address, long bankCardNumber, double? money, int? cVV, string? expirationDate, string? nameOnCard)
        {
            FirstName = firstName;
            LastName = lastName;
            Address = address;
            BankCardNumber = bankCardNumber;
            Money = money;
            CVV = cVV;
            ExpirationDate = expirationDate;
            NameOnCard = nameOnCard;
        }

        public Customer() {}
    }
}
