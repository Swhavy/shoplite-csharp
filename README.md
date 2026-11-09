# ShopLite API

A clean, minimal e-commerce Web API built with **.NET 8**, **ASP.NET Core Controllers**, **Entity Framework Core (SQLite)**, **Swagger/OpenAPI**, and **xUnit**.

## Features

- **Products**: List all products (with case-insensitive LINQ search via `?search=`), get product by ID, create new products, and automatic seeding of 5 sample products on startup.
- **Cart**: Add items to a customer's cart (`POST /api/cart/{customerId}/items`) with automatic stock validation, and view the cart (`GET /api/cart/{customerId}`) with per-item subtotals and computed cart total.
- **Checkout**: Convert a customer's cart into an order inside a database transaction (`POST /api/checkout/{customerId}`), deduct product stock quantities, and clear the cart.

## Project Structure

```text
ShopLite/
├── ShopLite.sln
├── Dockerfile
├── .dockerignore
├── .gitignore
├── README.md
├── ShopLite.Api/
│   ├── Controllers/
│   │   ├── ProductsController.cs
│   │   ├── CartController.cs
│   │   └── CheckoutController.cs
│   ├── Data/
│   │   ├── ShopLiteDbContext.cs
│   │   └── DbInitializer.cs
│   ├── DTOs/
│   │   ├── ProductDtos.cs
│   │   ├── CartDtos.cs
│   │   └── OrderDtos.cs
│   ├── Models/
│   │   ├── Product.cs
│   │   ├── CartItem.cs
│   │   └── Order.cs
│   ├── Services/
│   │   ├── IProductService.cs
│   │   ├── ProductService.cs
│   │   ├── ICartService.cs
│   │   ├── CartService.cs
│   │   ├── ICheckoutService.cs
│   │   └── CheckoutService.cs
│   ├── Program.cs
│   └── ShopLite.Api.csproj
└── ShopLite.Tests/
    ├── ShopLiteServiceTests.cs
    └── ShopLite.Tests.csproj
```

## Running Locally

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Start the API
```bash
dotnet run --project ShopLite.Api
```
The SQLite database (`shoplite.db`) is created and seeded automatically on startup.
- **API Base URL**: `http://localhost:5000`
- **Swagger UI**: `http://localhost:5000/swagger`

## Running with Docker

```bash
docker build -t shoplite-api .
docker run --rm -p 8080:8080 shoplite-api
```
- **API Base URL**: `http://localhost:8080`
- **Swagger UI**: `http://localhost:8080/swagger`

## Running Tests

Run the xUnit test suite (uses an isolated in-memory SQLite database):

```bash
dotnet test
```

## Example API Requests

### 1. List / Search Products
```bash
# Get all products
curl -X GET "http://localhost:5000/api/products"

# Search products by name (LINQ filter)
curl -X GET "http://localhost:5000/api/products?search=keyboard"
```

### 2. Add an Item to a Customer's Cart & View Cart
```bash
# Add 2 units of Product #1 to customer "alice"
curl -X POST "http://localhost:5000/api/cart/alice/items" \
  -H "Content-Type: application/json" \
  -d '{"productId": 1, "quantity": 2}'

# View cart with computed subtotals and total
curl -X GET "http://localhost:5000/api/cart/alice"
```

### 3. Checkout Cart
```bash
# Convert cart to an order, reduce stock, and clear cart
curl -X POST "http://localhost:5000/api/checkout/alice"
```
