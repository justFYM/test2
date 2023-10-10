using APILogin.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace APILogin.Controllers
{
    public class UsuariosController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ApplicationDbContext context;

        public UsuariosController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.context = context;
        }

        [HttpGet("usuarios")]
        public IActionResult GetUsuarios()
        {
            var usuarios = userManager.Users.ToList();
            return Ok(usuarios);
        }


     
            [HttpPost("login")]
            public async Task<IActionResult> Login([FromBody] LoginViewModel modelo)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Modelo no válido");
                }

            // Lógica de autenticación y validación del modelo
            var resultado = await signInManager.PasswordSignInAsync(modelo.Email, modelo.Password, modelo.AutoLogin, lockoutOnFailure: false);
            
            if (resultado.Succeeded)
            {
                var Usuario = await userManager.FindByEmailAsync(modelo.Email);
                var Datos = new
                {
                    UserId = Usuario.Id,
                    UserName = Usuario.UserName

                };
                return Ok(Datos);
            }
            else
            {
                return Unauthorized("Credenciales inválidas");
            }

            /*
            if (modelo.Email == "usuario@ejemplo.com" && modelo.Password == "contraseña")
            {
                return Ok("Autenticación exitosa");
            }
            else
            {
                return Unauthorized("Credenciales inválidas");
            }
            */
        }






        //
        [HttpPost("registrar-usuario")]
        public async Task<IActionResult> LoginExterno([FromBody] ExternalLoginModel externalLoginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Modelo no válido");
            }

            // Lógica de autenticación y validación del modelo
            var resultadoLoginExterno = await signInManager.ExternalLoginSignInAsync(externalLoginModel.Provider, externalLoginModel.ProviderKey, isPersistent: true,
                 bypassTwoFactor: true);


            if (resultadoLoginExterno.Succeeded)
            {

                var usuario = await userManager.FindByEmailAsync(externalLoginModel.Email);
                var Datos = new
                {
                    UserId = usuario.Id,
                    UserName = usuario.Email

                };
                return Ok(Datos);
            }
            else
            {
                var usuario = new IdentityUser { Email = externalLoginModel.Email, UserName = externalLoginModel.Email };
                var resultadoCrearUsuario = await userManager.CreateAsync(usuario);
                if (!resultadoCrearUsuario.Succeeded)
                {

                    return BadRequest();
                }
                //Se agrega el proveedor del login externo a la base de datos.
                UserLoginInfo userLoginInfo = new UserLoginInfo(externalLoginModel.Provider, externalLoginModel.ProviderKey, externalLoginModel.ProviderDisplayName);

                var resultadoAgregarLogin = await userManager.AddLoginAsync(usuario, userLoginInfo);
                if (resultadoAgregarLogin.Succeeded)
                {
                   
                    var Datos = new
                    {
                        UserId = usuario.Id,
                        UserName = usuario.Email

                    };
                    return Ok(Datos);
                  
                }
              
                return BadRequest();





                //return Unauthorized("Credenciales inválidas");
            }

            /*
            if (modelo.Email == "usuario@ejemplo.com" && modelo.Password == "contraseña")
            {
                return Ok("Autenticación exitosa");
            }
            else
            {
                return Unauthorized("Credenciales inválidas");
            }
            */
        }
        //




    }






    }

