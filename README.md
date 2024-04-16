
# npgsql repro: ArrayNullabilityMode.PerInstance buffer over-read

## Usage

start the database via docker (the below will host on port 5433):

```
docker build ./ -f ./Dockerfile -t npgsql-repros:nullable-array-overread
docker run -p5433:5432 npgsql-repros:nullable-array-overread
```

run the dotnet project

```
dotnet run 
```

observe crash due to buffer over-read

## Error Description

This particular error is caused by `PolymorphicArrayConverter<T>`'s `Read` method leading with 2 unchecked `ReadInt32` calls,
which may read past the boundary of the working buffer.

permalink: https://github.com/npgsql/npgsql/blob/058894067d33229fbef2f3bcafbfa75858fc60fb/src/Npgsql/Internal/Converters/ArrayConverter.cs#L631-L639

offending code snippet:

```csharp
    public override TBase Read(PgReader reader)
    {
        _ = reader.ReadInt32();
        var containsNulls = reader.ReadInt32() is 1;
        reader.Rewind(sizeof(int) + sizeof(int));
        return containsNulls
            ? _nullableElementCollectionConverter.Read(reader)
            : _structElementCollectionConverter.Read(reader);
    }
```

I've personally patched this with

```csharp
    public override TBase Read(PgReader reader)
    {
        if (reader.ShouldBuffer(sizeof(int) + sizeof(int)))
            reader.Buffer(sizeof(int) + sizeof(int));

        _ = reader.ReadInt32();
        var containsNulls = reader.ReadInt32() is 1;
        reader.Rewind(sizeof(int) + sizeof(int));
        return containsNulls
            ? _nullableElementCollectionConverter.Read(reader)
            : _structElementCollectionConverter.Read(reader);
    }
```
