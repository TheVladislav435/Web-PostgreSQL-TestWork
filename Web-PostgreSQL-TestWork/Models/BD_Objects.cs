using System.ComponentModel.DataAnnotations;

namespace Web_PostgreSQL_TestWork.Models
{
    public class BD_Objects
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Вам нужно ввести ИНН")]
        public string INN { get; set; }

        [Required(ErrorMessage = "Вам нужно ввести Описание")]
        public string Description { get; set; }
    }
}