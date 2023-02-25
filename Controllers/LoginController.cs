using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFirstAPI.Models;

namespace MyFirstAPI.Controllers
{
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ACE42023Context db;
        public LoginController( ACE42023Context _db)
        {
            db = _db;
        }

        [HttpPost]
        [Route("api/login", Name = "Authentication")]
        public  ActionResult Login(RahulUser? u){
            if(u.Username == null || u.Userpassword == null){
                return BadRequest(new {Message = "Username and Password are Required!"});
            } 
            var result = (from i in db.RahulUsers
                            where  i.Username == u.Username && i.Userpassword == u.Userpassword
                            select i).SingleOrDefault();
            
            if(result != null){
                return Ok(result);
            }
            else{
                return Unauthorized(new {Message = "Invalid Credentials!"});
            }
        }
    }
}
