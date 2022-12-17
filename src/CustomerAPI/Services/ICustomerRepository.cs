using CustomerAPI.Entities;
using CustomerAPI.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerAPI.Services
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetCustomersAsync(CustomerSearchParams customerSearchBy);
        Task<Customer> GetCustomerByIdAsync(int id);
        void AddCustomer(Customer customer);
        void UpdateCustomer(Customer customer);
        void DeleteCustomer(Customer customer);
        Task<bool> SaveAsync();
    }
}
