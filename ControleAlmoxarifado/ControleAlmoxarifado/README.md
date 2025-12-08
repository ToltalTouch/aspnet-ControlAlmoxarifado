# Controle Almoxarifado

## Overview
Controle Almoxarifado is a web application built with ASP.NET Core that manages inventory items in a warehouse. The application allows users to record item entries and exits, providing a simple interface for tracking inventory levels.

## Features
- Add new items to the inventory.
- Record item exits from the inventory.
- View current inventory levels.
- User-friendly interface with ASP.NET Core MVC.

## Project Structure
```
ControleAlmoxarifado
├── Controllers
│   └── HomeController.cs
├── Models
│   ├── Item.cs
│   └── InventoryTransaction.cs
├── Views
│   ├── Home
│   │   └── Index.cshtml
│   ├── Shared
│   │   └── _Layout.cshtml
│   ├── _ViewStart.cshtml
│   └── _ViewImports.cshtml
├── Data
│   └── ApplicationDbContext.cs
├── wwwroot
│   ├── css
│   │   └── site.css
│   └── js
│       └── site.js
├── appsettings.json
├── Program.cs
├── Startup.cs
├── ControleAlmoxarifado.csproj
└── README.md
```

## Setup Instructions
1. Clone the repository to your local machine.
2. Navigate to the project directory.
3. Restore the project dependencies by running:
   ```
   dotnet restore
   ```
4. Update the `appsettings.json` file with your database connection string.
5. Run the application using:
   ```
   dotnet run
   ```
6. Open your web browser and navigate to `http://localhost:5000` to access the application.

## Contributing
Contributions are welcome! Please feel free to submit a pull request or open an issue for any enhancements or bug fixes.

## License
This project is licensed under the MIT License. See the LICENSE file for more details.