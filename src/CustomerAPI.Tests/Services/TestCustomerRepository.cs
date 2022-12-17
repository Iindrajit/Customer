using CustomerAPI.DbContexts;
using CustomerAPI.Services;
using CustomerAPI.Tests.MockData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CustomerAPI.Tests.Services
{
    public class TestCustomerRepository : IDisposable
    {
        private readonly CustomerContext _context;
        public TestCustomerRepository()
        {
            var options = new DbContextOptionsBuilder<CustomerContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new CustomerContext(options);
            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task GetCustomersAsync_ReturnCustomerList()
        {
            ///Arrange
            _context.Customers.AddRange(MockCustomerData.GetCustomers());
            await _context.SaveChangesAsync();
            var sut = new CustomerRepository(_context);

            ///Act
            var result = await sut.GetCustomersAsync(new Helpers.CustomerSearchParams());

            ///Assert
            result.Should().HaveCount(MockCustomerData.GetCustomers().Count());
        }

        [Fact]
        public async Task GetCustomerByIdAsync_ReturnCustomerIfCustomerIdMatches()
        {
            ///Arrange
            var customerToSearch = MockCustomerData.GetCustomers().FirstOrDefault();

            _context.Customers.AddRange(MockCustomerData.GetCustomers());
            await _context.SaveChangesAsync();
            var sut = new CustomerRepository(_context);

            ///Act
            var result = await sut.GetCustomerByIdAsync(customerToSearch.Id);

            ///Assert
            result.Should().NotBeNull();
            result.FirstName.Should().Be(customerToSearch.FirstName);
            result.LastName.Should().Be(customerToSearch.LastName);
        }


        [Fact]
        public async Task GetCustomerByIdAsync_ReturnNullIfCustomerIdDoesNotMatch()
        {
            ///Arrange
            var customerToSearch = MockCustomerData.CreateNewCustomer();

            _context.Customers.AddRange(MockCustomerData.GetCustomers());
            await _context.SaveChangesAsync();
            var sut = new CustomerRepository(_context);

            ///Act
            var result = await sut.GetCustomerByIdAsync(customerToSearch.Id);

            ///Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetCustomersAsync_ReturnCustomerListSearchByFirstName()
        {
            ///Arrange
            _context.Customers.AddRange(MockCustomerData.GetCustomers());
            await _context.SaveChangesAsync();

            var customerSearchParams = new Helpers.CustomerSearchParams { FirstName = "an" };
            var sut = new CustomerRepository(_context);

            ///Act
            var result = await sut.GetCustomersAsync(customerSearchParams);

            ///Assert
            var customersWithFirstNameLike =
                MockCustomerData.GetCustomers()
                                .Count(c => c.FirstName.ToLower().Contains(customerSearchParams.FirstName));

            result.Should().HaveCount(customersWithFirstNameLike);
        }

        [Fact]
        public async Task GetCustomersAsync_ReturnCustomerListSearchByLastName()
        {
            ///Arrange
            _context.Customers.AddRange(MockCustomerData.GetCustomers());
            await _context.SaveChangesAsync();

            var customerSearchParams = new Helpers.CustomerSearchParams { LastName = "oe" };
            var sut = new CustomerRepository(_context);

            ///Act
            var result = await sut.GetCustomersAsync(customerSearchParams);

            ///Assert
            var customersWithLastNameLike =
                MockCustomerData.GetCustomers()
                                .Count(c => c.LastName.ToLower().Contains(customerSearchParams.LastName));

            result.Should().HaveCount(customersWithLastNameLike);
        }

        [Fact]
        public async Task GetCustomersAsync_ReturnCustomerListSearchByFirstNameAndLastName()
        {
            ///Arrange
            _context.Customers.AddRange(MockCustomerData.GetCustomers());
            await _context.SaveChangesAsync();

            var customerSearchParams = new Helpers.CustomerSearchParams { FirstName = "ane", LastName = "do" };
            var sut = new CustomerRepository(_context);

            ///Act
            var result = await sut.GetCustomersAsync(customerSearchParams);
            ///Assert
            var customersWithFirstNameAndLastNameLike =
                MockCustomerData.GetCustomers()
                                .Count(c => c.FirstName.ToLower().Contains(customerSearchParams.FirstName) 
                                && c.LastName.ToLower().Contains(customerSearchParams.LastName));

            result.Should().HaveCount(customersWithFirstNameAndLastNameLike);
        }

        [Fact]
        public async Task SaveAsync_AddCustomer()
        {
            ///Arrange
            var newCustomer = MockCustomerData.CreateNewCustomer();
            _context.Customers.AddRange(MockCustomerData.GetCustomers());
            _context.SaveChanges();
            var recordExistsInDbBeforeSave = _context.Customers.Any(c => c.Id == newCustomer.Id);

            ///Act
            var sut = new CustomerRepository(_context);
            sut.AddCustomer(newCustomer);
            var result = await sut.SaveAsync();

            ///Assert
            result.Should().BeTrue();

            var recordExistsInDbAfterSave = _context.Customers.Any(c => c.Id == newCustomer.Id);
            recordExistsInDbBeforeSave.Should().NotBe(recordExistsInDbAfterSave);
            recordExistsInDbAfterSave.Should().BeTrue();

            int expectedRecordCount = MockCustomerData.GetCustomers().Count() + 1;
            _context.Customers.Count().Should().Be(expectedRecordCount);
        }

        [Fact]
        public async Task SaveAsync_DeleteCustomer()
        {
            ///Arrange
            var customersList = MockCustomerData.GetCustomers();
            var customerToDelete = customersList.FirstOrDefault();

            _context.Customers.AddRange(customersList);
            _context.SaveChanges();
            var recordExistsInDbBeforeDelete = _context.Customers.Any(c => c.Id == customerToDelete.Id);

            ///Act
            var sut = new CustomerRepository(_context);
            sut.DeleteCustomer(customerToDelete);
            var result = await sut.SaveAsync();

            ///Assert
            result.Should().BeTrue();

            var recordExistsInDbAfterDelete = _context.Customers.Any(c => c.Id == customerToDelete.Id);
            recordExistsInDbBeforeDelete.Should().NotBe(recordExistsInDbAfterDelete);
            recordExistsInDbAfterDelete.Should().BeFalse();

            int expectedRecordCount = customersList.Count() - 1;
            _context.Customers.Count().Should().Be(expectedRecordCount);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
