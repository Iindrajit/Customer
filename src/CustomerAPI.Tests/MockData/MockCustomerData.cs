using CustomerAPI.Entities;
using CustomerAPI.Models;
using System;
using System.Collections.Generic;

namespace CustomerAPI.Tests.MockData
{
    public class MockCustomerData
    {
        public static Customer GetCustomer()
        {
            return new Customer
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.Now
            };
        }

        public static IEnumerable<Customer> GetCustomers()
        {
            return new List<Customer>
            {
                new Customer
                {
                    Id = 2,
                    FirstName = "Allan",
                    LastName = "Donald",
                    DateOfBirth = DateTime.Now
                },
                new Customer
                {
                    Id = 3,
                    FirstName = "John",
                    LastName = "Doe",
                    DateOfBirth = DateTime.Now
                },
                new Customer
                {
                    Id = 4,
                    FirstName = "Jane",
                    LastName = "Doe",
                    DateOfBirth = DateTime.Now
                },
                 new Customer
                {
                    Id = 5,
                    FirstName = "Kane",
                    LastName = "Doe",
                    DateOfBirth = DateTime.Now
                }
            };
        }

        public static CreateCustomerDto CreateCustomer(bool isValid = true)
        {
            if(!isValid)
            {
                return new CreateCustomerDto();
            }

            return new CreateCustomerDto
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.Now
            };
        }

        public static Customer CreateNewCustomer()
        {
            return new Customer
            {
                Id = 0,
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.Now
            };
        }

        public static UpdateCustomerDto UpdateCustomer()
        {
            return new UpdateCustomerDto
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.Now
            };
        }
    }
}
