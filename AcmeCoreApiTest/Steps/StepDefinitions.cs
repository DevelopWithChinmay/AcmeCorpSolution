using AcmeCorpApi.Controllers;
using AcmeCorpBusiness.Domain.Contacts;
using AcmeCorpBusiness.Domain.Customers;
using AcmeCorpBusiness.Domain.Orders;
using AcmeCorpBusiness.Entities;
using AcmeCorpBusiness.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechTalk.SpecFlow;

namespace AcmeCoreApiTest.Steps
{
    [Binding]
    public class StepDefinitions
    {
        private AcmeCorpDbContext? _context;
        
        private CustomersController? _customerController;
        private CustomerOrdersController? _customerOrdersController;
        private OrdersController? _ordersController;

        private CustomerReadHandler? _customerReadHandler;
        private CustomerWriteHandler? _customerWriteHandler;
        private OrderReadHandler? _orderReadHandler;
        private OrderWriteHandler? _orderWriteHandler;

        private List<CustomerFacade>? _customers;
        private List<OrderFacade>? _orders; 
        
        private CustomerFacade? _newCustomer;
        private CustomerFacade? _retrievedCustomer;
        
        private OrderFacade? _newOrder;
        private OrderFacade? _retrievedOrder;

        [BeforeScenario]
        public void Setup()
        {
            MappingHelper.Configure();
            var databaseName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<AcmeCorpDbContext>()
                .UseInMemoryDatabase(databaseName: $"AcmeCorpDb_{databaseName}")
                .Options;

            _context = new AcmeCorpDbContext(options);

            _orderReadHandler = new OrderReadHandler(_context);
            _orderWriteHandler = new OrderWriteHandler(_context);

            _customerReadHandler = new CustomerReadHandler(_context);
            _customerWriteHandler = new CustomerWriteHandler(_context);
            _customerController = new CustomersController();
            _customerOrdersController = new CustomerOrdersController();
            _ordersController = new OrdersController(_orderWriteHandler);
            
            _customers = [];
            _orders = [];
        }

        [Given(@"the database is empty")]
        public void GivenTheDatabaseIsEmpty()
        {
            // The database is empty due to the setup
        }

        [When(@"I add an order with the following details")]
        public async Task WhenIAddAnOrderWithTheFollowingDetails(Table table)
        {
            var row = table.Rows[0];
            _newOrder = new OrderFacade
            {
                CustomerId = int.Parse(row["CustomerId"]),
                ProductName = row["ProductName"],
                TotalAmount = int.Parse(row["TotalAmount"])
            };

            await _ordersController!.AddOrder(_newOrder);
        }

        [Then(@"the order should be added successfully")]
        public void ThenTheOrderShouldBeAddedSuccessfully()
        {
            var orderInDb = _context!.Orders.SingleOrDefault(o => o.ProductName == _newOrder!.ProductName);
            Assert.That(orderInDb, Is.Not.Null);
        }

        [Then(@"I should see the order in the database")]
        public void ThenIShouldSeeTheOrderInTheDatabase()
        {
            var orderInDb = _context!.Orders.SingleOrDefault(o => o.ProductName == _newOrder!.ProductName);
            Assert.That(orderInDb, Is.Not.Null);
        }

        [Given(@"an order exists with the following details")]
        public async Task GivenAnOrderExistsWithTheFollowingDetails(Table table)
        {
            var row = table.Rows[0];
            var order = new Order
            {
                Id = int.Parse(row["Id"]),
                CustomerId = int.Parse(row["CustomerId"]),
                ProductName = row["ProductName"],
                TotalAmount = int.Parse(row["TotalAmount"])
            };

            _context!.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        [When(@"I retrieve the order by ID")]
        public async Task WhenIRetrieveTheOrderByID(Table table)
        {
            var id = int.Parse(table.Rows[0]["Id"]);
            var result = await _ordersController!.GetOrder(_orderReadHandler!, id);
            _retrievedOrder = result.Value;
        }

        [Then(@"I should see the order details")]
        public void ThenIShouldSeeTheOrderDetails(Table table)
        {
            var row = table.Rows[0];
            Assert.Multiple(() =>
            {
                Assert.That(_retrievedOrder!.CustomerId, Is.EqualTo(int.Parse(row["CustomerId"])));
                Assert.That(_retrievedOrder!.ProductName, Is.EqualTo(row["ProductName"]));
                Assert.That(_retrievedOrder.TotalAmount, Is.EqualTo(double.Parse(row["TotalAmount"])));
            });
        }

        [When(@"I update the order with the following details")]
        public async Task WhenIUpdateTheOrderWithTheFollowingDetails(Table table)
        {
            var row = table.Rows[0];
            var updatedOrder = new OrderFacade
            {
                Id = int.Parse(row["Id"]),
                CustomerId = int.Parse(row["CustomerId"]),
                ProductName = row["ProductName"],
                TotalAmount = double.Parse(row["TotalAmount"])
            };

            _retrievedOrder = (await _ordersController!.UpdateOrder(updatedOrder)).Value;
        }

        [Then(@"the order should be updated successfully")]
        public void ThenTheOrderShouldBeUpdatedSuccessfully()
        {
            Assert.IsNotNull(_retrievedOrder);
        }

        [Then(@"I should see the updated order details")]
        public void ThenIShouldSeeTheUpdatedOrderDetails(Table table)
        {
            var row = table.Rows[0];
            var orderInDb = _context!.Orders.Single(o => o.Id == _retrievedOrder!.Id);
            Assert.Multiple(() =>
            {
                Assert.That(orderInDb.CustomerId, Is.EqualTo(int.Parse(row["CustomerId"])));
                Assert.That(orderInDb.ProductName, Is.EqualTo(row["ProductName"]));
                Assert.That(orderInDb.TotalAmount, Is.EqualTo(double.Parse(row["TotalAmount"])));
            });
        }

        [When(@"I retrieve orders by CustomerID")]
        public async Task WhenIRetrieveOrdersByCustomerID(Table table)
        {
            var customerId = int.Parse(table.Rows[0]["CustomerId"]);
            var result = await _customerOrdersController!.GetOrdersAsync(_orderReadHandler!, customerId);
            if (result.Result is OkObjectResult okResult)
            {
                _orders = (okResult.Value as IEnumerable<OrderFacade>)!.ToList();
            }
        }

        [Then(@"I should see the order list")]
        public void ThenIShouldSeeTheOrderList(Table table)
        {
            var row = table.Rows[0];
            Assert.Multiple(() =>
            {
                Assert.That(_orders![0]!.CustomerId, Is.EqualTo(int.Parse(row["CustomerId"])));
                Assert.That(_orders![0]!.ProductName, Is.EqualTo(row["ProductName"]));
                Assert.That(_orders![0]!.TotalAmount, Is.EqualTo(double.Parse(row["TotalAmount"])));
            });
        }

        [When(@"I retrieve all customers")]
        public async Task WhenIRetrieveAllCustomers()
        {
            var result = await _customerController!.GetCustomers(_customerReadHandler!);
            _customers = result.ToList();
        }

        [Then(@"I should receive an empty list")]
        public void ThenIShouldReceiveAnEmptyList()
        {
            Assert.IsEmpty(_customers!);
        }

        [When(@"I add a customer with the following details")]
        public async Task WhenIAddACustomerWithTheFollowingDetails(Table table)
        {
            var row = table.Rows[0];
            _newCustomer = new CustomerFacade
            {
                Name = row["Name"],
                ContactDetail = new ContactDetailFacade
                {
                    Email = row["Email"],
                    PhoneNumber = row["PhoneNumber"]
                }
            };

            await _customerController!.AddCustomer(_customerWriteHandler!, _newCustomer);
        }

        [Then(@"the customer should be added successfully")]
        public void ThenTheCustomerShouldBeAddedSuccessfully()
        {
            var customerInDb = _context!.Customers.SingleOrDefault(c => c.Email == _newCustomer!.ContactDetail.Email);
            Assert.IsNotNull(customerInDb);
        }

        [Then(@"I should see the customer in the database")]
        public void ThenIShouldSeeTheCustomerInTheDatabase()
        {
            var customerInDb = _context!.Customers.SingleOrDefault(c => c.Email == _newCustomer!.ContactDetail.Email);
            Assert.IsNotNull(customerInDb);
        }

        [Given(@"a customer exists with the following details")]
        public async Task GivenACustomerExistsWithTheFollowingDetails(Table table)
        {
            var row = table.Rows[0];
            var customer = new Customer
            {
                Id = int.Parse(row["Id"]),
                Name = row["Name"],
                Email = row["Email"],
                PhoneNumber = row["PhoneNumber"]
            };

            _context!.Customers.Add(customer);
            await _context.SaveChangesAsync();
        }

        [When(@"I retrieve the customer by ID")]
        public async Task WhenIRetrieveTheCustomerByID(Table table)
        {
            var id = int.Parse(table.Rows[0]["Id"]);
            var result = await _customerController!.GetCustomer(_customerReadHandler!, id);
            _retrievedCustomer = result.Value;
        }

        [Then(@"I should see the customer details")]
        public void ThenIShouldSeeTheCustomerDetails(Table table)
        {
            var row = table.Rows[0];
            Assert.Multiple(() =>
            {
                Assert.That(_retrievedCustomer!.Name, Is.EqualTo(row["Name"]));
                Assert.That(_retrievedCustomer.ContactDetail.Email, Is.EqualTo(row["Email"]));
                Assert.That(_retrievedCustomer.ContactDetail.PhoneNumber, Is.EqualTo(row["PhoneNumber"]));
            });
        }

        [When(@"I update the customer with the following details")]
        public async Task WhenIUpdateTheCustomerWithTheFollowingDetails(Table table)
        {
            var row = table.Rows[0];
            var updatedCustomer = new CustomerFacade
            {
                Id = int.Parse(row["Id"]),
                Name = row["Name"],
                ContactDetail = new ContactDetailFacade
                {
                    Email = row["Email"],
                    PhoneNumber = row["PhoneNumber"]
                }
            };

            _retrievedCustomer = (await _customerController!.UpdateCustomer(_customerWriteHandler!, updatedCustomer))?.Value;
        }

        [Then(@"the customer should be updated successfully")]
        public void ThenTheCustomerShouldBeUpdatedSuccessfully()
        {
            Assert.IsNotNull(_retrievedCustomer);
        }

        [Then(@"I should see the updated customer details")]
        public void ThenIShouldSeeTheUpdatedCustomerDetails(Table table)
        {
            var row = table.Rows[0];
            var customerInDb = _context!.Customers.Single(c => c.Id == _retrievedCustomer!.Id);
            Assert.Multiple(() =>
            {
                Assert.That(customerInDb.Name, Is.EqualTo(row["Name"]));
                Assert.That(customerInDb.Email, Is.EqualTo(row["Email"]));
                Assert.That(customerInDb.PhoneNumber, Is.EqualTo(row["PhoneNumber"]));
            });
        }
    }
}
