Feature: Customer Order Management

Scenario: Retrieve orders by Customer ID
	Given a customer exists with the following details
      | Id | Name       | Email              | PhoneNumber   |
      | 1  | Jane Doe   | jane@example.com   | 0987654321    |
    Given an order exists with the following details
      | Id | CustomerId | ProductName | TotalAmount |
      | 1  | 1          | Widget A    | 5        |
    When I retrieve orders by CustomerID
      | CustomerId |
      | 1  |
    Then I should see the order list
      | CustomerId | ProductName | TotalAmount |
      | 1          | Widget A    | 5        |