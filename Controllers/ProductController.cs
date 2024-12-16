using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.IO;
using eMuStock.Models;

namespace eMuStock.Controllers
{
    public class ProductController : Controller
    {
        private readonly string dbPath = "db.json";

        public IActionResult Index()
        {
            var products = LoadProducts();
            return View(products);
        }

        [HttpPost]
        public IActionResult Add(Product product)
        {
            var products = LoadProducts();
            product.Id = products.Count > 0 ? products[^1].Id + 1 : 1; // Yeni ID
            products.Add(product);
            SaveProducts(products);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id, int? quantity)
        {
            var products = LoadProducts();
            var product = products.FirstOrDefault(p => p.Id == id);

            if (product != null)
            {
                if (quantity.HasValue && quantity.Value > 0)
                {
                    product.Stock -= quantity.Value;
                    if (product.Stock <= 0) products.Remove(product);
                }
                else
                {
                    products.Remove(product);
                }

                SaveProducts(products);
            }

            return RedirectToAction("Index");
        }

        private List<Product> LoadProducts()
        {
            if (!System.IO.File.Exists(dbPath))
                return new List<Product>();

            var json = System.IO.File.ReadAllText(dbPath);
            return JsonSerializer.Deserialize<List<Product>>(json) ?? new List<Product>();
        }

        private void SaveProducts(List<Product> products)
        {
            var json = JsonSerializer.Serialize(products);
            System.IO.File.WriteAllText(dbPath, json);
        }
    }
}
