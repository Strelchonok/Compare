using System;
using System.Collections.Generic;

namespace Compare.Entities
{
    public partial class Vendors
    {
        public Vendors()
        {
            Products = new HashSet<Products>();
        }

        public int VendorId { get; set; }
        public string VendorName { get; set; }

        public virtual ICollection<Products> Products { get; set; }
    }
}
