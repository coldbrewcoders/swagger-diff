using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwaggerDiff.Models;

namespace SwaggerDiff.Controllers
{
  [Route("api/swaggerdiff")]
  [ApiController]
  public class SwaggerDiffController : ControllerBase
  {
    private readonly SwaggerDiffContext _context;

    public SwaggerDiffController(SwaggerDiffContext context)
    {
      _context = context;
    }

    // GET: api/swaggerdiff
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SwaggerItem>>> GetSwaggerItems()
    {
      return await _context.SwaggerItems.ToListAsync();
    }

    // GET: api/swaggerdiff/5
    [HttpGet("{id}")]
    public async Task<ActionResult<SwaggerItem>> GetSwaggerItem(long id)
    {
      var swaggerItem = await _context.SwaggerItems.FindAsync(id);

      if (swaggerItem == null) return NotFound();

      return swaggerItem;
    }

    // PUT: api/swaggerdiff/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for
    // more details see https://aka.ms/RazorPagesCRUD.
    [HttpPut("{id}")]
    public async Task<IActionResult> PutSwaggerItem(long id, SwaggerItem swaggerItem)
    {
      if (id != swaggerItem.Id) return BadRequest();

      _context.Entry(swaggerItem).State = EntityState.Modified;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!SwaggerItemExists(id)) {
          return NotFound();
        } else {
          throw;
        }
      }

      return NoContent();
    }

    // POST: api/swaggerdiff
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for
    // more details see https://aka.ms/RazorPagesCRUD.
    [HttpPost]
    public async Task<ActionResult<SwaggerItem>> PostSwaggerItem(SwaggerItem swaggerItem)
    {
      _context.SwaggerItems.Add(swaggerItem);

      await _context.SaveChangesAsync();

      return CreatedAtAction(nameof(GetSwaggerItem), new { id = swaggerItem.Id }, swaggerItem);
    }

    // DELETE: api/swaggerdiff/5
    [HttpDelete("{id}")]
    public async Task<ActionResult<SwaggerItem>> DeleteSwaggerItem(long id)
    {
      var swaggerItem = await _context.SwaggerItems.FindAsync(id);

      if (swaggerItem == null) return NotFound();

      _context.SwaggerItems.Remove(swaggerItem);
      await _context.SaveChangesAsync();

      return swaggerItem;
    }

    private bool SwaggerItemExists(long id)
    {
      return _context.SwaggerItems.Any(e => e.Id == id);
    }
  }
}
