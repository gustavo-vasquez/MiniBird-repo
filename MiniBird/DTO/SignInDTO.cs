using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MiniBird.DTO
{
    public class SignInDTO
    {
        public RegisterDTO Register { get; set; }
        public LoginDTO Login { get; set; }        
    }

    public class RegisterDTO
    {
        [Required(ErrorMessage = "Ingresar un nombre de usuario.")]
        [RegularExpression("^[a-zA-Z0-9_]*(\\S)$", ErrorMessage = "Permitido: minúsculas, mayúsculas, nº y _")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Ingresar dirección de correo.")]
        [RegularExpression(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*@((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$", ErrorMessage = "Correo no válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Ingresar una contraseña.")]
        [MinLength(6, ErrorMessage = "Al menos 6 caracteres.")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).+$", ErrorMessage = "Al menos: minúscula, mayúscula y número.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Volver a ingresar la contraseña.")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; }

        public bool TermsAndConditions { get; set; }
    }

    public class LoginDTO
    {
        [Required(ErrorMessage = "Ingresar dirección de correo.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Ingresar contraseña.")]
        [MinLength(6, ErrorMessage = "Al menos 6 caracteres.")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).+$", ErrorMessage = "Al menos: minúscula, mayúscula y número.")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}