using System;
using System.Collections.Generic;
using System.Text;

namespace MyCompany.Domains
{
    public class CustomerRepository : ICustomerRepository
    {
        public void Save(Customer customer)
        {
            Console.WriteLine(string.Concat("The concrete customer repository was called for customer ", customer.Name));
        }
    }
}
