@echo off
cd TourismSmartTransportation.Data
dotnet ef dbcontext scaffold "Server=se32.database.windows.net;Database=tourism-smart-transportation;TrustServerCertificate=true;User Id=se32;Password=Password@32" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --context-dir Context --force --no-onconfiguring
if %errorlevel%==0 (echo Done!) else (echo Failed!)
pause