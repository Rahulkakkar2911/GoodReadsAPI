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
    // [RoutePrefix("api/books")]
    public class BookController : ControllerBase
    {
        private readonly ACE42023Context db;
        public BookController( ACE42023Context _db)
        {
            db = _db;
        }

        [HttpGet]
        [Route("api/books", Name = "GetAllBooks")]
        public async Task<ActionResult<IEnumerable<RahulBook>>> Book(){
            return Ok(await db.RahulBooks.ToListAsync());
        }
        [HttpGet]
        [Route("api/books/{id:int}", Name="GetBookById")]
        public async Task<ActionResult> GetBookByID(int? id){
            if(id == null){
                return BadRequest(new {Message = "Book ID is Required!"});
            }
            RahulBook b = await db.RahulBooks.FindAsync(id);
            if(b!=null){
                return Ok(b);
            }
            return NotFound(new {Message = "Book not found!"});
        }

        //ADMIN SPECIFIC ROUTES
    
        [HttpPost]
        [Route("api/books/admin/addBook", Name="AddABook")]
        //Here i stuck for 30 mins, data was passing in url so luckily figured out how to solve it
        public async Task<ActionResult> AddBook([FromBody] RahulBook? b){
            if(b == null){
                return BadRequest(new {Message = "Please Provide all the necessary details!"});
            }
            if(ModelState.IsValid){
                db.RahulBooks.Add(b);
                await db.SaveChangesAsync();
                return Ok(b);
            }
            else{
                return BadRequest(new {Message = "Please Provide details in valid form!"});
            }
        }

        [HttpPut]
        [Route("api/books/admin/editBook/{id:int}", Name="EditABook")]
        public async Task<ActionResult> EditBook(int? id,[FromBody] RahulBook? g){
            
            if(id == null) return BadRequest(new {Message = "Book ID is Required!"});
            RahulBook b = await db.RahulBooks.FindAsync(id);
            if(b == null){
                return BadRequest(new {Message = "Book not found!"});
            }
                b.BookName = g.BookName;
                b.BookAuthor = g.BookAuthor;
                b.BookQty = g.BookQty;
                db.RahulBooks.Update(b);
                await db.SaveChangesAsync();
            return Ok(b);
        }

        [HttpDelete]
        [Route("api/books/admin/deleteBook/{id:int}", Name="DeleteABook")]
        public async Task<ActionResult> DeleteBook(int? id){
            
            if(id == null) return BadRequest(new {Message = "Book ID is Required!"});
                RahulBook b = await db.RahulBooks.FindAsync(id);
            if(b!=null){
                db.RahulBooks.Remove(b);
                await db.SaveChangesAsync();
            }
            else{
                return NotFound(new {Message = "Book not found!"});
            }
            return NoContent();
        }
        [HttpPost]
        [Route("api/books/search", Name = "SearchBooks")]
        //Continous words should be present in DB -> you cant skip//
        // Although we can requset for all books and filter in client side
        public ActionResult searchBooks([FromBody] string? keyword){
            
            if(keyword == null) return BadRequest(new {Message = "Search Query is Required!"});
                var result = db.RahulBooks.Where(x => x.BookName.Contains(keyword)).Select(x => x).ToList();
            if(result.Count() != 0){
                return Ok(result);
            }
            else{
                return NotFound(new {Message = $"Books Not Found related to {keyword}"});
            }
        }
    }
}
