using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

public class StudentControler : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}