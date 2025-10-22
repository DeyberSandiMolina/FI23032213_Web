using Microsoft.AspNetCore.Mvc;
using MVC.Models;

namespace MVC.Controllers;

public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Index(TheModel model)
    {
        ViewBag.Valid = ModelState.IsValid;
        if (ViewBag.Valid)
        { 
            var characters = model.Phrase!
                .Where(c => c != ' ')  
                .ToArray();
               
            model.Counts = characters
                .GroupBy(c => c)
                .ToDictionary(g => g.Key, g => g.Count())
                .OrderByDescending(kvp => kvp.Value)
                .ThenBy(kvp => kvp.Key)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            
            model.Lower = new string(characters.Select(c => char.ToLower(c)).ToArray());
            model.Upper = new string(characters.Select(c => char.ToUpper(c)).ToArray());
        }
        return View(model);
    }
}
