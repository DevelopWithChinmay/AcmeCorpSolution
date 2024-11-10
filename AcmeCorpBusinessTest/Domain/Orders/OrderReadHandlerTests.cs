using AcmeCorpBusiness.Domain.Contacts;
using AcmeCorpBusiness.Domain.Customers;
using AcmeCorpBusiness.Domain.Orders;
using AcmeCorpBusiness.Entities;
using AcmeCorpBusiness.Exceptions;
using AcmeCorpBusiness.Helpers;
using FluentAssertions;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AcmeCorpBusinessTest.Domain.Orders
{
    [TestFixture]
    public class OrderReadHandlerTests
    {
        private AcmeCorpDbContext context;
        private OrderReadHandler handler;
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
            handler = new OrderReadHandler(context);
            context.Customers.Add(customerFacade.Adapt<Customer>());
            context.Orders.AddRange(new List<Order>
            {
                new() { Id = 1, CustomerId = 1, OrderDate = DateTime.Now, TotalAmount = 100.0 },
                new() { Id = 2, CustomerId = 1, OrderDate = DateTime.Now.AddDays(-1), TotalAmount = 150.0 },
                new() { Id = 3, CustomerId = 2, OrderDate = DateTime.Now.AddDays(-2), TotalAmount = 200.0 }
            });
            context.SaveChanges();
        }

        [Test]
        public async Task QueryByCustomerId_ShouldReturnOnlyCustomerOrders()
        {
            int customerId = 1;
            var result = await handler.QueryByCustomerIdAsync(customerId);

            result.Should().HaveCount(2);
            result.Should().AllSatisfy(order => order.CustomerId.Should().Be(customerId));
        }

        [Test]
        public void QueryByCustomerId_ThrowsExceptionForInvalidCustomerId()
        {
            int nonExistentCustomerId = 99;
            var exception = Assert.ThrowsAsync<ItemNotFoundException>(async () => await handler.QueryByCustomerIdAsync(nonExistentCustomerId));
            exception.Message.Should().Be($"Item 'Customer' with ID {nonExistentCustomerId} was not found.");
        }

        [Test]
        public async Task GetById_ShouldReturnOrder_ForValidId()
        {
            int orderId = 1;
            var result = await handler.GetByIdAsync(orderId);

            result.Should().NotBeNull();
            result.Id.Should().Be(orderId);
            result.TotalAmount.Should().Be(100.0);
        }

        [Test]
        public void GetById_ThrowsItemNotFound_ForInvalidId()
        {
            int nonExistentOrderId = 99;

            var exception = Assert.ThrowsAsync<ItemNotFoundException>(async () => await handler.GetByIdAsync(nonExistentOrderId));
            exception.Message.Should().Be($"Item 'Order' with ID {nonExistentOrderId} was not found.");
        }

        [TearDown]
        public void TearDown()
        {
            context.Dispose();
        }
    }
}
