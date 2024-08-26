using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GroupSpace23.ApiModels;
using GroupSpace23.Data;

namespace GroupSpace23.ApiController
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginModelsController : ControllerBase
    {
        private readonly MyDbContext _context;

        public LoginModelsController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/LoginModels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoginModel>>> GetLoginModel()
        {
            return await _context.LoginModel.ToListAsync();
        }

        // GET: api/LoginModels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LoginModel>> GetLoginModel(int id)
        {
            var loginModel = await _context.LoginModel.FindAsync(id);

            if (loginModel == null)
            {
                return NotFound();
            }

            return loginModel;
        }

        // PUT: api/LoginModels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLoginModel(int id, LoginModel loginModel)
        {
            if (id != loginModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(loginModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoginModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // LOGIN
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            try
            {
                // Find the user by username
                var user = await _context.LoginModel.FirstOrDefaultAsync(u => u.Name == loginModel.Name);
                if (user == null)
                {
                    return Unauthorized("Invalid username or password");
                }

                // Check the password (you should hash passwords in a real application)
                if (user.Password != loginModel.Password)
                {
                    return Unauthorized("Invalid username or password");
                }

                // For now, return a success message (in production, consider issuing a JWT token)
                return Ok("Login successful");
            }
            catch (Exception ex)
            {
                // Log the exception (use a logging framework in production)
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        // POST: api/LoginModels
        [HttpPost]
        public async Task<ActionResult<LoginModel>> PostLoginModel(LoginModel loginModel)
        {
            _context.LoginModel.Add(loginModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLoginModel", new { id = loginModel.Id }, loginModel);
        }

        // DELETE: api/LoginModels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLoginModel(int id)
        {
            var loginModel = await _context.LoginModel.FindAsync(id);
            if (loginModel == null)
            {
                return NotFound();
            }

            _context.LoginModel.Remove(loginModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LoginModelExists(int id)
        {
            return _context.LoginModel.Any(e => e.Id == id);
        }
    }
}
