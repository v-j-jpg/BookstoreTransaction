using CommonLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public class CustomerDatabase
    {
        public List<Customer> Customers { get; set; }

        public CustomerDatabase() { 
        
            Customers = new List<Customer>() { 
                new Customer("Alice", "Johnson", "123 Main Street, Cityville, USA", 1234567890123456, 500.00, 789, "10/26", "Alice Johnson"),
                new Customer("Bob", "Smith", "456 Oak Avenue, Townsville, USA", 9876543210987654, 750.50, 999, "01/27", "Bob Smith"), 
                new Customer("Charlie", "Davis", "789 Pine Lane, Villagetown", 1111222233334444, 800.75, 888, "03/30", "Charlie Davis")
            };
        }
        
    }
}
