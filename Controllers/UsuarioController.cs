using API_TCC.Database;
using API_TCC.Model;
using API_TCC.Repositories;
using API_TCC.Services;
using API_TCC.DTO;
using Microsoft.AspNetCore.Mvc;

namespace API_TCC.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly UsuarioService _usuarioService;
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuariosController(MyDbContext context, UsuarioService usuarioService, IUsuarioRepository usuarioRepository)
        {
            _context = context;
            _usuarioService = usuarioService;
            _usuarioRepository = usuarioRepository;

        }

        [HttpPost("login")]
        public IActionResult Login(UsuarioDTO usuarioDTO)
        {
            if (usuarioDTO.Senha == null || usuarioDTO.Login == null)
            {
                return BadRequest("Credenciais inválidas");
            }

            bool result = _usuarioService.ValidarLogin(usuarioDTO.Login, usuarioDTO.Senha);

            return result ? Ok(result) : BadRequest("Credenciais inválidas");

        }

        [HttpPost("criaLogin")]
        public IActionResult CriaLogin([FromBody] UsuarioDTO usuarioDTO)
        {
            _usuarioService.CriaLogin(usuarioDTO.NOME, usuarioDTO.Login, usuarioDTO.Senha);
            return CreatedAtAction(nameof(CriaLogin), null);
        }

        [HttpPut("alteraSenha")]

        public IActionResult AlteraSenha(UsuarioDTO usuarioDTO)
        {

            bool result = _usuarioService.VerificaLogin(usuarioDTO.Login);
            if (result == true)
            {
                _usuarioService.AlteraSenha(usuarioDTO.Login, usuarioDTO.Senha);
                return Ok();
            }
            else
            {
                return BadRequest();
            }


        }
        [HttpGet("verificaLogin")]
        public IActionResult VerificaLogin(UsuarioDTO usuarioDTO)
        {
            bool result = _usuarioService.VerificaLogin(usuarioDTO.Login);

            return result ? Ok(result) : BadRequest("Usuário Inexistente");
        }

    }



}


