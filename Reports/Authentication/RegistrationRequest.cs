using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Reports.Models
{
    public class RegistrationRequest
    {
        [Required(ErrorMessage = "У пользователя не указан логин!")]
        [StringLength(maximumLength: 30, MinimumLength = 3, ErrorMessage = "У пользователя логин должен содержать от 3 до 30 символов!")]
        public string Login { get; set; }

        [Required(ErrorMessage = "У пользователя не указан пароль!")]
        [StringLength(maximumLength: 30, MinimumLength = 3, ErrorMessage = "У пользователя пароль должен содержать от 3 до 30 символов!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "У пользователя не указан email!")]
        [StringLength(maximumLength: 30, MinimumLength = 3, ErrorMessage = "У пользователя email должен содержать от 3 до 30 символов!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "У пользователя не указано имя!")]
        [StringLength(maximumLength: 30, MinimumLength = 3, ErrorMessage = "У пользователя имя должно содержать от 3 до 30 символов!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "У пользователя не указана фамилия!")]
        [StringLength(maximumLength: 30, MinimumLength = 3, ErrorMessage = "У пользователя фамилия должна содержать от 3 до 30 символов!")]
        public string Surname { get; set; }
    }
}
