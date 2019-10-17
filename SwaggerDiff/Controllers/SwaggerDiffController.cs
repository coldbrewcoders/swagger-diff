using System.Collections.Generic;
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
      SwaggerItem swaggerItem = await _context.SwaggerItems.FindAsync(id);

      if (swaggerItem == null) return NotFound();

      return swaggerItem;
    }

    // PUT: api/swaggerdiff/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutSwaggerItem(long id, SwaggerItem swaggerItem)
    {
      // Check if swaggerItem is a valid SwaggerItem model
      if (ModelState.IsValid) return BadRequest();

      // Find the swaggerItem from SwaggerItems context (returns null by default)
      SwaggerItem item = await _context.SwaggerItems.FirstOrDefaultAsync(s => s.Id == id);

      if (item != null)
      {
        item.ServiceName = swaggerItem.ServiceName;
        item.ServiceJSON = swaggerItem.ServiceJSON;
      }

      try
      {
        // Update the DB with in memory context DB changes
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException ex)
      {
        return BadRequest(ex.Message);
      }

      return NoContent();
    }

    // POST: api/swaggerdiff
    [HttpPost]
    public async Task<ActionResult<SwaggerItem>> PostSwaggerItem(SwaggerItem swaggerItem)
    {
      await _context.SwaggerItems.AddAsync(swaggerItem);

      await _context.SaveChangesAsync();

      return CreatedAtAction(nameof(GetSwaggerItem), new { id = swaggerItem.Id }, swaggerItem);
    }

    // DELETE: api/swaggerdiff/5
    [HttpDelete("{id}")]
    public async Task<ActionResult<SwaggerItem>> DeleteSwaggerItem(long id)
    {
      SwaggerItem swaggerItem = await _context.SwaggerItems.FindAsync(id);

      if (swaggerItem == null) return NotFound();

      _context.SwaggerItems.Remove(swaggerItem);
      await _context.SaveChangesAsync();

      return Ok();
    }
  }
}
