# Interface Usage Demo - .NET 8.0

## Overview
This project demonstrates 5 different interface usage patterns in C# with comprehensive exception handling.

## Examples

### Example 1: Basic Repository Pattern
**File:** `Example1_BasicRepository.cs`
**Interface:** `IRepository`
- Demonstrates basic CRUD operations (Create, Read, Update, Delete)
- Shows exception handling for:
  - `KeyNotFoundException` when accessing non-existent items
  - `ArgumentException` for invalid input parameters
- Pattern: Repository pattern for data access abstraction

### Example 2: Multiple Interface Implementation
**File:** `Example2_MultipleInterfaces.cs`
**Interfaces:** `ILogger`, `IDisposable`, `IConfigurable`
- Shows a single class implementing multiple interfaces
- Demonstrates exception handling for:
  - `InvalidOperationException` for missing configuration
  - `ObjectDisposedException` for operations on disposed objects
  - `IOException` wrapped in custom exceptions
- Pattern: Multi-interface implementation with resource management

### Example 3: Event-Driven Interface
**File:** `Example3_EventDrivenInterface.cs`
**Interface:** `INotificationService`
- Implements event-driven architecture with EventHandler
- Shows exception handling for:
  - `FormatException` for invalid data format
  - `InvalidOperationException` for business rule violations
  - `ArgumentException` for null/empty parameters
- Pattern: Observer pattern with events and delegates

### Example 4: Generic Interface
**File:** `Example4_GenericInterface.cs`
**Interface:** `ICache<TKey, TValue>`
- Demonstrates generic type-safe interface
- Thread-safe implementation with lock
- Shows exception handling for:
  - `KeyNotFoundException` for cache misses
  - `InvalidOperationException` for expired entries
  - `ArgumentNullException` for null parameters
- Pattern: Generic cache with expiration support

### Example 5: Interface Segregation Principle (ISP)
**File:** `Example5_InterfaceSegregation.cs`
**Interfaces:** `IReadable`, `IWritable`, `IDeletable`, `ISearchable`
- Demonstrates SOLID principles (Interface Segregation)
- Multiple small, focused interfaces instead of one large interface
- Shows exception handling for:
  - `FileNotFoundException` for missing documents
  - `ArgumentException` for invalid parameters
  - `InvalidOperationException` for wrapped exceptions
- Pattern: Interface segregation with composition

## Running the Demo
```bash
dotnet run
```

## Key Concepts Demonstrated

1. **Interface Design Patterns**
   - Basic interfaces
   - Generic interfaces
   - Multiple interface implementation
   - Interface segregation

2. **Exception Handling**
   - Try-catch blocks at multiple levels
   - Specific exception types
   - Exception wrapping and rethrowing
   - Finally blocks for cleanup

3. **Best Practices**
   - Dependency injection friendly
   - Thread safety where needed
   - Resource disposal (IDisposable)
   - Event-driven architecture
   - SOLID principles

4. **C# Features**
   - Events and delegates
   - Generic constraints
   - Nullable reference types
   - Expression-bodied members
   - Pattern matching (where applicable)

## Architecture
- Framework: .NET 8.0
- Pattern: Clean separation of concerns
- Each example is self-contained in its own file
- Main program orchestrates all examples with global exception handling
