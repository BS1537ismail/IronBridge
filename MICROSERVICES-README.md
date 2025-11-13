# IronBridge Microservices Architecture

This project has been converted from a monolithic architecture to a microservices architecture with 4 independent services.

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                      Client Applications                      │
└───────────────┬─────────────────┬──────────────┬─────────────┘
                │                 │              │
        ┌───────▼────────┐ ┌──────▼──────┐ ┌────▼─────────┐
        │  UserAuth      │ │   Product    │ │   Booking    │
        │  Service       │ │   Service    │ │   Service    │
        │  Port: 5001    │ │  Port: 5002  │ │  Port: 5003  │
        └────────────────┘ └──────────────┘ └──────────────┘
                ▲                  ▲              │
                │                  │              │
                └──────────┬───────┴──────────────┘
                           │
                    ┌──────▼──────┐
                    │    Admin     │
                    │   Service    │
                    │  Port: 5004  │
                    └──────────────┘
```

## Services

### 1. UserAuth Service (Port: 5001)
**Responsibility**: User authentication, registration, and user management

**Database**: `UserAuthDb`

**Endpoints**:
- `POST /api/Auth/register` - Register new user
- `POST /api/Auth/login` - User login
- `GET /api/Auth/user/{userId}` - Get user by ID
- `GET /api/Auth/user/email/{email}` - Get user by email

**Technologies**:
- JWT Authentication
- BCrypt for password hashing
- Entity Framework Core
- SQL Server

### 2. Product Service (Port: 5002)
**Responsibility**: Product catalog management

**Database**: `ProductDb`

**Endpoints**:
- `GET /api/Product` - Get all products
- `GET /api/Product/active` - Get active products
- `GET /api/Product/{id}` - Get product by ID
- `GET /api/Product/category/{category}` - Get products by category
- `POST /api/Product` - Create new product
- `PUT /api/Product/{id}` - Update product
- `DELETE /api/Product/{id}` - Soft delete product
- `PATCH /api/Product/{id}/stock?quantity={qty}` - Update stock

**Technologies**:
- Entity Framework Core
- SQL Server

### 3. Booking Service (Port: 5003)
**Responsibility**: Handle product bookings and payments

**Database**: `BookingDb`

**Endpoints**:
- `POST /api/Booking` - Create new booking
- `GET /api/Booking/{id}` - Get booking by ID
- `GET /api/Booking/user/{userId}` - Get user bookings
- `PUT /api/Booking/{id}/status` - Update booking/payment status

**Service Communication**:
- Calls Product Service to verify product availability and update stock

**Technologies**:
- Entity Framework Core
- SQL Server
- HttpClient for inter-service communication

### 4. Admin Service (Port: 5004)
**Responsibility**: Admin operations for product management

**Endpoints**:
- `GET /api/AdminProduct` - Get all products
- `GET /api/AdminProduct/{id}` - Get product by ID
- `POST /api/AdminProduct` - Create product (Admin only)
- `PUT /api/AdminProduct/{id}?adminId={guid}` - Update product (Admin only)
- `DELETE /api/AdminProduct/{id}?adminId={guid}` - Delete product (Admin only)

**Service Communication**:
- Calls UserAuth Service to verify admin role
- Calls Product Service for product operations

**Technologies**:
- HttpClient for inter-service communication

## Data Models

### UserAuth Service - Users Table
```
Id              GUID            Primary key
FullName        string          User full name
Email           string          Unique, used for login
PasswordHash    string          Hashed password
PhoneNumber     string?         Optional
Role            enum            User, Admin
CreatedAt       DateTime        Registration time
UpdatedAt       DateTime        Last update time
IsActive        bool            Account status
```

### Product Service - Products Table
```
Id              int             Primary key
ProductName     string          Product title
Description     string          Product details
Price           decimal(18,2)   Product price
Stock           int             Available quantity
Category        string          Product category
ImageUrl        string?         Product image
CreatedBy       Guid            Reference to Admin (UserAuth.Id)
CreatedAt       DateTime        Added date
UpdatedAt       DateTime        Modified date
IsActive        bool            Availability status
```

### Booking Service - Bookings Table
```
Id              int             Primary key
UserId          Guid            From UserAuth
ProductId       int             From Product Service
Quantity        int             Number of products booked
TotalAmount     decimal         Total = Quantity × Product.Price
BookingStatus   enum            Pending, Confirmed, Cancelled
PaymentStatus   enum            Pending, Paid, Failed
TransactionId   string?         From payment gateway
PaymentMethod   string          SSLCommerz, Bkash, etc.
BookingDate     DateTime        When booked
UpdatedAt       DateTime        Last status change
```

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server (SQLEXPRESS)
- Docker (optional, for containerized deployment)

### Running Services Locally

1. **Update Connection Strings** (if needed)
   Each service has its own `appsettings.json` with connection strings pointing to SQL Server.

2. **Create Databases**
   Run migrations for each service:
   ```bash
   # UserAuth Service
   cd Services/UserAuth.Service/UserAuth.Service
   dotnet ef migrations add InitialCreate
   dotnet ef database update

   # Product Service
   cd Services/Product.Service/Product.Service
   dotnet ef migrations add InitialCreate
   dotnet ef database update

   # Booking Service
   cd Services/Booking.Service/Booking.Service
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

3. **Run Services**
   Open 4 separate terminals and run:
   ```bash
   # Terminal 1 - UserAuth Service
   cd Services/UserAuth.Service/UserAuth.Service
   dotnet run

   # Terminal 2 - Product Service
   cd Services/Product.Service/Product.Service
   dotnet run

   # Terminal 3 - Booking Service
   cd Services/Booking.Service/Booking.Service
   dotnet run

   # Terminal 4 - Admin Service
   cd Services/Admin.Service/Admin.Service
   dotnet run
   ```

4. **Access Swagger UI**
   - UserAuth: http://localhost:5001/swagger
   - Product: http://localhost:5002/swagger
   - Booking: http://localhost:5003/swagger
   - Admin: http://localhost:5004/swagger

### Running with Docker Compose

```bash
# Build and run all services
docker-compose -f docker-compose.microservices.yml up --build

# Stop all services
docker-compose -f docker-compose.microservices.yml down
```

## Service Communication

Services communicate via **HTTP REST APIs** using `HttpClient`:

### Booking → Product
- `GET /api/Product/{id}` - Verify product availability
- `PATCH /api/Product/{id}/stock` - Update product stock after booking

### Admin → UserAuth
- `GET /api/Auth/user/{userId}` - Verify admin role

### Admin → Product
- All product CRUD operations

## Configuration

### Service URLs
Update these in `appsettings.json` if service ports change:

**Booking Service:**
```json
"Services": {
  "ProductService": "http://localhost:5002"
}
```

**Admin Service:**
```json
"Services": {
  "UserAuthService": "http://localhost:5001",
  "ProductService": "http://localhost:5002"
}
```

### JWT Configuration (UserAuth Service)
```json
"Jwt": {
  "Key": "YourSuperSecretKeyForJWTTokenGenerationMustBe32CharsLong!",
  "Issuer": "IronBridge.UserAuth",
  "Audience": "IronBridge.Services"
}
```

**⚠️ IMPORTANT**: Change the JWT key in production!

## Example API Workflows

### 1. Register Admin User
```http
POST http://localhost:5001/api/Auth/register
Content-Type: application/json

{
  "fullName": "Admin User",
  "email": "admin@ironbridge.com",
  "password": "Admin@123",
  "phoneNumber": "1234567890",
  "role": "Admin"
}
```

### 2. Create Product (via Admin Service)
```http
POST http://localhost:5004/api/AdminProduct
Content-Type: application/json

{
  "productName": "Laptop",
  "description": "High-performance laptop",
  "price": 1200.00,
  "stock": 50,
  "category": "Electronics",
  "imageUrl": "https://example.com/laptop.jpg",
  "createdBy": "{admin-user-id-from-step-1}"
}
```

### 3. Register Customer
```http
POST http://localhost:5001/api/Auth/register
Content-Type: application/json

{
  "fullName": "John Doe",
  "email": "john@example.com",
  "password": "Pass@123",
  "phoneNumber": "0987654321",
  "role": "User"
}
```

### 4. Create Booking
```http
POST http://localhost:5003/api/Booking
Content-Type: application/json

{
  "userId": "{user-id-from-step-3}",
  "productId": 1,
  "quantity": 2,
  "paymentMethod": "SSLCommerz"
}
```

### 5. Update Booking Payment Status
```http
PUT http://localhost:5003/api/Booking/1/status
Content-Type: application/json

{
  "bookingStatus": "Confirmed",
  "paymentStatus": "Paid",
  "transactionId": "TXN123456789"
}
```

## Project Structure
```
IronBridge/
├── Services/
│   ├── UserAuth.Service/
│   │   ├── Controllers/
│   │   ├── Data/
│   │   ├── Models/
│   │   ├── Services/
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   └── Dockerfile
│   ├── Product.Service/
│   │   ├── Controllers/
│   │   ├── Data/
│   │   ├── Models/
│   │   ├── Services/
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   └── Dockerfile
│   ├── Booking.Service/
│   │   ├── Controllers/
│   │   ├── Data/
│   │   ├── Models/
│   │   ├── Services/
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   └── Dockerfile
│   └── Admin.Service/
│       ├── Controllers/
│       ├── Services/
│       ├── Program.cs
│       ├── appsettings.json
│       └── Dockerfile
├── Shared/
│   └── IronBridge.Shared/
│       ├── DTOs/
│       └── Enums/
├── docker-compose.microservices.yml
├── IronBridge.Microservices.sln
└── MICROSERVICES-README.md
```

## Future Enhancements

1. **API Gateway**: Add an API Gateway (e.g., Ocelot, YARP) for unified entry point
2. **Service Discovery**: Implement Consul or Eureka for dynamic service discovery
3. **Message Queue**: Replace HTTP calls with RabbitMQ/Azure Service Bus for async communication
4. **Distributed Caching**: Add Redis for cross-service caching
5. **Logging**: Implement centralized logging (ELK Stack, Seq)
6. **Monitoring**: Add Application Insights or Prometheus/Grafana
7. **Authentication**: Implement IdentityServer or Auth0 for centralized authentication
8. **Payment Gateway**: Integrate real SSLCommerz/Stripe payment APIs
9. **CQRS**: Separate read/write models for better performance
10. **Circuit Breaker**: Add Polly for resilience

## Notes

- All services are independent and can be deployed separately
- Each service has its own database (Database per Service pattern)
- Services communicate synchronously via HTTP (can be replaced with message queues)
- Admin service acts as a gateway for admin operations
- Booking service automatically updates product stock when creating bookings
- JWT tokens from UserAuth service can be used across all services (with shared secret key)

## License

This project is for educational/demonstration purposes.
