using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWTAPI.BussinessLogic;
using Microsoft.AspNetCore.Mvc;

namespace JWTAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly UserLists _userLists;

        public LoginController (UserLists userLists)
        {
            _userLists = userLists;
        }

        [HttpGet]
        public string LoginUser (string email, string password)
        {
            var result = _userLists.LoginCheck(email,password);
            return result;
            
        }
    }
}