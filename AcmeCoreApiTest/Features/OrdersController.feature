Feature: Order Management
  In order to manage orders
  As an administrator
  I want to create, retrieve, and update orders

  Scenario: Add a new order
    Given a customer exists with the following details
      | Id | Name       | Email              | PhoneNumber   |
      | 1  | Jane Doe   | jane@example.com   | 0987654321    |
    When I add an order with the following details
      | CustomerId | ProductName    | TotalAmount |
      | 1          | Widget A       | 5        |
    Then the order should be added successfully
    And I should see the order in the database

  Scenario: Retrieve an order by ID
    Given an order exists with the following details
      | Id | CustomerId | ProductName | TotalAmount |
      | 1  | 1          | Widget A    | 5        |
    When I retrieve the order by ID
      | Id |
      | 1  |
    Then I should see the order details
      | CustomerId | ProductName | TotalAmount |
      | 1          | Widget A    | 5        |

  Scenario: Update an existing order
    Given an order exists with the following details
      | Id | CustomerId | ProductName | TotalAmount |
      | 1  | 1          | Widget A    | 5        |
    When I update the order with the following details
      | Id | CustomerId | ProductName   | TotalAmount |
      | 1  | 1          | Widget B      | 10       |
    Then the order should be updated successfully
    And I should see the updated order details
      | CustomerId | ProductName | TotalAmount |
      | 1          | Widget B    | 10       |