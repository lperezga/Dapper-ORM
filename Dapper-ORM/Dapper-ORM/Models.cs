using System;
using System.Collections.Generic;

namespace Dapper_ORM
{
    public class Models
    {
        public class Customer
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string City { get; set; }
            public string Country { get; set; }
            public string Phone { get; set; }
            public List<Order> Orders { get; set; }
        }

        public class Order
        {
            public int Id { get; set; }
            public DateTime OrderDate { get; set; }
            public string OrderNumber { get; set; }
            public int CustomerId { get; set; }
            public decimal TotalAmount { get; set; }
        }
    }
}
