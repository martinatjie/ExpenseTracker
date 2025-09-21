# ExpenseTracker Solution

## Overview

ExpenseTracker is a modular solution for managing, processing, and visualizing expense data. It consists of several projects, each with a distinct responsibility, making the system maintainable and extensible.

## Projects

- **ExpenseProcessing**  
  Contains core business logic for expense management, including downloading, transforming, categorizing, caching, exporting, and importing expense data.

- **ExpenseAgent**  
  Implements background worker services (using `BackgroundService`) to automate or schedule expense processing tasks.

- **ExpenseTracker**  
  Provides the cross-platform user interface using .NET MAUI, allowing users to view and interact with their expense data.

- **Common**  
  Houses shared abstractions and interfaces (e.g., `IDownloadService`) used across multiple projects.

- **Tests**  
  Contains unit and integration tests to ensure reliability and correctness of business logic and services.

## Key Services

- **BankDownloadService / EmailDownloadService**  
  Download expense data from banks or emails, implementing the `IDownloadService` interface.

- **CachingService**  
  Manages temporary storage to optimize performance and reduce redundant processing.

- **CategoryService**  
  Classifies transactions into categories (e.g., groceries, utilities).

- **RuleService**  
  Applies business rules to transactions for categorization or validation.

- **TransactionService**  
  Handles CRUD operations and business logic for expense transactions.

- **TransformationService**  
  Transforms raw downloaded data into a normalized format for further processing.

- **ExportService**  
  Exports processed expense data to files (CSV, Excel) or external systems.

- **ImportService** 
  Handles logic for manual file uploads, importing transactions from user-provided files.

- **VisualizationService**  
  Provides data visualization features, such as charts and reports, for expense tracking.

- **Worker**  
  Automates and schedules expense processing tasks as a background service.

## Getting Started

1. Clone the repository.
2. Build the solution using Visual Studio 2022.
3. Configure your database (PostgreSQL recommended).
4. Run the worker service and MAUI app as needed.

## Contributing

Contributions are welcome! Please submit issues or pull requests via GitHub.

## License

This project is licensed under the GNU 3 License.