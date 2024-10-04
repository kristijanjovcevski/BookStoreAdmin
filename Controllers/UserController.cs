using BookStoreAdminApplication.Models;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace BookStoreAdminApplication.Controllers
{
    public class UserController : Controller
    {
            //public UserManager<EShopApplicationUser> usermanager;
            //public UserController(UserManager<EShopApplicationUser> usermanager)
            //{
            //    this.usermanager = usermanager;
            //}
            public IActionResult Index()
            {
                return View();
            }

            public IActionResult ImportUsers(IFormFile file)
            {
                string homePath = Environment.GetEnvironmentVariable("HOME");

                string appDataPath = Path.Combine(homePath, "site", "wwwroot", "wwwroot", "files");

                string pathToUpload = Path.Combine(appDataPath, file.FileName);

                using (FileStream fileStream = System.IO.File.Create(pathToUpload))
                {
                    file.CopyTo(fileStream);
                    fileStream.Flush();
                }

                List<User> users = getAllUsersFromFile(file.FileName);
                HttpClient client = new HttpClient();
                string URL = "https://bookstoreweb20240918115617.azurewebsites.net/api/Admin/ImportAllUsers";

                HttpContent content = new StringContent(JsonConvert.SerializeObject(users), Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PostAsync(URL, content).Result;

                var result = response.Content.ReadAsAsync<bool>().Result;

                return RedirectToAction("Index");

            }

            private List<User> getAllUsersFromFile(string fileName)
            {
                List<User> users = new List<User>();

                //string filePath = Path.Combine(@"D:\home\site\wwwroot\files\", fileName);
    
                string homePath = Environment.GetEnvironmentVariable("HOME");

                string appDataPath = Path.Combine(homePath, "site", "wwwroot","wwwroot","files");




                string filePath = Path.Combine(appDataPath, fileName);



                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        while (reader.Read())
                        {
                            users.Add(new Models.User
                            {
                                Email = reader.GetValue(0).ToString(),
                                Password = reader.GetValue(1).ToString(),
                                ConfirmPassword = reader.GetValue(2).ToString()
                            });
                        }

                    }
                }
                return users;

            }
        }
    }

