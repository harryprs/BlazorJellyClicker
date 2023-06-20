# BlazorJellyClicker
## About The Project
A jelly themed idle clicker game developed in .NET Core 7 Blazor.

### Features
* Gain score by clicking on the clickable jelly.
* Buy various jellies to gain passive income.
* Buy various upgrades to boost stats of specific jellies, or global stats such as click power and overall income.
* Secure API endpoints using JWT-based Authentication.
* Play as a Guest, or Register and Login to an account for more save functionality.
* Maintain progress: **Auto-save/load** from local storage. **Save/load from database** with a registered account. **Export/Import** save file.

### Built With
* .NET Core 7 API
* ASP.NET Core
* Blazor
* EntityFrameworkCore
* C#
* HTML
* CSS
* Developed in Visual Studio 2022

### Building And Running The App
1. Clone this project.
2. Open **BlazorJellyClicker.sln**
3. In Visual Studio Build menu, select Build Solution (**F6**).
4. Run the application locally (**F5**).

### Adding A Database
If you want to implement a database to be able to register/login and save/load from an account:
1. Update **ConnectionStrings:defaultConnection** value in **Server/appsettings.json** to your DB.
2. Update **SecretKeys:JwtKey and SecretKeys:RefreshKey** values in **Server/appsettings.json**.
3. In the NuGet Package Manager Console, create your database from the Migrations by typing **Update-Database**.


### Contact
Harry Parsons - harryprs@hotmail.co.uk
