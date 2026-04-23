var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume("recyclingapp-postgres-data")
    .WithPgAdmin();

var db = postgres.AddDatabase("recyclingapp");

builder.AddProject<Projects.RecyclingApp_Api>("api")
    .WithReference(db)
    .WaitFor(db);

builder.Build().Run();
