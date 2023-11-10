using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task_1.Models;

namespace Task_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LuceneController : ControllerBase
    {
        private readonly LuceneService _luceneService;

        public LuceneController(LuceneService luceneService)
        {
            _luceneService = luceneService;
        }

        //[HttpPost("Index")]
        //public IActionResult IndexCustomers()
        //{
        //    _luceneService.IndexCustomers();
        //    return Ok();
        //}

      
        [HttpGet("SearchById")]
        public ActionResult<List<Customer_Information>> SearchById([FromQuery] int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest("Please provide a valid 'id' parameter as id cannot be empty !!");
            }

            List<Customer_Information> results = _luceneService.SearchById(id.Value);

            if (results.Count == 0)
            {
                return NotFound("Sorry!! No results found for the provided 'id'. ");
            }

            return Ok(results);
        }

        [HttpGet("SearchByName")]
        public ActionResult<List<Customer_Information>> SearchByName([FromQuery] string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Please the 'name' feild should not be empty!! Please fill it**");
            }

            List<Customer_Information> results = _luceneService.SearchByName(name);

            if (results.Count == 0)
            {
                return NotFound("Sorry!! The Name given does not exist. ");
            }

            return Ok(results);
        }

        [HttpGet("SearchByAddress")]
        public ActionResult<List<Customer_Information>> SearchByAddress([FromQuery] string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return BadRequest("Please provide a non-empty 'address' parameter.!!");
            }

            List<Customer_Information> results = _luceneService.SearchByAddress(address);

            if (results.Count == 0)
            {
                return NotFound("Sorry!! The address given does not exist.");
            }

            return Ok(results);
        }

        [HttpGet("SearchByEmail")]
        public ActionResult<List<Customer_Information>> SearchByEmail([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Please 'email' parameter must not be empty so as to give you a result!!.");
            }

            List<Customer_Information> results = _luceneService.SearchByEmail(email);

            if (results.Count == 0)
            {
                return NotFound("Sorry!! The provided 'email' does not exist.");
            }

            return Ok(results);
        }
    }
}
