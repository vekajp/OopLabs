using System;

namespace Shops.Services
{
    public class Product
    {
    public Product(int id, string name)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException();
    }

    public int Id { get; }
    public string Name { get; }
    }
}