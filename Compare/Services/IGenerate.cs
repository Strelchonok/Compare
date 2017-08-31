using Compare.Entities;
using Compare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Compare.Services
{
    public interface IGenerate
    {
        List<Vendors> GenerateVendorsAndProductsDB(int QtyVendors, int QtyProductsForOneVendor, bool MatchingProducts = true);
        List<CsvModel> GenerateVendorsAndProductsCSV(int QtyVendors, int QtyProductsForOneVendor, bool MatchingProducts = true);
    }

    public class Generate : IGenerate
    {
        public Generate()
        {

        }

        public List<Vendors> GenerateVendorsAndProductsDB(int QtyVendors, int QtyProductsForOneVendor, bool MatchingProducts = true)
        {
            if (MatchingProducts)
            {
                List<Vendors> VendorsWithProducts = new List<Vendors>();
                for (int v = QtyVendors; v != 0; v--)
                {
                    Vendors vendor = new Vendors
                    {
                        VendorName = "Vendor" + v
                    };

                    List<Products> VendorProducts = new List<Products>();
                    for (int p = QtyProductsForOneVendor; p != 0; p--)
                    {
                        Products product = new Products
                        {
                            ProductName = "V." + v + "_" + "Product" + p,
                            Price = p
                        };
                        VendorProducts.Add(product);
                    }
                    vendor.Products = VendorProducts;
                    VendorsWithProducts.Add(vendor);
                }
                return VendorsWithProducts;
            }
            else {
                List<Vendors> VendorsWithProducts = new List<Vendors>();
                for (int v = QtyVendors; v != 0; v--)
                {
                    Vendors vendor = new Vendors
                    {
                        VendorName = "FakeVendor" + v
                    };

                    List<Products> VendorProducts = new List<Products>();
                    for (int p = QtyProductsForOneVendor; p != 0; p--)
                    {
                        Products product = new Products
                        {
                            ProductName = "V." + v + "_" + "FakeProduct" + p,
                            Price = p,
                            
                        };
                        VendorProducts.Add(product);
                    }
                    vendor.Products = VendorProducts;
                    VendorsWithProducts.Add(vendor);
                }
                return VendorsWithProducts;
            }

        }

        public List<CsvModel> GenerateVendorsAndProductsCSV(int QtyVendors, int QtyProductsForOneVendor, bool MatchingProducts = true)
        {
            if (MatchingProducts)
            {
                string vendorName;
                List<CsvModel> VendorsAndProducts = new List<CsvModel>();
                for (int v = QtyVendors; v != 0; v--)
                {
                    vendorName = "Vendor" + v;
          
                    for (int p = QtyProductsForOneVendor; p != 0; p--)
                    {
                        CsvModel product = new CsvModel
                        {
                            VendorName = vendorName,
                            ProductName = "V." + v + "_" + "Product" + p,
                            Price = p
                        };
                        VendorsAndProducts.Add(product);
                    }
                }
                return VendorsAndProducts;
            }
            else
            {
                string vendorName;
                List<CsvModel> VendorsAndProducts = new List<CsvModel>();
                for (int v = QtyVendors; v != 0; v--)
                {
                    vendorName = "FakeVendor" + v;

                    for (int p = QtyProductsForOneVendor; p != 0; p--)
                    {
                        CsvModel product = new CsvModel
                        {
                            VendorName = vendorName,
                            ProductName = "V." + v + "_" + "FakeProduct" + p,
                            Price = p
                        };
                        VendorsAndProducts.Add(product);
                    }
                }
                return VendorsAndProducts;
            }

           

        }

        
        }
    }
    

