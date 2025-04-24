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
    public class ActivityController : ControllerBase
    {
        private readonly ActivityBusiness _ActivityBusiness;
        private readonly ILogger<ActivityController> _logger;

        /// <summary>
        /// Constructor del controlador de actividades
        /// </summary>
        public ActivityController(ActivityBusiness ActivityBusiness, ILogger<ActivityController> logger)
        {
            _ActivityBusiness = ActivityBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las actividades del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ActivityDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllActivity()
        {
            try
            {
                var activitys = await _ActivityBusiness.GetAllActivitysAsync();
                return Ok(activitys);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener actividades");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una actividad específica por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ActivityDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetActivityById(int id)
        {
            try
            {
                var activity = await _ActivityBusiness.GetActivityByIdAsync(id);
                return Ok(activity);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para actividad con ID: {ActivityId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Actividad no encontrado con ID: {ActivityId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener actividad con ID: {ActivityId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva actividad en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(FormDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateDestination([FromBody] ActivityDTO ActivityDto)
        {
            try
            {
                var createdActivity = await _ActivityBusiness.CreateActivityAsync(ActivityDto);
                return CreatedAtAction(nameof(GetActivityById), new { id = createdActivity.ActivityId }, createdActivity);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear actividad");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear actividad");
                return StatusCode(500, new { message = ex.Message });
            }
            }
        }
    }