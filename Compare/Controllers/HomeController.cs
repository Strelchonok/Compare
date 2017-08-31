using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Compare.Entities;
using System.IO;
using Compare.Models;
using Compare.Services;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Diagnostics;

namespace Compare.Controllers
{
    public class HomeController : Controller
    {
        private CompareContext db;
        private ICsvWriter CsvWriter;
        private IHostingEnvironment env;
        private IGenerate gen;

        public HomeController(CompareContext _db, ICsvWriter _csv, IHostingEnvironment _env, IGenerate _gen)
        {
            this.db = _db;
            this.CsvWriter = _csv;
            this.env = _env;
            this.gen = _gen;
        }
              
        private const int QtyVendors = 5000;
        private const int ProductsFromOneVendor = 100;
        private const int VendorsСoincidence = 80;
        private const int ProductsWithNewPrice = 60;

        public IActionResult Index()
        {
            return View();
        }

        public JsonResult GenerationToDB()
        {
            Stopwatch sw = Stopwatch.StartNew();
            List<Vendors> VendorsWithProducts = gen.GenerateVendorsAndProductsDB(QtyVendors, ProductsFromOneVendor);

            if (db.Vendors.Count() > 0)
                db.Database.ExecuteSqlCommand("DELETE FROM [Vendors]");

            db.Vendors.AddRange(VendorsWithProducts);
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                // Обработать исключения
            }
            sw.Stop();

          return Json(sw.Elapsed.ToString());
        }

        public JsonResult GenerationToCSV()
        {
            Stopwatch sw = Stopwatch.StartNew();
            const int QtyMatchingVendors = ((QtyVendors * VendorsСoincidence) / 100);
            const int QtyNotMatchingVendors = QtyVendors - QtyMatchingVendors;
            const int NewPriceProducts = ((QtyVendors * ProductsFromOneVendor) * ProductsWithNewPrice / 100);

            List<CsvModel> MatchingVendorsWithProducts = gen.GenerateVendorsAndProductsCSV(QtyMatchingVendors, ProductsFromOneVendor);
            List<CsvModel> NotMatchingVendorsWithProducts = gen.GenerateVendorsAndProductsCSV(QtyNotMatchingVendors, ProductsFromOneVendor, false);
            List<CsvModel> VendorsToCSV = MatchingVendorsWithProducts.Concat(NotMatchingVendorsWithProducts).ToList();

            // Забиваем новые цены, тестовое значение 555
            for (int i = 0; i < NewPriceProducts; i++) { VendorsToCSV[i].Price = 555; }

            string Path = env.WebRootPath;
            string fileName = string.Format("{0}\\test.csv", Path + "\\CsvFiles\\");
            CsvWriter.Write(VendorsToCSV, fileName, false);

            sw.Stop();
            return Json(sw.Elapsed.ToString());
        }

        public JsonResult Compare()
        {
            Stopwatch sw = Stopwatch.StartNew();
            // Достаем товары из CSV файла
            string Path = env.WebRootPath + "\\CsvFiles\\";
            string[] stringsCSV = CsvWriter.ReadCSVFile("test.csv", Path);
            List<CsvModel> FileProducts = CsvWriter.FromCsvToCSVModel(stringsCSV);

            
            int Coincidences = 0;
            int NewPrice = 0;
            foreach (var item in FileProducts)
            {
               var product =  db.Products.FirstOrDefault(c => c.VendorRef.VendorName == item.VendorName && c.ProductName == item.ProductName);
                if (product != null)
                {
                    Coincidences++;
                    if (product.Price != item.Price)
                    {
                        product.Price = item.Price;
                        NewPrice++;
                    }
                }
            }
            try
            {
                db.SaveChanges();
                sw.Stop();

            }
            catch (Exception)
            {
                // Обработать исключения
            }

            CompareResult result = new CompareResult
            {
                Coincidences = Coincidences,
                NewPrice = NewPrice,
                Time = sw.Elapsed.ToString()
            };

            return Json(result);
        }
    }
}
