@echo off

set Elastic__Url=http://elastic:9200
set CUSTOMCONNSTR_ApplicationDbContext="Host=postgres;Database=postgres;Username=postgres;Password=postgres"
dotnet ef migrations remove --project .\InventoryManagement.Infrastructure\ --startup-project .\InventoryManagement.Api\