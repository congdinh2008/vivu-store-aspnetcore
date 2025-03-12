# Migrations

## 1. Using the CLI

### Add a migration
```bash
dotnet ef migrations add AddSupplierModel --project ViVuStore.Data --startup-project ViVuStore.API --context ViVuStoreDbContext --output-dir Migrations
dotnet ef migrations add [MigrationName] --project ViVuStore.Data --startup-project ViVuStore.API --context ViVuStoreDbContext --output-dir Migrations
dotnet ef migrations add [MigrationName] --project ViVuStore.Data --startup-project ViVuStore.API --context StorageDbContext --output-dir Migrations/Storage
```

### Update the database
```bash
dotnet ef database update --project ViVuStore.Data --startup-project ViVuStore.API --context ViVuStoreDbContext
dotnet ef database update --project ViVuStore.Data --startup-project ViVuStore.API --context StorageDbContext
```

### Roll back a migration
```bash
dotnet ef database update AddIdentityModel --project ViVuStore.Data --startup-project ViVuStore.API --context ViVuStoreDbContext
dotnet ef database update [MigrationName] --project ViVuStore.Data --startup-project ViVuStore.API --context StorageDbContext
```

### Drop the database
```bash
dotnet ef database drop --project ViVuStore.Data --startup-project ViVuStore.API --context ViVuStoreDbContext
dotnet ef database drop --project ViVuStore.Data --startup-project ViVuStore.API --context StorageDbContext
```

### Remove a migration
```bash
dotnet ef migrations remove --project ViVuStore.Data --startup-project ViVuStore.API --context ViVuStoreDbContext
dotnet ef migrations remove --project ViVuStore.Data --startup-project ViVuStore.API --context StorageDbContext
```

## 2. Using the Package Manager Console
### Add a migration
```bash
Add-Migration [MigrationName] -Project ViVuStore.Data -StartupProject ViVuStore.API -Context ViVuStoreDbContext -OutputDir ViVuStore.Data/Migrations
```

### Update the database
```bash
Update-Database -Project ViVuStore.Data -StartupProject ViVuStore.API -Context ViVuStoreDbContext
```

### Roll back a migration
```bash
Update-Database [MigrationName] -Project ViVuStore.Data -StartupProject ViVuStore.API -Context ViVuStoreDbContext
```

### Remove a migration
```bash
Remove-Migration -Project ViVuStore.Data -StartupProject ViVuStore.API -Context ViVuStoreDbContext
```

[]: # Path: README.md
