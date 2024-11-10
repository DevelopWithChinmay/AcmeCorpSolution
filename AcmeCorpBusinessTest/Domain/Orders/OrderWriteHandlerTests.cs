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
    public class OrderWriteHandlerTests
    {
        private AcmeCorpDbContext context;
        private OrderWriteHandler handler;
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
        private readonly OrderFacade orderFacade = new()
        {
            Id = 1,
            CustomerId = 1,
            OrderDate = new DateTime(2019, 12, 5, 9, 15, 0),
            TotalAmount = 100.0,
            ProductName = "Product 1"
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
            handler = new OrderWriteHandler(context);

            context.Customers.Add(customerFacade.Adapt<Customer>());
            context.Orders.Add(orderFacade.Adapt<Order>());
            context.SaveChanges();
        }

        [Test]
        public async Task Write_ShouldAddNewOrder()
        {
            context.Orders.Should().HaveCount(1);
            var result = await handler.WriteAsync(orderFacade with { Id = 0 } ); // Simulating a new order (ID <= 0)
            
            result.Should().NotBeNull();
            result.Id.Should().BePositive();
            result.Should().BeEquivalentTo(orderFacade, opt => opt.Excluding(i => i.Id));

            context.Orders.Should().HaveCount(2);
        }

        [Test]
        public void Write_ThrowsItemNotFoundException_ForInvalidCustomerId()
        {
            var invalidCustomerId = 99;
            var exception = Assert.ThrowsAsync<ItemNotFoundException>(async () =>
                await handler.WriteAsync(orderFacade with { Id = 0, CustomerId = invalidCustomerId }));
            exception.Message.Should().Be($"Item 'Customer' with ID {invalidCustomerId} was not found.");
        }

        [Test]
        public async Task Write_ShouldUpdateOrder_ForValidId()
        {
            var result = await handler.WriteAsync(orderFacade with
            {
                CustomerId = 2, // This shold not get changed
                OrderDate = DateTime.Now, // This should not get changed
                TotalAmount = 250,
                ProductName = "New Product"
            });

            var updatedOrder = await context.Orders.FirstOrDefaultAsync(x => x.Id == orderFacade.Id);
            updatedOrder.Should().NotBeNull();

            // Updated Values
            updatedOrder!.TotalAmount.Should().Be(250);
            updatedOrder!.ProductName.Should().Be("New Product");

            // Values which should not have been updated
            updatedOrder!.CustomerId.Should().Be(1);
            updatedOrder!.OrderDate.Should().Be(new DateTime(2019, 12, 5, 9, 15, 0));
        }

        [Test]
        public void Write_ThrowsItemNotFoundException_ForInvalidOrderId()
        {
            var invalidId = 99;
            var exception = Assert.ThrowsAsync<ItemNotFoundException>(async () =>
                await handler.WriteAsync(orderFacade with { Id = invalidId }));
            exception.Message.Should().Be($"Item 'Order' with ID {invalidId} was not found.");
        }

        [TearDown]
        public void TearDown()
        {
            context.Dispose();
        }
    }
}
