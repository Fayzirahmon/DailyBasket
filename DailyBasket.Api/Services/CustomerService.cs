// ====================================================================
// Component: Services/CustomerService.cs
// Layer: Business Logic Layer
// Responsibility: Handles business logic and validations for customer accounts.
// ====================================================================

using DailyBasket.Api.DTOs.Customers;
using DailyBasket.Api.Exceptions;
using DailyBasket.Api.Mappings;
using DailyBasket.Api.Models;
using DailyBasket.Api.Repositories.Interfaces;
using DailyBasket.Api.Services.Interfaces;

namespace DailyBasket.Api.Services;

/// <summary>
/// Service implementation managing all business rules for customer registration and management,
/// including unique email check validations and relational integrity rules for deletion.
/// </summary>
public class CustomerService(ICustomerRepository customerRepository) : ICustomerService
{
    /// <summary>
    /// Retrieves a list of all registered customer records.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, returning a read-only list of <see cref="CustomerResponse"/>.</returns>
    public async Task<IReadOnlyList<CustomerResponse>> GetAllAsync()
    {
        var customers = await customerRepository.GetAllAsync();
        return customers.Select(customer => customer.ToResponse()).ToList();
    }

    /// <summary>
    /// Retrieves a specific customer's details by their unique ID.
    /// </summary>
    /// <param name="id">The unique database customer ID.</param>
    /// <returns>A task representing the asynchronous operation, returning the <see cref="CustomerResponse"/>.</returns>
    /// <exception cref="NotFoundException">Thrown when the customer ID is not found.</exception>
    public async Task<CustomerResponse> GetByIdAsync(int id)
    {
        var customer = await customerRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Customer with ID {id} was not found.");

        return customer.ToResponse();
    }

    /// <summary>
    /// Registers a new customer after cleaning user inputs and validating email uniqueness.
    /// </summary>
    /// <param name="request">The customer registration payload.</param>
    /// <returns>A task representing the asynchronous operation, returning the created <see cref="CustomerResponse"/>.</returns>
    /// <exception cref="BadRequestException">Thrown when the email address is already registered.</exception>
    public async Task<CustomerResponse> CreateAsync(CustomerCreateRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await customerRepository.GetByEmailAsync(email) is not null)
        {
            throw new BadRequestException("A customer with this email already exists.");
        }

        var customer = new Customer
        {
            FullName = request.FullName.Trim(),
            Email = email,
            PhoneNumber = request.PhoneNumber.Trim(),
            Address = request.Address.Trim()
        };

        await customerRepository.AddAsync(customer);
        return customer.ToResponse();
    }

    /// <summary>
    /// Updates the data details of an existing customer record, ensuring the email stays unique.
    /// </summary>
    /// <param name="id">The unique database ID of the customer to update.</param>
    /// <param name="request">The customer update request payload.</param>
    /// <returns>A task representing the asynchronous update operation.</returns>
    /// <exception cref="NotFoundException">Thrown when the target customer is not found.</exception>
    /// <exception cref="BadRequestException">Thrown when the new email is already claimed by another active customer.</exception>
    public async Task UpdateAsync(int id, CustomerUpdateRequest request)
    {
        var customer = await customerRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Customer with ID {id} was not found.");

        var email = request.Email.Trim().ToLowerInvariant();
        var existingEmailOwner = await customerRepository.GetByEmailAsync(email);
        if (existingEmailOwner is not null && existingEmailOwner.CustomerId != id)
        {
            throw new BadRequestException("Another customer already uses this email.");
        }

        customer.FullName = request.FullName.Trim();
        customer.Email = email;
        customer.PhoneNumber = request.PhoneNumber.Trim();
        customer.Address = request.Address.Trim();

        await customerRepository.UpdateAsync(customer);
    }

    /// <summary>
    /// Deletes a customer account if there are no linked active cart items or completed orders.
    /// </summary>
    /// <param name="id">The unique database ID of the customer to delete.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    /// <exception cref="NotFoundException">Thrown when the customer is not found.</exception>
    /// <exception cref="BadRequestException">Thrown when attempting to delete a customer who has existing carts or orders.</exception>
    public async Task DeleteAsync(int id)
    {
        var customer = await customerRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Customer with ID {id} was not found.");

        if (await customerRepository.HasCartItemsAsync(id) || await customerRepository.HasOrdersAsync(id))
        {
            throw new BadRequestException("Customer cannot be deleted while they have cart items or orders.");
        }

        await customerRepository.DeleteAsync(customer);
    }
}
