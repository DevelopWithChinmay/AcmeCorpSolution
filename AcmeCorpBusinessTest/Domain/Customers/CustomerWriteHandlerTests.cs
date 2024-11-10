using AcmeCorpBusiness.Domain.Contacts;
using AcmeCorpBusiness.Domain.Customers;
using AcmeCorpBusiness.Entities;
using AcmeCorpBusiness.Exceptions;
using AcmeCorpBusiness.Helpers;
using FluentAssertions;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AcmeCorpBusinessTest.Domain.Customers
{
    [TestFixture]
    public class CustomerWriteHandlerTests
    {
        private AcmeCorpDbContext context;
        private CustomerWriteHandler handler;
        private readonly CustomerFacade customerFacade = new()
        {
            Id = 1,
            DateOfBirth = new DateTime(1990, 01, 01, 9, 15, 0),
            Name = "Chinmay 1",
            ContactDetail = new ContactDetailFacade
            {
                Email = "chinmay1@xyz.com",
                PhoneNumber = "123-456-7890",
                MailingAddress = "123, Vaishali Nagar, Jaipur"
            }
        };

        [SetUp]
        public void Setup()
        {
            MappingHelper.Configure();

            var databaseName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<AcmeCorpDbContext>()
                .UseInMemoryDatabase(databaseName: $"AcmeCorpDb_{databaseName}")
                .Options;

            context = new AcmeCorpDbContext(options);
            handler = new CustomerWriteHandler(context);
        }

        [Test]
        public async Task Write_ShouldAddNewCustomer()
        {
            context.Customers.Should().HaveCount(0);
            var result = await handler.WriteAsync(customerFacade with { Id = 0 });
            context.Customers.Should().HaveCount(1);
            result.Should().BeEquivalentTo(customerFacade, opt => opt.Excluding(i => i.Id));
            result.Id.Should().BePositive();
        }

        [Test]
        public async Task Write_ShouldUpdateExistingCustomer_ForValidId()
        {
            context.Customers.Add(customerFacade.Adapt<Customer>());
            await context.SaveChangesAsync();

            var updatedCustomerFacade = customerFacade with
            {
                Name = "Chinmay Upudated",
                DateOfBirth = DateTime.UtcNow,  // This should not get updated after customer update
                ContactDetail = new ContactDetailFacade
                {
                    Email = "chinmay_updated@xyz.com",
                    PhoneNumber = "555-555-5555",
                    MailingAddress = "125, Mehrauli, Gurugram"
                }
            };

            await handler.WriteAsync(updatedCustomerFacade);
            
            var updatedCustomer = await context.Customers.FirstOrDefaultAsync(c => c.Id == 1);
            updatedCustomer.Should().NotBeNull();
            updatedCustomer.Should().BeEquivalentTo(new Customer()
            {
                Id = 1,
                DateOfBirth = new DateTime(1990, 01, 01, 9, 15, 0),
                Name = "Chinmay Upudated",
                Email = "chinmay_updated@xyz.com",
                MailingAddress = "125, Mehrauli, Gurugram",
                PhoneNumber = "555-555-5555"

            });
        }

        [Test]
        public async Task WriteAsync_ShouldThrowItemNotFoundException_ForInvalidId()
        {
            var nonExistentCustomerId = 99;
            context.Customers.Add(customerFacade.Adapt<Customer>());
            await context.SaveChangesAsync();

            var updatedCustomerFacade = customerFacade with
            {
                Id = nonExistentCustomerId,
                Name = "Chinmay Upudated"
            };

            var exception = Assert.ThrowsAsync<ItemNotFoundException>(async () => await handler.WriteAsync(updatedCustomerFacade));
            exception.Message.Should().Be($"Item 'Customer' with ID {nonExistentCustomerId} was not found.");
        }

        [TearDown]
        public void TearDown()
        {
            context.Dispose();
        }
    }
}
