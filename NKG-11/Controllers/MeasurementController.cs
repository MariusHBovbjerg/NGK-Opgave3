using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NGK_11.Data;
using NGK_11.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NGK_11.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Authorize]
    public class MeasurementController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MeasurementController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet, AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Measurement>>> GetMeasurements()
        {
            return await _context.Measurements.Include(m => m.LocationOfMeasurement).ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Measurement>> GetMeasurement(long id)
        {
            var Measurement = await _context.Measurements.Include(m => m.LocationOfMeasurement).Where(m => m.MeasurementID == id).FirstAsync();

            if (Measurement == null)
            {
                return NotFound();
            }

            return Measurement;
        }

        [HttpGet("new")]
        public async Task<ActionResult<IEnumerable<Measurement>>> GetMeasurement()
        {
            var Measurement = await _context.Measurements.Include(m => m.LocationOfMeasurement).OrderByDescending(m => m.MeasurementID).Take(3).ToListAsync();
            
            if (Measurement == null)
            {
                return NotFound();
            }

            return Measurement;
        }

        [HttpGet("filter/{filterdate}")]
        public async Task<ActionResult<IEnumerable<Measurement>>> GetMeasurement(DateTime filterdate)
        {
            var Measurement = await _context.Measurements.Include(m => m.LocationOfMeasurement).Where(m => m.TimeOfMeasurement.Year == filterdate.Year & m.TimeOfMeasurement.Month == filterdate.Month & m.TimeOfMeasurement.Day == filterdate.Day).ToListAsync();

            if (Measurement == null)
            {
                return NotFound();
            }

            return Measurement;
        }

        // GET: api/Products/5
        [HttpGet("{date1}/{date2}")]
        public async Task<ActionResult<IEnumerable<Measurement>>> GetMeasurement(DateTime date1, DateTime date2)
        {
            var Measurement = await _context.Measurements.Include(m => m.LocationOfMeasurement).Where(m => m.TimeOfMeasurement >= date1 & m.TimeOfMeasurement <= date2).ToListAsync();

            if (Measurement == null)
            {
                return NotFound();
            }

            return Measurement;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMeasurement(long id, Measurement measurement)
        {
            if (id != measurement.MeasurementID)
            {
                return BadRequest();
            }

            _context.Entry(measurement).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MeasurementExists(id))
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

        // POST: api/Products
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Measurement>> PostProduct(Measurement measurement)
        {
            _context.Measurements.Add(measurement);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMeasurement", new { id = measurement.MeasurementID }, measurement);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Measurement>> DeleteMeasurement(long id)
        {
            var measurement = await _context.Measurements.FindAsync(id);
            if (measurement == null)
            {
                return NotFound();
            }

            _context.Measurements.Remove(measurement);
            await _context.SaveChangesAsync();

            return measurement;
        }

        private bool MeasurementExists(long id)
        {
            return _context.Measurements.Any(e => e.MeasurementID == id);
        }
    }
}
