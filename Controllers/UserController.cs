using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFirstAPI.Models;
// using System.Web.Http;

namespace MyFirstAPI.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ACE42023Context db;
        public UserController( ACE42023Context _db)
        {
            db = _db;
        }
        //Logout -> API
        // public IActionResult Logout(){
        //     HttpContext.Session.Clear();
        //     return RedirectToAction("Login", "Login");
        // }
        [HttpPost]
        [Route("api/signup", Name = "CreateAUser")]
        public async Task<ActionResult> Create(RahulUser? u){
            //Check VAlidation errors
            if(u == null){
                return BadRequest(new {Message = "Please Provide all the necessary details!"});
            }

            var check_u = db.RahulUsers.Where(x => x.Username == u.Username).SingleOrDefault();
            if(check_u != null){
                return BadRequest(new {Message = "User already Exists!"});
            }
            if(ModelState.IsValid){
                db.RahulUsers.Add(u);
                await db.SaveChangesAsync();
                return Ok(u);
            }else{
                return BadRequest("Please Provide details in valid form!");
            }
        }
        [HttpPost]
        [Route("api/user", Name = "GetUserID")]
        public ActionResult GetUserID([FromBody] string? uname){
            //Check VAlidation errors
            if(uname == null){
                return BadRequest(new {Message = "Please provide username!"});
            }
            var check_u = db.RahulUsers.Where(x => x.Username == uname).SingleOrDefault();
            if(check_u != null){
                return Ok(check_u.Userid);
            }
            else{
                return BadRequest("Please provide valid username!");
            }
        }
        
    }
}
