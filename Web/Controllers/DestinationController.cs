using Business;
using Data;
using Entity.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de actividades en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class DestinationController : ControllerBase
    {
        private readonly DestinationBusiness _DestinationBusiness;
        private readonly ILogger<DestinationController> _logger;

        /// <summary>
        /// Constructor del controlador de formularios
        /// </summary>
        public DestinationController(DestinationBusiness DestinationBusiness, ILogger<DestinationController> logger)
        {
            _DestinationBusiness = DestinationBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los destinos del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FormDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllDestination()
        {
            try
            {
                var destinations = await _DestinationBusiness.GetAllDestinationsAsync();
                return Ok(destinations);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener destinos");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un destino específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FormDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetDestinationById(int id)
        {
            try
            {
                var form = await _DestinationBusiness.GetDestinationByIdAsync(id);
                return Ok(form);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para destino con ID: {DestinationId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Destino no encontrado con ID: {DestinationId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener destino con ID: {DestinationId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo destino en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(FormDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateDestination([FromBody] DestinationDTO DestinationDto)
        {
            try
            {
                var createdDestination = await _DestinationBusiness.CreateDestinationAsync(DestinationDto);
                return CreatedAtAction(nameof(GetDestinationById), new { id = createdDestination.DestinationId }, createdDestination);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear destino");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear destino");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}