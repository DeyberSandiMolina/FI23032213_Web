using Microsoft.AspNetCore.Mvc;
using QuixoWeb.Data.Repositories;

public class QuixoController : Controller
{
    private readonly IQuixoRepository _repo;

    public QuixoController(IQuixoRepository repo)
    {
        _repo = repo;
    }

    public async Task<IActionResult> Index()
    {
        var lista = await _repo.GetAllAsync();
        return View(lista);
    }
}
