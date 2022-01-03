@echo off
cd TourismSmartTransportation.Data
dotnet ef dbcontext scaffold "Server=tourism-smart-transportation.database.windows.net;Database=TourismSmartTransportation;TrustServerCertificate=true;User Id=uni03_cp;Password=password@03" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --context-dir Context --force --no-onconfiguring
if %errorlevel%==0 (echo Done!) else (echo Failed!)
pause