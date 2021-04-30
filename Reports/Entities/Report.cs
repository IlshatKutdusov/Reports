using System.ComponentModel.DataAnnotations;

namespace Reports.Entities
{
    public class Report : BaseEntity
    {
        [Required(ErrorMessage = "У отчета не указан id пользователя!")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "У отчета не указан id файла!")]
        public int FileId { get; set; }

        [Required(ErrorMessage = "У отчета не указано имя!")]
        [StringLength(maximumLength: 30, MinimumLength = 3, ErrorMessage = "У отчета имя должно содержать от 3 до 30 символов!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "У отчета не указан путь!")]
        [StringLength(maximumLength: 30, MinimumLength = 3, ErrorMessage = "У отчета путь должен содержать от 3 до 30 символов!")]
        public string Path { get; set; }

        [Required(ErrorMessage = "У отчета не указан формат!")]
        [StringLength(maximumLength: 30, MinimumLength = 3, ErrorMessage = "У отчета путь должен содержать от 3 до 30 символов!")]
        public string Format { get; set; }

        [Required(ErrorMessage = "У отчета не указан размер!")]
        public int Size { get; set; }


        [Required(ErrorMessage = "У отчета нет файла!")]
        public File File { get; set; }
    }
}
