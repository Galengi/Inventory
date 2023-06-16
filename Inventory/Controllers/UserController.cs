using Inventory.Models.Request;
using Inventory.Models.Response;
using Inventory.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        //[ValidateAntiForgeryToken]
        public IActionResult Autentificar([FromBody] AuthRequest model)
        {
            Response oResponse = new Response();
            var userResponse = _userService.Auth(model);
            if (userResponse == null)
            {
                oResponse.Message = "Usuario o contraseña incorrecto";
                return BadRequest(oResponse);
            }
            oResponse.Success = 1;
            oResponse.Data = userResponse;

            return Ok(oResponse);
        }
    }
}
