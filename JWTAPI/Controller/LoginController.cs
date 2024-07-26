using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWTAPI.BussinessLogic;
using JWTAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace JWTAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly UserLists _userLists;

        public LoginController(UserLists userLists)
        {
            _userLists = userLists;
        }

        [HttpPost("login")]
        public ActionResult<UserDetails> LoginUser([FromBody] LoginModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Invalid login data");
            }

            var result = _userLists.LoginCheck(model.Email, model.Password);
            if (result != null)
            {
                return Ok(result);
            }
            return Unauthorized("Invalid email or password");
        }
    }
}