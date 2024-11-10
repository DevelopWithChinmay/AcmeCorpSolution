Feature: Customer Management

Scenario: Retrieve all customers
    Given the database is empty
    When I retrieve all customers
    Then I should receive an empty list

Scenario: Add a new customer
    Given the database is empty
    When I add a customer with the following details
      | Name       | Email              | PhoneNumber   |
      | John Doe   | john@example.com   | 1234567890    |
    Then the customer should be added successfully
    And I should see the customer in the database

  Scenario: Retrieve a customer by ID
    Given a customer exists with the following details
      | Id | Name       | Email              | PhoneNumber   |
      | 1  | Jane Doe   | jane@example.com   | 0987654321    |
    When I retrieve the customer by ID
      | Id |
      | 1  |
    Then I should see the customer details
      | Name     | Email            | PhoneNumber   |
      | Jane Doe | jane@example.com | 0987654321    |

  Scenario: Update an existing customer
    Given a customer exists with the following details
      | Id | Name     | Email            | PhoneNumber   |
      | 1  | John Doe | john@example.com | 1234567890    |
    When I update the customer with the following details
      | Id | Name       | Email                | PhoneNumber   |
      | 1  | John Smith | john.smith@example.com | 9876543210 |
    Then the customer should be updated successfully
    And I should see the updated customer details
      | Name      | Email                | PhoneNumber   |
      | John Smith | john.smith@example.com | 9876543210 |