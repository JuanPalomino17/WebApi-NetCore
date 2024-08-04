using ApiVehiculos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiVehiculos.Context;

namespace ApiVehiculos.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class VehiculoController : Controller
    {
        private readonly AppDbContext _context;
        private Vehiculo vehiculos;

        public VehiculoController(AppDbContext context)
        {
            _context = context;
        }

        //Función para Obtener Registros
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vehiculo>>> GetVehiculos()
        {
            return await _context.Vehiculos.ToListAsync();
        }
        //Función para Obtener Registros por Id
        [HttpGet("{id}")]
        public async Task<ActionResult<Vehiculo>> GetVehiculo(int id)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);

            if (vehiculo == null)
            {
                return NotFound();
            }
            return vehiculo;
        }

        //Función para Agregar un Registro
        [HttpPost]
        public async Task<ActionResult<Vehiculo>> PostVehiculo(Vehiculo vehiculo)
        {
            _context.Vehiculos.Add(vehiculo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVehiculos), new { id = vehiculo.Id }, vehiculo);
        }

        //Función para Actualizar un Registro
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVehiculo(int id, Vehiculo vehiculo)
        {
            if (id != vehiculo.Id)
            {
                return BadRequest();
            }

            _context.Entry(vehiculo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehiculosExists(id))
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
        //Función de Eliminar un Registro
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var vehicle = await _context.Vehiculos.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            _context.Vehiculos.Remove(vehiculos);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VehiculosExists(int id)
        {
            return _context.Vehiculos.Any(e => e.Id == id);
        }
    }
}
