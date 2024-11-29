using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tienda.Data;
using Tienda.Models;

namespace Tienda.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ApplicationDbContext context, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] UserRegisterRequest model)
        {
            try
            {
                _logger.LogInformation("Iniciando registro para usuario: {Username}", model.Username);

                if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
                {
                    return BadRequest(new { success = false, message = "Usuario y contraseña son requeridos" });
                }

                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
                if (existingUser != null)
                {
                    return BadRequest(new { success = false, message = "El usuario ya existe" });
                }

                var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
                var user = new User
                {
                    Username = model.Username,
                    PasswordHash = passwordHash
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Usuario registrado exitosamente: {Username}", model.Username);

                var token = GenerateToken(user);
                return Ok(new { success = true, message = "Usuario registrado exitosamente", token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar usuario: {Username}", model?.Username);
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UserLoginRequest model)
        {
            try
            {
                _logger.LogInformation("Intento de login para usuario: {Username}", model.Username);

                if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
                {
                    return BadRequest(new { success = false, message = "Usuario y contraseña son requeridos" });
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
                if (user == null)
                {
                    return BadRequest(new { success = false, message = "Usuario o contraseña incorrectos" });
                }

                var isValidPassword = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
                if (!isValidPassword)
                {
                    return BadRequest(new { success = false, message = "Usuario o contraseña incorrectos" });
                }

                _logger.LogInformation("Login exitoso para usuario: {Username}", model.Username);

                var token = GenerateToken(user);
                return Ok(new { success = true, message = "Login exitoso", token,
                    username = user.Username
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en login para usuario: {Username}", model?.Username);
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        private string GenerateToken(User user)
        {
            try
            {
                var tokenKey = _configuration["AppSettings:Token"];
                if (string.IsNullOrEmpty(tokenKey))
                {
                    throw new InvalidOperationException("Token key no encontrada en la configuración");
                }

                // Aseguramos que la clave tenga el tamaño correcto
                while (Encoding.UTF8.GetBytes(tokenKey).Length < 64)
                {
                    tokenKey += tokenKey;
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // Cambiamos a SHA256

                var claims = new[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(1),
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar token para usuario: {Username}", user.Username);
                throw new InvalidOperationException("Error al generar el token de autenticación");
            }
        }
    }
}