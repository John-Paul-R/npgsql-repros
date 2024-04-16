using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;

Console.WriteLine("Hello, World!");

var connectionString = new NpgsqlConnectionStringBuilder
{
    Host = "localhost",
    Port = 5432,
    Database = "npgsql_repros",
    Username = "postgres",
    ArrayNullabilityMode = ArrayNullabilityMode.PerInstance,
}.ToString();


var options = new DbContextOptionsBuilder<MyDbContext>()
    .UseNpgsql(connectionString)
    .Options;

using (var dbContext = new MyDbContext(options))
{
    // Apply all pending migrations
    // dbContext.Database.Migrate();

    // Add your test code here
}

using (var connection = new NpgsqlConnection(connectionString)) {
    var entities = connection.Query<Obj>(
        /*language=sql*/$$"""
        SELECT
            ARRAY_AGG("set_04") as "Set04"
        FROM generate_series(0, 8) set_00
        CROSS JOIN generate_series(0, 8) set_01
        CROSS JOIN generate_series(0, 8) set_02
        CROSS JOIN generate_series(0, 8) set_03
        CROSS JOIN generate_series(0, 1) set_04
        GROUP BY "set_00", "set_01", "set_02", "set_03"
        """);

    Console.WriteLine("done! {0}", entities.ToList().Count);
}

public sealed class Obj {
    // public required int Id { get; set; }
    // public required int Set01 { get; set; }
    // public required int Set02 { get; set; }
    // public required int Set03 { get; set; }
    public required int[] Set04 { get; set; }
}


public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
    {
    }

    public DbSet<MyEntity> MyEntities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Add any model configurations here
    }
}

public class MyEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
}
