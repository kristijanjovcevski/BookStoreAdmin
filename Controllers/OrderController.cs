using BookStoreAdminApplication.Models;
using ClosedXML.Excel;
using GemBox.Document;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Template;
using Newtonsoft.Json;
using System.Text;

namespace BookStoreAdminApplication.Controllers
{
    public class OrderController : Controller
    {

        public OrderController()
        {
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
        }
        public IActionResult Index()
        {
            HttpClient client = new HttpClient();
            string URL = "https://bookstoreweb20240918115617.azurewebsites.net/api/Admin/GetAllOrders";
            HttpResponseMessage response = client.GetAsync(URL).Result;

            var data = response.Content.ReadAsAsync<List<Order>>().Result;
            return View(data);
        }

        public IActionResult Details(Guid Id)
        {
            HttpClient client = new HttpClient();
            string URL = "https://bookstoreweb20240918115617.azurewebsites.net/api/Admin/GetDetailsForOrder";
            var model = new
            {
                Id = Id
            };
            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            var data = response.Content.ReadAsAsync<Order>().Result;
            return View(data);
        }

        public FileContentResult CreateInvoice(Guid Id)
        {
            HttpClient client = new HttpClient();
            string URL = "https://bookstoreweb20240918115617.azurewebsites.net/api/Admin/GetDetailsForOrder";
            var model = new
            {
                Id = Id
            };
            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            var data = response.Content.ReadAsAsync<Order>().Result;

            //var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Invoice.docx");


            string homePath = Environment.GetEnvironmentVariable("HOME");


            string appDataPath = Path.Combine(homePath, "site", "wwwroot","wwwroot","files");


            var templatePath = Path.Combine(appDataPath, "Invoice.docx");


            var document = DocumentModel.Load(templatePath);
            document.Content.Replace("{{OrderNumber}}", data.Id.ToString());
            document.Content.Replace("{{UserName}}", data.Owner.UserName);
            StringBuilder sb = new StringBuilder();
            double total = 0.0;

            foreach (var item in data.BooksInOrder)
            {
                sb.Append("Book " + item?.Book?.Title + " with quantity " + item?.Quantity + " with price " + item?.Book?.Price + " мкд");
                total += (item.Quantity * item.Book.Price);
            }

            document.Content.Replace("{{BookList}}", sb.ToString());
            document.Content.Replace("{{TotalPrice}}", total.ToString() + " мкд");

            var stream = new MemoryStream();
            document.Save(stream, new PdfSaveOptions());
            return File(stream.ToArray(), new PdfSaveOptions().ContentType, "ExportedInvoice.pdf");
        }

        [HttpGet]
        public FileContentResult ExportOrders()
        {
            string fileName = "Orders.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            using(var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("All Orders");
                worksheet.Cell(1,1).Value = "Order Id";
                worksheet.Cell(1, 2).Value = "Customer Email";

                HttpClient client = new HttpClient();
                string URL = "https://bookstoreweb20240918115617.azurewebsites.net/api/Admin/GetAllOrders";
                HttpResponseMessage response = client.GetAsync(URL).Result;

                var data = response.Content.ReadAsAsync<List<Order>>().Result;

                for(int i = 1; i <= data.Count(); i++)
                {
                    var item = data[i-1];

                    worksheet.Cell(i+1, 1).Value = item.Id.ToString();
                    worksheet.Cell(i+1, 2).Value = item.Owner.Email;

                    for(int b = 0; b < item.BooksInOrder.Count(); b++)
                    {
                        worksheet.Cell(1, b + 3).Value = "Book-" + (b + 1);
                        worksheet.Cell(i + 1, b + 3).Value = item.BooksInOrder.ElementAt(b).Book.Title;
                    }
                }
                using(var stream = new MemoryStream()) 
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, contentType, fileName);
                }
            }
            
        }

    }
}
