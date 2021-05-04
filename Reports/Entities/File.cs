using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Reports.Entities
{
    public class File : BaseEntity
    {
        [Required(ErrorMessage = "У файла не указан id пользователя!")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "У файла не указано имя!")]
        //[StringLength(maximumLength: 30, MinimumLength = 3, ErrorMessage = "У файла имя должно содержать от 3 до 30 символов!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "У файла не указан путь!")]
        //[StringLength(maximumLength: 30, MinimumLength = 3, ErrorMessage = "У файла путь должен содержать от 3 до 30 символов!")]
        public string Path { get; set; }

        [Required(ErrorMessage = "У файла не указан размер!")]
        public int Size { get; set; }

        public List<Report> Reports { get; set; }
    }
}
