using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    public class UserBusiness
    {
        private readonly UserData _userData;
        private readonly ILogger<User> _logger;

        public UserBusiness(UserData userData, ILogger<User> logger)
        {
            _userData = userData;
            _logger = logger;
        }

        // Método para obtener todos los usuarios como DTOs
        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userData.GetAllAsync();
                var usersDTO = new List<UserDTO>();

                foreach (var user in users)
                {
                    usersDTO.Add(new UserDTO
                    {
                        UserId = user.UserId,
                        Username = user.Username,
                        Email = user.Email,
                        Password = user.Password
                    });
                }

                return usersDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de usuarios", ex);
            }
        }

        // Método para obtener un usuario por ID como DTO
        public async Task<UserDTO> GetUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un usuario con ID inválido: {UserId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del usuario debe ser mayor que cero");
            }

            try
            {
                var user = await _userData.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogInformation("No se encontró ningún usuario con ID: {UserId}", id);
                    throw new EntityNotFoundException("Usuario", id);
                }

                return new UserDTO
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    Password = user.Password
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con ID: {UserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el usuario con ID {id}", ex);
            }
        }

        // Método para crear un usuario desde un DTO
        public async Task<UserDTO> CreateUserAsync(UserDTO UserDto)
        {

            try
            {
                ValidateUser(UserDto);

                var user = new User
                {
                    Username = UserDto.Username,
                    Email = UserDto.Email,
                    Password = UserDto.Password
                };

                var userCreado = await _userData.CreateAsync(user);

                return new UserDTO
                {
                    UserId = userCreado.UserId,
                    Username = userCreado.Username,
                    Email= userCreado.Email,
                    Password = userCreado.Password
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo usuario: {Username}", UserDto?.Username ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el usuario", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateUser(UserDTO UserDto)
        {
            if (UserDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto usuario no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(UserDto.Username))
            {
                _logger.LogWarning("Se intentó crear/actualizar un usuario con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del usuario es obligatorio");
            }
        }
    }
}