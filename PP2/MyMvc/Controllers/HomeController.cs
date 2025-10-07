using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyMvc.Models;

namespace MyMvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult MyForm()
    {
        return View(new MyBinary());
    }


    [HttpPost]
    public IActionResult MyForm(MyBinary model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        else { 

        // Normalizar a 8 bits
        string a8 = model.a.PadLeft(8, '0');
        string b8 = model.b.PadLeft(8, '0');

        int decA = Convert.ToInt32(model.a, 2);
        int decB = Convert.ToInt32(model.b, 2);

        // Operaciones binarias
        string andBin = new string(a8.Zip(b8, (x, y) => (x == '1' && y == '1') ? '1' : '0').ToArray());
        string orBin = new string(a8.Zip(b8, (x, y) => (x == '1' || y == '1') ? '1' : '0').ToArray());
        string xorBin = new string(a8.Zip(b8, (x, y) => (x != y) ? '1' : '0').ToArray());

        // Operaciones aritméticas
        int sum = decA + decB;
        int product = decA * decB;

        // Enviar todo a la vista usando ViewBag
        ViewBag.A = ToAllBases(decA, a8);
        ViewBag.B = ToAllBases(decB, b8);
        ViewBag.AND = ToAllBases(Convert.ToInt32(andBin, 2), andBin);
        ViewBag.OR = ToAllBases(Convert.ToInt32(orBin, 2), orBin);
        ViewBag.XOR = ToAllBases(Convert.ToInt32(xorBin, 2), xorBin);
        ViewBag.Sum = ToAllBases(sum, Convert.ToString(sum, 2));
        ViewBag.Product = ToAllBases(product, Convert.ToString(product, 2));

        return View("ResultTable");
        }
    }

    private dynamic ToAllBases(int value, string binary)
    {
        return new
        {
            Binary = binary.PadLeft(8, '0'),
            Octal  = Convert.ToString(value, 8),
            Decimal = value.ToString(),
            Hex    = Convert.ToString(value, 16).ToUpper()
        };
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
