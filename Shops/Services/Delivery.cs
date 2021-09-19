using System.Collections.Generic;
using System.Linq;

namespace Shops.Services
{
    public class Delivery
    {
        public Delivery(List<ProductUnit> units)
        {
            Units = units;
        }

        private List<ProductUnit> Units { get; }

        public void Deliver(Shop shop)
        {
            foreach (ProductUnit unit in Units)
            {
                shop.AddProduct(unit);
            }
        }
    }
}