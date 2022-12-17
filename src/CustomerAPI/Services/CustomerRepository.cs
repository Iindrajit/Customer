using CustomerAPI.DbContexts;
using CustomerAPI.Entities;
using CustomerAPI.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAPI.Services
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerContext _context;

        public CustomerRepository(CustomerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void AddCustomer(Customer customer)
        {
            if(customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            _context.Customers.Add(customer);
        }

        public void DeleteCustomer(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            _context.Customers.Remove(customer);
        }

        public async Task<IEnumerable<Customer>> GetCustomersAsync(CustomerSearchParams customerSearchBy)
        {
            var customers = _context.Customers as IQueryable<Customer>;

            if (!string.IsNullOrEmpty(customerSearchBy.FirstName))
            {
                customers = customers.Where(c =>
                c.FirstName.ToLower().Trim().Contains(customerSearchBy.FirstName.ToLower().Trim()));

            }
            if (!string.IsNullOrEmpty(customerSearchBy.LastName))
            {
                customers = customers.Where(c =>
                c.LastName.ToLower().Trim().Contains(customerSearchBy.LastName.ToLower().Trim()));
            }

            return await customers.ToListAsync();
        }

        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
            return await _context.Customers.FindAsync(id);
        }

        public void UpdateCustomer(Customer customer)
        {
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

    }
}
