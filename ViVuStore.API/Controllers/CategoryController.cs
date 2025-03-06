using Microsoft.AspNetCore.Mvc;

namespace ViVuStore.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{

    public CategoryController()
    {
    }

    public IActionResult Get()
    {
        return Ok();
    }
}
