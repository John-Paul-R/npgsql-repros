using Dapper;
using Npgsql;

Console.WriteLine("Hello, World!");

var connectionString = new NpgsqlConnectionStringBuilder
{
    Host = "localhost",
    Port = 5433,
    Database = "npgsql_repros",
    Username = "postgres",
    ArrayNullabilityMode = ArrayNullabilityMode.PerInstance,
}.ToString();

using (var connection = new NpgsqlConnection(connectionString)) {
    // There is nothing special about the below query other than that
    // it was the first array-valued query I could construct that hit
    // the "2 bytes of array metadata start less than 8 bytes from the
    // end of the 8192 byte npgsql buffer" error condition.
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
