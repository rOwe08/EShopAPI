# Eshop API  

A RESTful API for managing e-shop products.

## Features  
- CRUD operations for products  
- Versioned API endpoints (v1, v2)  
- Async stock updates using an in-memory queue  
- Swagger documentation  

## Prerequisites  
- .NET 8 SDK  
- SQLite (for development)  

## Running the Application  
1. Clone the repository:  
   ```bash  
   git clone https://github.com/yourusername/EShopAPI.git
   
2. Navigate to the project directory:

   ```bash  
   cd EshopApi
   
3. Run the application:

   ```bash  
   dotnet run
   
4. Access Swagger UI at:
  http://localhost:5272


## Running Unit Tests

1. Clone the repository(if you didn't before:  
   ```bash  
   git clone https://github.com/yourusername/EShopAPI.git
   
2. Navigate to the test project:

    ```bash
    cd EshopApi.Tests
    
3. Run tests:

    ```bash
    dotnet test 
