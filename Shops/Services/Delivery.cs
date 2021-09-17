using System.Collections.Generic;
using System.Linq;

namespace Shops.Services
{
    public class Delivery
    {
        public Delivery()
        {
            Units = new List<ProductUnit>();
        }

        public List<ProductUnit> Units { get; }

        public ProductUnit AddProduct(Product product, Price price, uint quantity)
        {
            ProductUnit unit = new ProductUnit(product, price, quantity);
            Units.Add(unit);
            return unit;
        }

        public void Deliver(Shop shop)
        {
            foreach (ProductUnit unit in Units)
            {
                shop.AddProduct(unit);
            }
        }
    }
}