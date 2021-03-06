﻿using System;
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
        [MinLength(5, ErrorMessage = "Mínimo 5 caracteres.")]
        [MaxLength(15, ErrorMessage = "Límite: 15 caracteres.")]
        [RegularExpression("^[A-Za-z0-9_]*[A-Za-z0-9][A-Za-z0-9_]*$", ErrorMessage = "Permitido: minúsculas, mayúsculas, nº y _")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Ingresar dirección de correo.")]
        [MaxLength(100, ErrorMessage = "Límite: 100 caracteres.")]
        [RegularExpression(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*@((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$", ErrorMessage = "Correo no válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Ingresar una contraseña.")]
        [MinLength(6, ErrorMessage = "Al menos 6 caracteres.")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).+$", ErrorMessage = "Al menos: minúscula, mayúscula y número.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Volver a ingresar la contraseña.")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginDTO
    {
        [Required(ErrorMessage = "Ingresar usuario o correo.")]
        public string EmailOrUsername { get; set; }

        [Required(ErrorMessage = "Ingresar contraseña.")]        
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}