using Microsoft.AspNetCore.Mvc;

namespace Eve.Api.Controllers.Common;
[ApiController]
[Route("[controller]")]
public abstract class BaseController : ControllerBase
{
}
