using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWTAPI.BussinessLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWTAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserLists _userlist;

        public UserController(UserLists userLists)
        {
            _userlist = userLists;
        }

        [HttpGet]
        [Route("GetUserList")]
        //[Authorize]
        public IActionResult GetUserList()
        {
        // var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        // var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // int id = int.Parse(userId);
        
        try
        {
            var response = _userlist.Get_All_User_Detail();
            return Ok(new { response = response });
        }
        catch (Exception ex)
        {
            //_logger.LogError(ex, "An error occurred while fetching recruitment data.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        }
    }
}