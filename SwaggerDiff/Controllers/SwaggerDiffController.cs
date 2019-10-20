using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using SwaggerDiff.Models;
using SwaggerDiff.Services;

namespace SwaggerDiff.Controllers
{
  [Route("api/swaggerdiff")]
  [ApiController]
  public class SwaggerDiffController : ControllerBase
  {
    private readonly SwaggerDiffContext _context;
    private readonly ILogger _logger;

    public SwaggerDiffController(SwaggerDiffContext context, ILogger<SwaggerDiffController> logger)
    {
      _context = context;
      _logger = logger;
    }

    // GET: api/swaggerdiff
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SwaggerItem>>> GetSwaggerItems()
    {
      _logger.LogInformation("Send contents of in memory database in response");

      // Return every instance of service -> JSON we have stored
      return await _context.SwaggerItems.ToListAsync();
    }

    // GET: api/swaggerdiff/:serviceName (Exposed Webhook URL)
    [HttpGet("{serviceName}")]
    [HttpPost("{serviceName}")]
    public async Task<ActionResult<SwaggerItem>> GetSwaggerItem(string serviceName)
    {
        // Create instance of SwaggerServiceUrlManager
        SwaggerServiceUrlManager urlManager = new SwaggerServiceUrlManager();

        // Check if webhook called with valid service name
        if(!urlManager.ServiceNames.Contains(serviceName))
        {
            return BadRequest();
        }

        SwaggerItem swaggerItem = await _context.SwaggerItems.FindAsync(serviceName);

        // Fetch new swagger JSON document

        // Check to see if there is a change

        // If there is a change...

            // Send slack notification of API change

            // Update DB with new service JSON file
            /* 
            
                _context.Entry(swaggerItem).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    TODO: do something?
                }

              */

        return Ok();
    }
  }
}
