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
    public class BookingController : ControllerBase
    {
        private readonly ACE42023Context db;
        public BookingController( ACE42023Context _db)
        {
            db = _db;
        }

        [HttpPost]
        [Route("api/Bookings", Name = "CreateABooking")]
        //here i have to alter what i was recieving;
        public ActionResult CreateBooking([FromBody] RahulBooking? bk){
            if(bk.Bid == null || bk.Uid == null){
                return BadRequest(new {Message = "UserID and BookID are Required!"});
            }
            if(bk.Uid != null){
                //Decreasing a book quantity
                RahulBook b = db.RahulBooks.Find(bk.Bid);
                if(bk.Bid == null){
                    return NotFound(new {Message = "Book Not Found!"});
                }
                RahulUser user = db.RahulUsers.Where(x => x.Userid == bk.Uid).SingleOrDefault();
                if(bk.Uid == null){
                    return NotFound(new {Message = "User Not Found!"});
                }
                
                foreach (var item in db.RahulBookings)
                {
                    if(item.Bid == bk.Bid && item.Uid == bk.Uid){
                        return BadRequest(new {Message = "Book Already Exist!"});
                    }                
                }
                // checking if the book is now outofstock
                if((b.BookQty) == 0){
                    return BadRequest(new {Message = "Book is out of stock!"});
                }
                b.BookQty = b.BookQty - 1;
                db.RahulBooks.Update(b);                   
                    //adding new Booking
                if(ModelState.IsValid){
                    db.RahulBookings.Add(bk);
                    db.SaveChanges();
                }
                return Ok(bk);
            }
            else{
                return Unauthorized(new {Message = "Authentication Needed to Acess this route!"});
            }
        }
        [HttpGet]
        [Route("api/Bookings/{uid:int}", Name = "GetBookingForAUser")]

        public async Task<ActionResult> ViewBookings(int? uid){
            if(uid == null){
                return BadRequest(new {Message = "User ID is REQUIRED!"});
            } 
            var u = await db.RahulUsers.FindAsync(uid);
            if(u == null){
                return NotFound(new {Message = "User Not Found"});
            }else{
                var result = await (from o in db.RahulBookings.Include(x=>x.BidNavigation) where o.Uid == uid select o).ToListAsync();
                return Ok(result);
            }

        }

        [HttpDelete]
        [Route("api/Bookings/{id:int}", Name = "DeleteBookingByID")]

        public async Task<ActionResult> DeleteBooking(int? id){
            if(id == null){
                return BadRequest(new {Message = "Booking ID is Required"});
            }
            else{
                //Finding the booking
                RahulBooking? booking = await db.RahulBookings.FindAsync(id);
                if(booking == null){
                    return NotFound(new {Message = "Booking not found!"});
                }
                int? bookid = booking.Bid;
                
                //Delete the Booking
                db.RahulBookings.Remove(booking);
                
                //Increasing a book quantity
                RahulBook b = await db.RahulBooks.FindAsync(bookid);
                b.BookQty = b.BookQty + 1;
                db.RahulBooks.Update(b);
                await db.SaveChangesAsync();
                return NoContent();
            }
        }

    }
}
