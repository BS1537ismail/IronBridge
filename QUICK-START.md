# IronBridge Microservices - Quick Start Guide

## ✅ Current Status
Your microservices architecture is **ready to use!** All services have been created and the solution builds successfully.

## 📁 Project Structure
```
IronBridge/
├── Services/
│   ├── UserAuth.Service/    → Port 5001 (Authentication & Users)
│   ├── Product.Service/      → Port 5002 (Product Catalog)
│   ├── Booking.Service/      → Port 5003 (Bookings & Payments)
│   └── Admin.Service/        → Port 5004 (Admin Operations)
├── Shared/
│   └── IronBridge.Shared/    → Common DTOs & Enums
└── IronBridge.Microservices.sln
```

## 🚀 Getting Started

### Step 1: Create Databases

Run these commands in **3 separate terminals** (one for each database):

**Terminal 1 - UserAuth Database:**
```bash
cd "Services/UserAuth.Service"
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**Terminal 2 - Product Database:**
```bash
cd "Services/Product.Service"
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**Terminal 3 - Booking Database:**
```bash
cd "Services/Booking.Service"
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Step 2: Run All Services

Open **4 separate terminals** and run each service:

**Terminal 1 - UserAuth Service:**
```bash
cd "Services/UserAuth.Service"
dotnet run
```

**Terminal 2 - Product Service:**
```bash
cd "Services/Product.Service"
dotnet run
```

**Terminal 3 - Booking Service:**
```bash
cd "Services/Booking.Service"
dotnet run
```

**Terminal 4 - Admin Service:**
```bash
cd "Services/Admin.Service"
dotnet run
```

### Step 3: Access Swagger UI

Once all services are running, open these URLs in your browser:

- **UserAuth Service:** http://localhost:5001/swagger
- **Product Service:** http://localhost:5002/swagger
- **Booking Service:** http://localhost:5003/swagger
- **Admin Service:** http://localhost:5004/swagger

## 🔥 Quick Test Workflow

### 1. Register an Admin User
```
POST http://localhost:5001/api/Auth/register
{
  "fullName": "Admin User",
  "email": "admin@test.com",
  "password": "Admin@123",
  "phoneNumber": "1234567890",
  "role": "Admin"
}
```
**Save the returned `userId`!**

### 2. Create a Product (via Admin Service)
```
POST http://localhost:5004/api/AdminProduct
{
  "productName": "Laptop",
  "description": "High-performance laptop",
  "price": 1200.00,
  "stock": 50,
  "category": "Electronics",
  "imageUrl": "https://example.com/laptop.jpg",
  "createdBy": "{userId-from-step-1}"
}
```

### 3. Register a Regular User
```
POST http://localhost:5001/api/Auth/register
{
  "fullName": "John Doe",
  "email": "john@test.com",
  "password": "User@123",
  "phoneNumber": "0987654321",
  "role": "User"
}
```
**Save the returned `userId`!**

### 4. Create a Booking
```
POST http://localhost:5003/api/Booking
{
  "userId": "{userId-from-step-3}",
  "productId": 1,
  "quantity": 2,
  "paymentMethod": "SSLCommerz"
}
```

### 5. Check Product Stock
```
GET http://localhost:5002/api/Product/1
```
You should see the stock reduced by 2!

## 📊 Service Communication

- **Booking → Product**: Automatically checks stock and updates it
- **Admin → UserAuth**: Verifies admin role before allowing product operations
- **Admin → Product**: Performs all product CRUD operations

## 🛠️ Troubleshooting

### "Cannot connect to database"
- Make sure SQL Server is running
- Check connection strings in `appsettings.json` files
- Your server name is: `BS-01343\SQLEXPRESS`

### "Port already in use"
- Make sure no other services are using ports 5001-5004
- Check `Properties/launchSettings.json` in each service

### "Build failed"
- Run `dotnet clean` and then `dotnet build IronBridge.Microservices.sln`

## 📚 Full Documentation

For complete documentation, API endpoints, and advanced features, see:
- **MICROSERVICES-README.md** - Complete architecture and API documentation

## 🎯 Next Steps

1. ✅ **Create databases** (Step 1 above)
2. ✅ **Run all services** (Step 2 above)
3. ✅ **Test the APIs** using Swagger UI
4. 📖 Read the full **MICROSERVICES-README.md** for advanced features
5. 🔐 **Change JWT secret key** in `Services/UserAuth.Service/appsettings.json`

## 🐳 Docker Deployment (Optional)

To run all services using Docker:

```bash
docker-compose -f docker-compose.microservices.yml up --build
```

Stop all services:
```bash
docker-compose -f docker-compose.microservices.yml down
```

---

**Happy Coding! 🚀**

For issues or questions, check the **MICROSERVICES-README.md** file.
