# E-Shop Web API (.NET 9)

A modern, high-performance e-commerce backend built with **.NET 9**. This project focuses on Clean Architecture, scalability, and deep database optimization using the latest features of the .NET ecosystem.


---

## Key Technical Features

### Architecture & Clean Code
* **Layered Architecture:** Strict separation of concerns between Presentation, Business Logic, and Data Access layers.
* **Clean Program.cs:** Utilizes **Extension Methods** for service configurations, keeping the entry point modular, readable, and easy to maintain.
* **RESTful Design:** Full compliance with REST standards, including proper HTTP methods, status codes, and resource-based routing.

### Performance Optimization
* **Advanced Data Access:** Optimized **EF Core** queries using `.AsNoTracking()` for read-heavy operations.
* **Distributed Caching Strategy:** Implemented abstract caching via the `IDistributedCache` interface. This architectural choice allows seamless switching between **MemoryCache** and **Redis** without modifying business logic.
* **Modern C# 13 Features:** Leveraged the latest language features for more efficient, expressive, and memory-safe code.

### Security & Access Control
* **JWT Authentication:** Secure, token-based authentication for stateless communication.
* **Role-Based Access Control (RBAC):** Fine-grained permission management for `Admin`, `Manager`, and `User` roles to protect sensitive endpoints.

---

## Tech Stack

| Technology | Purpose |
| :--- | :--- |
| **.NET 9** | Core Framework (ASP.NET Core) |
| **EF Core** | Object-Relational Mapper (ORM) |
| **PostgreSQL** | Primary Relational Database |
| **Distributed Cache** | High-speed Caching Layer (`IDistributedCache`) |
| **Docker & Compose** | Containerization and Infrastructure Orchestration |
| **Scalar / Swagger** | API Documentation and Interactive Testing |

---

## Quick Start

The project is fully containerized. You can spin up the entire infrastructure (API, Database, and Cache) with a single command.

1. **Clone the repository:**
   ```bash
   git clone [https://github.com/DorogaSmerti/ShopProjectProd.git](https://github.com/DorogaSmerti/ShopProjectProd.git)
2. **Spin up the infrastructure:**
   ```bash
   docker compose up -d --build
3. **Access API Documentation:**
  Open http://localhost:5000/scalar/v1 in your browser.

---

## Roadmap
[ ] Integration of FluentValidation for decoupled and robust request validation.

[ ] Migration to MediatR (CQRS) to further separate read and write operations.

[ ] CI/CD Pipeline implementation via GitHub Actions.

---

## Configuration

The project uses the standard .NET configuration provider hierarchy. 

**Note on Secrets:**
The `appsettings.json` file contains **placeholder values** (e.g., `USER`, `Password`, `SecretKey`). These are intended for demonstration purposes only.

To run the application with your own credentials:

1.  **Option A (Local):** Update the values directly in `appsettings.json`.
2.  **Option B (Recommended):** Use **.NET User Secrets** or set **Environment Variables** to override the default settings without modifying the source code.
3.  **Option C (Docker):** Provide the following variables via your `.env` file to be picked up by `docker-compose.yml`:
    * `DB_USER`: PostgreSQL username
    * `DB_PASSWORD`: PostgreSQL password
    * `NAME_DB`: Database name
    * `JWT_KEY`: Your secret JWT key (at least 32 characters)

**Required Configuration Keys:**
* `ConnectionStrings:DefaultConnection` (PostgreSQL format)
* `Jwt:Key` (Security key for token generation)