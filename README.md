# Product Management API

This is a **Product Management API** built with **ASP.NET Core 8** and connected to **MS SQL Server**. The front-end is built using **React 19**. This API allows users to perform CRUD operations on products.

## Prerequisites

Before running the application locally, make sure you have the following installed:

1. **.NET 8 SDK** (or newer) - To run the API.
2. **SQL Server** or **SQL Server Express** - To store your data.
3. **Node.js** and **npm** - For the React front-end.
4. **Microsoft Visual Studio 2022** (or newer) - For editing, building, and running the project.
5. **Postman** or any other API client to test API endpoints.

## Steps to Run the Application Locally Using Microsoft Visual Studio

### 1. Clone the Repository

Clone the repository to your local machine using the following command:

```bash
git clone https://github.com/sheptianbagjautama/ProductManagementApp.git
```

### 2. Set Up the Backend (ASP.NET Core 8)
#### 2.1 . Open the Solution in Visual Studio
- Open Microsoft Visual Studio.
- Click on File > Open > Project/Solution.
- Select the ProductManagementApp.sln file from the cloned repository.

#### 2.2 Install Dependencies
- Right-click on the solution in Solution Explorer.
- Select Restore NuGet Packages.

#### 2.3 Configure the Database Connection
- Open the appsettings.json file.
- Configure your SQL Server connection string under the ConnectionStrings section.

#### 2.4 Run Database Migrations
- Go to Tools > NuGet Package Manager > Package Manager Console.
- In the Package Manager Console, run the following command:
```bash
dotnet ef database update
```

#### 2.5 Run the API
In Visual Studio, press F5 or click Start Debugging to run the API locally.

### 3. Set Up the Front-End (React 19)
#### 3.1 Install Dependencies
- Open a terminal/command prompt.
- Navigate to the productmanagementapp.frontend folder 
```bash
cd productmanagementapp.frontend
```
- Install the required dependencies:
```bash
npm install
```
- Start the React Development Server
```bash
npm run dev
```
