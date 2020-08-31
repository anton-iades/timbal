# timbal

## about

A collection (n=1) of utility methods.

## requirements

- [.NET Core SDK 3.1](https://dotnet.microsoft.com/download)

## Allocator

### Description

Allocates an amount evenly or proportionally to a collection of items.
The allocation uses a configurable precision and rounding method.
If an amount cannot be completely allocated, the remainder will be included in the result.

```csharp
// items can be any IEnumerable<T>
var items = new []
{
    new { Name = "Obj1", Weight = 1m},
    new { Name = "Obj2", Weight = 2m}
    new { Name = "Obj3", Weight = 3m}
};

// settings (optional)
var settings = new AllocatorSettings
{
    Precision = 2,
    MidpointRounding = MidpointRounding.AwayFromZero
};

// allocate evenly
var r1 = items.AllocateEvenly(10m, settings);
// r1.Allocations[0].Value => 3.33m
// r1.Allocations[1].Value => 3.33m
// r1.Allocations[2].Value => 3.33m
// r1.Remainder => 0.01m

// allocate proportionally
var r2 = items.AllocateProportionally(10m, i => i.Weight, settings);
// r2.Allocations[0].Value => 1.67m
// r2.Allocations[1].Value => 3.33m
// r2.Allocations[2].Value => 5m
// r2.Remainder => 0m
```

### Constraints

- Amount to allocate can be any valid `decimal`
- Items must contain at least one element
- The sum of weights must be non-zero (negative total weight allowed)

## run/test

```sh
dotnet test
```
