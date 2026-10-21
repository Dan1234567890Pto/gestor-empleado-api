using Microsoft.AspNetCore.Mvc;
using MiWebAPI.Data;
using MiWebAPI.Models;

namespace MiWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembresiasController : ControllerBase
    {
        private readonly MembresiaData _membresiaData;

        public MembresiasController(MembresiaData membresiaData)
        {
            _membresiaData = membresiaData;
        }

        // GET: api/Membresias              -> Devuelve TODOS
        // GET: api/Membresias?id=3         -> Devuelve SOLO el registro con ID = 3
        [HttpGet]
        public async Task<IActionResult> Obtener([FromQuery] int? id)
        {
            if (id.HasValue)
            {
                var objeto = await _membresiaData.ObtenerPorId(id.Value);
                if (objeto == null) return NotFound("Registro no encontrado o inactivo.");
                return Ok(objeto);
            }

            var lista = await _membresiaData.Lista();
            return Ok(lista);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Membresia objeto)
        {
            bool respuesta = await _membresiaData.Crear(objeto);
            if (!respuesta) return BadRequest("No se pudo registrar la membresía.");
            return Ok(new { mensaje = "Membresía registrada correctamente." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(int id, [FromBody] Membresia objeto)
        {
            objeto.IdMembresia = id;
            bool respuesta = await _membresiaData.Editar(objeto);
            if (!respuesta) return NotFound("No se encontró el registro para actualizar.");
            return Ok(new { mensaje = "Membresía actualizada correctamente." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            bool respuesta = await _membresiaData.Eliminar(id);
            if (!respuesta) return NotFound("No se encontró el registro para eliminar.");
            return Ok(new { mensaje = "Membresía dada de baja correctamente." });
        }
    }
}