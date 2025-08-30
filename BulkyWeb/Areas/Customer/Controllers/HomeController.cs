using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category,ProductImages");
            return View(productList);
        }

        public IActionResult AllBooks()
        {
            var products = _unitOfWork.Product.GetAll(includeProperties: "Category,ProductImages").ToList();

            if (User.Identity != null && User.Identity.IsAuthenticated && User.IsInRole("Employee"))
            {
                var today = DateTime.Today;
                var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
                var startOfMonth = new DateTime(today.Year, today.Month, 1);

                // Merr të gjitha detajet e porosive që kanë OrderHeader të lidhur dhe janë APPROVED
                var orderDetails = _unitOfWork.OrderDetail
                    .GetAll(includeProperties: "Product,OrderHeader")
                    .Where(od => od.OrderHeader.OrderStatus == SD.StatusApproved)
                    .ToList();

                var salesToday = orderDetails
                    .Where(od => od.OrderHeader.OrderDate.Date == today)
                    .GroupBy(od => od.ProductId)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.Count * x.Price));

                var salesThisWeek = orderDetails
                    .Where(od => od.OrderHeader.OrderDate >= startOfWeek)
                    .GroupBy(od => od.ProductId)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.Count * x.Price));

                var salesThisMonth = orderDetails
                    .Where(od => od.OrderHeader.OrderDate >= startOfMonth)
                    .GroupBy(od => od.ProductId)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.Count * x.Price));

                var orderCount = orderDetails
                    .GroupBy(od => od.ProductId)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.Count));

                // ViewBag që përdoren në AllBooks.cshtml
                ViewBag.SalesToday = salesToday;
                ViewBag.SalesThisWeek = salesThisWeek;
                ViewBag.SalesThisMonth = salesThisMonth;
                ViewBag.OrderCount = orderCount;
            }

            return View(products);
        }


        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult Details(int productId)
        {
            ShoppingCart cart = new()
            {
                Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category,ProductImages"),
                Count = 1,
                ProductId = productId
            };
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;

            var cartFromDb = _unitOfWork.ShoppingCart.Get(
                u => u.ApplicationUserId == userId && u.ProductId == shoppingCart.ProductId);

            if (cartFromDb != null)
            {
                cartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }
            else
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
            }

            _unitOfWork.Save();

            HttpContext.Session.SetInt32(SD.SessionCart,
                _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());

            TempData["success"] = "Cart updated successfully";
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
