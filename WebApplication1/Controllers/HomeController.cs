using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private MySqlConnection _connection =
            new MySqlConnection(@"server=localhost;user=root;database=gku;password=mysql");
        private MySqlDataAdapter _adapter;
        private DataSet _dataSet = new DataSet();
        private DataTable _dataTable = new DataTable();
        private MySqlCommandBuilder _commandBuilder;
        public int UserID = 0;

        private DataSet getDataUsers()
        {
            _adapter = new MySqlDataAdapter("select * from users", _connection);
            _adapter.Fill(_dataSet);
            return _dataSet;
        }
        
        private DataSet getDataTariffs()
        {
            _adapter = new MySqlDataAdapter("select * from tariffs", _connection);
            _adapter.Fill(_dataSet);
            return _dataSet;
        }
        
        private DataSet getDataConsuption()
                 {
            _adapter = new MySqlDataAdapter("select * from consuption", _connection);
            _adapter.Fill(_dataSet);
            return _dataSet;
        }
        
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel input)
        {
            if (ModelState.IsValid)
            {
                _dataSet = getDataUsers();
                var user = from data in _dataSet.Tables[0].AsEnumerable()
                    where (string) data["login"] == input.login && (string) data["password"] == input.password
                    select data;
                var resid = from data in _dataSet.Tables[0].AsEnumerable()
                    where (string) data["login"] == input.login && (string) data["password"] == input.password
                    select (int) data["userid"];
                
                if (user.Count() != 0)
                {
                    UserID = resid.FirstOrDefault();
                    await Authenticate(UserID.ToString()); // аутентификация
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View();
        }
        
        private async Task Authenticate(string userName)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
        
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Home");
        }
            
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if(User.Identity.IsAuthenticated)
            {
                ViewBag.ID = User.Identity.Name;
                return View();
            }
            return RedirectToAction("Login", "Home");
        }
        
        public IActionResult users()
        {
            if (User.Identity.Name == "1")
            {
                _dataSet = getDataUsers();
                ViewBag.Users = _dataSet.Tables[0].AsEnumerable();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult users(string surname, string name, string street, string house, string login,
            string password)
        {
            _dataSet = getDataUsers();
            _commandBuilder = new MySqlCommandBuilder(_adapter);
            _dataTable = _dataSet.Tables[0];
            DataRow dataRow = _dataTable.NewRow();
            dataRow["surname"] = surname;
            dataRow["name"] = name;
            dataRow["street"] = street;
            dataRow["house"] = house;
            dataRow["login"] = login;
            dataRow["password"] = password;
            _dataTable.Rows.Add(dataRow);
            _adapter.Update(_dataSet);
            _dataSet.AcceptChanges();
            ViewBag.Users = _dataSet.Tables[0].AsEnumerable();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        public IActionResult consuption()
        {
            if(User.Identity.IsAuthenticated)
            {
                _dataSet = getDataConsuption();
                var result = from data in _dataSet.Tables[0].AsEnumerable()
                    where (int) data["userid"] == int.Parse(User.Identity.Name)
                    select data;
                ViewBag.Cons = result;
                return View();
            }
            return RedirectToAction("Login", "Home");
        }
        
        public IActionResult addtariff()
        {
            if (User.Identity.Name == "1")
            {
                _dataSet = getDataTariffs();
                ViewBag.Users = _dataSet.Tables[0].AsEnumerable();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult addtariff(string name, int price)
        {
            _dataSet = getDataTariffs();
            _commandBuilder = new MySqlCommandBuilder(_adapter);
            _dataTable = _dataSet.Tables[0];
            DataRow dataRow = _dataTable.NewRow();
            dataRow["name"] = name;
            dataRow["price"] = price;
            _dataTable.Rows.Add(dataRow);
            _adapter.Update(_dataSet);
            _dataSet.AcceptChanges();
            ViewBag.Users = _dataSet.Tables[0].AsEnumerable();
            return View();
        }
    }
}