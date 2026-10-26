# Lab 06: Entity Framework Core + Blazor + Supabase 
**Topic:** Build a working Product Management application with PostgreSQL using EF Core & Blazor  

---

## 1. Project Overview & Architecture
This solution demonstrates a clean, layered architecture implementing CRUD operations using **Entity Framework Core** with **Supabase PostgreSQL** in an interactive **Blazor Web App**:
- **Models**: Defines the `Product` entity with data annotations (`[Required]`, `[Range]`, `[StringLength]`).
- **DataAccessLayer**: Contains `AppDbContext` configuring entity mappings to snake_case PostgreSQL columns and table constraints.
- **BusinessLogicLayer**: Contains `IProductService` and `ProductService` utilizing `IDbContextFactory<AppDbContext>` for thread-safe, short-lived database context management.
- **UserInterface**: Blazor Web App with interactive server-side rendering for product management (`/products`), including client validation, error notifications, add/edit/delete operations, and responsive UI.

---

## 2. Setup & Configuration

### Prerequisites
- .NET 10 SDK (or compatible .NET SDK)
- A Supabase project with PostgreSQL

### Database Table Creation
Execute the following DDL in your Supabase SQL Editor:
```sql
CREATE TABLE IF NOT EXISTS public.products (
    product_id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    name varchar(100) NOT NULL,
    description varchar(500),
    price numeric(10,2) NOT NULL CHECK (price >= 0),
    stock_quantity integer NOT NULL DEFAULT 0 CHECK (stock_quantity >= 0),
    created_at timestamptz NOT NULL DEFAULT now()
);
```

### Storing Connection String Securely (.NET User Secrets)
Never commit database passwords to source control. Configure your connection string using .NET User Secrets:

```bash
cd UserInterface
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:Supabase" "Host=YOUR_HOST;Port=YOUR_PORT;Database=postgres;Username=YOUR_USER;Password=YOUR_PASSWORD;SSL Mode=Require;Trust Server Certificate=true"
```
> **Note:** The application looks for the connection string under the configuration key `ConnectionStrings:Supabase`.

### Running the Application
```bash
dotnet build
dotnet run --project UserInterface/UserInterface.csproj
```
Open your browser and navigate to `http://localhost:5059` or `http://localhost:5059/products`.

---

## 3. Technology Explanations (4–6 Lines Each)

### Blazor
Blazor is a modern web framework from Microsoft that enables developers to build interactive client-side web user interfaces using C# and HTML instead of JavaScript. In Interactive Server mode, UI events are processed on the server over a real-time SignalR WebSocket connection, with only lightweight UI diffs sent back to the browser. It integrates seamlessly with ASP.NET Core dependency injection, routing, forms, and validation. This streamlines full-stack .NET development by sharing models and business logic across front-end and back-end.

### Entity Framework Core (EF Core)
Entity Framework Core is a lightweight, cross-platform Object-Relational Mapper (ORM) for .NET. It abstracts raw relational database operations by allowing developers to work with database tables as strongly-typed C# objects (`DbSet<T>`). EF Core translates LINQ queries into optimized SQL, tracks changes to entities, and automates schema migrations. By eliminating boilerplate ADO.NET and raw SQL string manipulation, it reduces SQL injection risks and drastically accelerates development velocity.

### Npgsql / Npgsql.EntityFrameworkCore.PostgreSQL
Npgsql is the open-source, high-performance ADO.NET data provider that enables .NET applications to connect to and communicate with PostgreSQL databases. The `Npgsql.EntityFrameworkCore.PostgreSQL` package is the official EF Core database provider built on top of Npgsql. It enables EF Core to translate LINQ queries and model configurations into PostgreSQL-specific SQL syntax, types, and functions (such as PostgreSQL `uuid`, `timestamptz`, and `numeric`). Without this provider, EF Core cannot understand or interact with PostgreSQL dialects.

### Supabase
Supabase is an open-source Firebase alternative that provides a fully managed PostgreSQL database in the cloud along with real-time subscriptions, authentication, and instant RESTful APIs. For this lab, Supabase acts as the remote hosted PostgreSQL database server. It exposes a standard PostgreSQL connection endpoint accessible over SSL and session connection poolers, and offers a web-based dashboard and SQL Editor to manage tables, run queries, and inspect stored data in real-time.

---

## 4. Assessment Viva / Reflection Questions & Answers

### 17. What problem does EF Core solve compared with writing SQL manually?
EF Core eliminates manual ADO.NET boilerplate (opening connections, building commands, manually mapping `IDataReader` columns to class properties, managing parameters). It provides compile-time type checking, prevents SQL injection vulnerabilities through automatic parameterization, and simplifies complex queries via LINQ. Additionally, change tracking automates updates and deletes without manual SQL formulation.

### 18. What is the purpose of DbContext and DbSet<Product>?
- **`DbContext`**: Represents a session with the database. It coordinates querying, tracks entity state changes, and persists changes back to the database using `SaveChanges` / `SaveChangesAsync`.
- **`DbSet<Product>`**: Represents an in-memory collection of `Product` entities that maps directly to the `products` database table, enabling LINQ queries and CRUD operations.

### 19. Why does the project need the Npgsql EF Core provider?
EF Core is provider-agnostic and relies on database-specific providers to generate valid SQL. `Npgsql.EntityFrameworkCore.PostgreSQL` teaches EF Core the PostgreSQL dialect, translates LINQ queries into PostgreSQL SQL, and correctly handles PostgreSQL data types such as `uuid`, `timestamptz`, and `numeric(10,2)`.

### 20. What is the difference between a C# entity and a PostgreSQL table?
- A **C# entity** is an in-memory object-oriented class instance residing in application memory, containing properties, methods, and C# type definitions (`Guid`, `decimal`, `DateTimeOffset`).
- A **PostgreSQL table** is a persistent relational structure stored on disk in the database engine, organized as rows and columns with database-level types (`uuid`, `numeric(10,2)`, `timestamptz`), constraints (foreign keys, check constraints, default values), and indexes.

### 21. Why should a database connection string not be placed in a Blazor component?
Placing connection strings inside Blazor components breaks separation of concerns, violates architectural layering, and introduces grave security risks (exposing credentials to client-side code, Git version control, or reverse engineering). Connection strings must remain server-side and be managed securely via configuration mechanisms like .NET User Secrets or environment variables.

### 22. Why is a short-lived DbContext important? Why use a factory in this Blazor lab?
`DbContext` is not thread-safe and is designed as a short-lived unit of work. In Blazor Interactive Server applications, components are long-lived and multiple asynchronous operations or events can execute concurrently. Using `IDbContextFactory<AppDbContext>` allows each asynchronous method (e.g., `GetAllAsync`, `SaveAsync`) to create, use, and dispose of its own isolated, short-lived `DbContext` (`await using var db = await _factory.CreateDbContextAsync()`), avoiding concurrency collisions and memory leaks.

### 23. What does SaveChangesAsync do?
`SaveChangesAsync` examines all tracked entities within the `DbContext` change tracker, determines which entities have been added, modified, or deleted, generates the corresponding SQL `INSERT`, `UPDATE`, or `DELETE` statements, executes them inside a transaction against PostgreSQL, and updates entity states (including populating auto-generated primary keys or default values).

### 24. Why use AsNoTracking for the product list?
`AsNoTracking()` tells EF Core not to store snapshot copies of retrieved entities in the change tracker cache. For read-only queries (like fetching product lists to display in a UI table), disabling change tracking significantly reduces CPU and memory overhead and speeds up query execution.

### 25. What is the difference between client-side validation and database constraints?
- **Client-side / UI validation** (e.g., `DataAnnotationsValidator` with `[Required]`, `[Range]`): Runs in the UI before data is sent, giving immediate, user-friendly feedback without network round-trips. However, it can be bypassed.
- **Database constraints** (e.g., `NOT NULL`, `CHECK (price >= 0)`, `PRIMARY KEY`): Enforced at the database engine level as the ultimate source of truth, guaranteeing data integrity regardless of which application, script, or API accesses the database.

### 26. If the database changes from Supabase PostgreSQL to another relational database, which parts of the application might need to change?
Only the data access configuration layer needs to be modified:
1. Replace `Npgsql.EntityFrameworkCore.PostgreSQL` with the target provider package (e.g., `Microsoft.EntityFrameworkCore.SqlServer` or `Pomelo.EntityFrameworkCore.MySql`).
2. Update the connection string format in configuration/secrets.
3. Replace `.UseNpgsql(connStr)` with `.UseSqlServer(connStr)` in `Program.cs`.
4. If database-specific SQL functions were configured in `OnModelCreating` (e.g., `gen_random_uuid()` or `now()`), adjust them to the target database equivalent (e.g., `NEWID()` and `GETDATE()` in SQL Server).  
Business logic, Blazor UI components, and entity models remain unaffected.
