using AutoMapper;
using CustomerAPI.Controllers;
using CustomerAPI.Entities;
using CustomerAPI.Helpers;
using CustomerAPI.Services;
using CustomerAPI.Tests.MockData;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace CustomerAPI.Tests.Controllers
{
    public class TestCustomersController
    {
        private static IMapper _mapper;
        private static ILogger<CustomersController> _logger;

        public TestCustomersController()
        {
            if(_mapper == null)
            {
                var mapperConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new AutoMapperProfiles());
                });
                IMapper mapper = mapperConfig.CreateMapper();
                _mapper = mapper;
            }

            if(_logger == null)
            {
                _logger = new Mock<ILogger<CustomersController>>().Object;
            }
        }
        [Fact]
        public async Task GetCustomerById_ShouldReturnOK()
        {
            ///Arrange
            var customerRepository = new Mock<ICustomerRepository>();
            customerRepository.Setup(_ => _.GetCustomerByIdAsync(1))
                .ReturnsAsync(MockCustomerData.GetCustomer());

            var sut = new CustomersController(customerRepository.Object, _mapper, _logger);

            ///Act
            var result = await sut.GetCustomerByIdAsync(1);

            ///Assert
            var actionResult = result.Result;
            actionResult.Should().NotBeNull();
            actionResult.Should().BeOfType(typeof(OkObjectResult));
            (actionResult as OkObjectResult).StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetCustomerById_ShouldReturnNotFound()
        {
            ///Arrange
            var customerRepository = new Mock<ICustomerRepository>();
            customerRepository.Setup(_ => _.GetCustomerByIdAsync(1))
                              .Returns(Task.FromResult<Customer>(null));

            var sut = new CustomersController(customerRepository.Object, _mapper, _logger);

            ///Act
            var result = await sut.GetCustomerByIdAsync(1);

            ///Assert
            var actionResult = result.Result;
            actionResult.Should().NotBeNull();
            actionResult.Should().BeOfType(typeof(NotFoundResult));
            (actionResult as NotFoundResult).StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task GetCustomers_ShouldReturnOK()
        {
            ///Arrange
            var searchParams = new Helpers.CustomerSearchParams();
            var customerRepository = new Mock<ICustomerRepository>();
            customerRepository.Setup(_ => _.GetCustomersAsync(searchParams))
                .ReturnsAsync(MockCustomerData.GetCustomers());

            var sut = new CustomersController(customerRepository.Object, _mapper, _logger);

            ///Act
            var result = await sut.GetCustomersAsync(searchParams);

            ///Assert
            var actionResult = result.Result;
            actionResult.Should().NotBeNull();
            actionResult.Should().BeOfType(typeof(OkObjectResult));
            (actionResult as OkObjectResult).StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetCustomers_ShouldReturnNotFound()
        {
            ///Arrange
            var searchParams = new Helpers.CustomerSearchParams();
            var customerRepository = new Mock<ICustomerRepository>();
            customerRepository.Setup(_ => _.GetCustomersAsync(searchParams))
                .Returns(Task.FromResult<IEnumerable<Customer>>(null));

            var sut = new CustomersController(customerRepository.Object, _mapper, _logger);

            ///Act
            var result = await sut.GetCustomersAsync(searchParams);

            ///Assert
            var actionResult = result.Result;
            actionResult.Should().NotBeNull();
            actionResult.Should().BeOfType(typeof(NotFoundResult));
            (actionResult as NotFoundResult).StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task CreateCustomer_ShouldReturnCreated()
        {
            ///Arrange
            var customerRepository = new Mock<ICustomerRepository>();
            customerRepository.Setup(_ => _.SaveAsync())
                .Returns(Task.FromResult<bool>(true));

            var sut = new CustomersController(customerRepository.Object, _mapper, _logger);

            ///Act
            var result = await sut.CreateCustomerAsync(MockCustomerData.CreateCustomer());

            ///Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(CreatedAtRouteResult));
            (result as CreatedAtRouteResult).StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task UpdateCustomerAsync_ShouldReturnNotFound()
        {
            ///Arrange
            var updateCustomerDto = MockCustomerData.UpdateCustomer();
            var customerRepository = new Mock<ICustomerRepository>();
            customerRepository.Setup(c => c.GetCustomerByIdAsync(1))
                .Returns(Task.FromResult<Customer>(null));

            var sut = new CustomersController(customerRepository.Object, _mapper, _logger);

            ///Act
            var result = await sut.UpdateCustomerAsync(1, updateCustomerDto);

            ///Assert
            result.Should().BeOfType(typeof(NotFoundObjectResult));
            (result as NotFoundObjectResult).StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task UpdateCustomerAsync_ShouldReturnNoContent()
        {
            ///Arrange
            var updateCustomerDto = MockCustomerData.UpdateCustomer();
            var customerRepository = new Mock<ICustomerRepository>();
            customerRepository.Setup(_ => _.GetCustomerByIdAsync(1))
                .ReturnsAsync(MockCustomerData.GetCustomer());
            customerRepository.Setup(_ => _.SaveAsync())
                .Returns(Task.FromResult<bool>(true));

            var sut = new CustomersController(customerRepository.Object, _mapper, _logger);

            ///Act
            var result = await sut.UpdateCustomerAsync(1, updateCustomerDto);

            ///Assert
            result.Should().BeOfType(typeof(NoContentResult));
            (result as NoContentResult).StatusCode.Should().Be(204);
        }

        [Fact]
        public async Task DeleteCustomerAsync_ShouldReturnNotFound()
        {
            ///Arrange
            var customerRepository = new Mock<ICustomerRepository>();
            customerRepository.Setup(c => c.GetCustomerByIdAsync(1))
                .Returns(Task.FromResult<Customer>(null));

            var sut = new CustomersController(customerRepository.Object, _mapper, _logger);

            ///Act
            var result = await sut.DeleteCustomerAsync(1);

            ///Assert
            result.Should().BeOfType(typeof(NotFoundObjectResult));
            (result as NotFoundObjectResult).StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task DeleteCustomerAsync_ShouldReturnNoContent()
        {
            ///Arrange
            var customerRepository = new Mock<ICustomerRepository>();
            customerRepository.Setup(_ => _.GetCustomerByIdAsync(1))
                .ReturnsAsync(MockCustomerData.GetCustomer());
            customerRepository.Setup(_ => _.SaveAsync())
                .Returns(Task.FromResult<bool>(true));

            var sut = new CustomersController(customerRepository.Object, _mapper, _logger);

            ///Act
            var result = await sut.DeleteCustomerAsync(1);

            ///Assert
            result.Should().BeOfType(typeof(NoContentResult));
            (result as NoContentResult).StatusCode.Should().Be(204);
        }

    }
}
