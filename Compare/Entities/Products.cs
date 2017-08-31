using System;
using System.Collections.Generic;

namespace Compare.Entities
{
    public partial class Products
    {
        public int ProductId { get; set; }
        public int VendorRefId { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }

        public virtual Vendors VendorRef { get; set; }
    }
}
