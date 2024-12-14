If you get: "Cannot access a disposed context instance..."
1. It's because you are returning "async void", return "async Task" or "async Task<>" instead
2. It's because at least 1 method in controller-service-repository DI chain is not returning async Task - all of them must return async Task (including controller)
3. We don't `await` on some async function in controller-service-repository DI chain

---

DB migrations:
`dotnet ef migrations add init --context ProvinceDbContext` - init
`dotnet ef migrations add NewMigrationName --context ProvinceDbContext` - when our Models changed we need to add new migration (remember to apply it)
`dotnet ef database update --context ProvinceDbContext` - apply any local migrations to DB
`dotnet ef migrations remove --context ProvinceDbContext`