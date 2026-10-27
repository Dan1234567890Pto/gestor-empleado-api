using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using MiWebAPI.Data;
using MiWebAPI.Models;

namespace MiWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiciosController : ControllerBase
    {
        private readonly ServicioData _servicioData;

        public ServiciosController(ServicioData servicioData)
        {
            _servicioData = servicioData;
        }

        // GET: api/Servicios
        // Devuelve TODOS los registros llamando a Servicio_ConsultarTodos
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var lista = await _servicioData.Lista();
            return Ok(lista);
        }

        // GET: api/Servicios/5
        // Devuelve UN SOLO registro por ID llamando a Servicio_ConsultarPorId
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var objeto = await _servicioData.ObtenerPorId(id);
            if (objeto == null)
                return NotFound("Registro no encontrado o inactivo.");

            return Ok(objeto);
        }

        
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Servicio objeto)
        {
            bool respuesta = await _servicioData.Crear(objeto);
            if (!respuesta)
                return BadRequest("No se pudo registrar el servicio.");

            return Ok(new { mensaje = "Servicio registrado correctamente." });
        }

        // PUT: api/Servicios/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Editar(int id, [FromBody] Servicio objeto)
        {
            objeto.IdServicio = id;
            bool respuesta = await _servicioData.Editar(objeto);
            if (!respuesta)
                return NotFound("No se encontró el registro para actualizar.");

            return Ok(new { mensaje = "Servicio actualizado correctamente." });
        }

        // DELETE: api/Servicios/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            if (id <= 0)
                return BadRequest("Debe proporcionar un ID válido.");

            bool respuesta = await _servicioData.Eliminar(id);
            if (!respuesta)
                return NotFound("No se encontró el registro para eliminar.");

            return Ok(new { mensaje = "Servicio dado de baja correctamente." });
        }
    }
}
