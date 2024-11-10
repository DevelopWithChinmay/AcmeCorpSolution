using AcmeCorpBusiness.Domain.Contacts;
using AcmeCorpBusiness.Domain.Customers;
using AcmeCorpBusiness.Entities;
using AcmeCorpBusiness.Exceptions;
using AcmeCorpBusiness.Helpers;
using Castle.Core.Resource;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace AcmeCorpBusinessTest
{
    [TestFixture]
    public class CustomerReadHandlerTests
    {
        private AcmeCorpDbContext context;
        private CustomerReadHandler handler;

        [SetUp]
        public void Setup()
        {
            MappingHelper.Configure();

            var options = new DbContextOptionsBuilder<AcmeCorpDbContext>()
                .UseInMemoryDatabase(databaseName: "AcmeCorpDb")
                .Options;

            context = new AcmeCorpDbContext(options);

            // Seed initial data
            context.Customers.AddRange(new List<Customer>
            {
                new() { Id = 1, Name = "Chinmay 1", Email = "chinmay1@xyz.com" },
                new() { Id = 2, Name = "Chinmay 2", Email = "chinmay2@xyz.com" }
            });
            context.SaveChanges();

            handler = new CustomerReadHandler(context);
        }

        [Test]
        public async Task Query_ShouldReturnCustomersAsync()
        {
            var result = await handler.QueryAsync();

            result.Should().HaveCount(2);
            var firstCustomer = result.First();
            firstCustomer.Should().BeEquivalentTo(new CustomerFacade()
            {
                Id = 1,
                Name = "Chinmay 1"
            }, opt => opt.Excluding(i => i.ContactDetail));

            firstCustomer.ContactDetail.Email.Should().Be("chinmay1@xyz.com");
        }

        [Test]
        public async Task GetById_ShouldReturnCustomer_WhenExists()
        {
            var result = await handler.GetByIdAsync(1);

            result.Should().NotBeNull();
            result.Name.Should().Be("Chinmay 1");
        }

        [Test]
        public void GetById_ShouldThrowItemNotFoundException_WhenNotExists()
        {
            var customerId = 99;
            var exception = Assert.ThrowsAsync<ItemNotFoundException>(async () => await handler.GetByIdAsync(customerId));
            Assert.That(exception.Message, Is.EqualTo($"Item 'Customer' with ID {customerId} was not found."));
        }

        [TearDown]
        public void TearDown()
        {
            context.Dispose();
        }
    }
}