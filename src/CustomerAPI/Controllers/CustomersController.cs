using AutoMapper;
using CustomerAPI.Entities;
using CustomerAPI.Helpers;
using CustomerAPI.Models;
using CustomerAPI.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace CustomerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController] 
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(ICustomerRepository customerRepository, IMapper mapper, 
            ILogger<CustomersController> logger)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{id:int}", Name ="GetCustomerById")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<ActionResult<CustomerDto>> GetCustomerByIdAsync(int id)
        {
            var customerFromRepo = await _customerRepository.GetCustomerByIdAsync(id);

            if(customerFromRepo == null)
            {
                return NotFound();
            }

            var customerToReturn = _mapper.Map<CustomerDto>(customerFromRepo);
            return Ok(customerToReturn);
        }

        [HttpGet()]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomersAsync(
            [FromQuery] CustomerSearchParams customerSearchBy)
        {
            var customersFromRepo = await _customerRepository.GetCustomersAsync(customerSearchBy);

            if (customersFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<IEnumerable<CustomerDto>>(customersFromRepo));
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateCustomerAsync(CreateCustomerDto customer)
        {
            var newCustomer = _mapper.Map<Customer>(customer);
            _customerRepository.AddCustomer(newCustomer);
            bool isCreated = await _customerRepository.SaveAsync();

            if(isCreated)
            {
                var customerDto = _mapper.Map<CustomerDto>(newCustomer);
                _logger.LogInformation($"Customer with First name: {newCustomer.FirstName}, Last name: {newCustomer.LastName} created.");
                return CreatedAtRoute("GetCustomerById", new { id = newCustomer.Id }, customerDto);
            }

            _logger.LogError($"Error while trying to add customer: First name: {customer.FirstName}, Last name: {customer.LastName}");
            return StatusCode(500);
        }


        [HttpOptions]
        public IActionResult GetCustomerOptions()
        {
            Response.Headers.Add("Allow", "GET,POST,PUT,PATCH,DELETE");
            return Ok();
        }


        [HttpPut("{customerId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> UpdateCustomerAsync(int customerId, UpdateCustomerDto customer)
        {
            var customerFromRepo = await _customerRepository.GetCustomerByIdAsync(customerId);

            if(customerFromRepo == null)
            {
                return NotFound($"Customer with id: {customerId} not found to update.");
            }

            _mapper.Map(customer, customerFromRepo);
            _customerRepository.UpdateCustomer(customerFromRepo);

            await _customerRepository.SaveAsync();
            return NoContent();
        }

        [HttpPatch("{customerId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> PatchUpdateCustomerAsync(int customerId, 
            JsonPatchDocument<UpdateCustomerDto> customerPatchDocument)
        {
            var customerFromRepo = await _customerRepository.GetCustomerByIdAsync(customerId);

            if (customerFromRepo == null)
            {
                return NotFound($"Customer with id: {customerId} not found to update.");
            }

            var customerToPatch = _mapper.Map<UpdateCustomerDto>(customerFromRepo);

            customerPatchDocument.ApplyTo(customerToPatch, ModelState);

            if(!TryValidateModel(customerToPatch))
            {
                return ValidationProblem(ModelState);
            }
            _mapper.Map(customerToPatch, customerFromRepo);
            _customerRepository.UpdateCustomer(customerFromRepo);

            await _customerRepository.SaveAsync();
            return NoContent();
        }

        [HttpDelete("{customerId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteCustomerAsync(int customerId)
        {
            var customerFromRepo = await _customerRepository.GetCustomerByIdAsync(customerId);

            if (customerFromRepo == null)
            {
                return NotFound($"Customer with id: {customerId} not found to delete.");
            }

            _customerRepository.DeleteCustomer(customerFromRepo);
            await _customerRepository.SaveAsync();
            return NoContent();
        }
    }
}
