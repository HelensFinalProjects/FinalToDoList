using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FinalToDoList.Models
{
    public class Subtask
    {
        [Key]
        public int SubtaskId { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Назва задачі")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Опис")]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Дата закінчення задачі")]
        public DateTime DeadLine { get; set; }
        [Required]
        public bool Done { get; set; }

        [Display(Name = "Підзадача")]
        public int MyTaskId { get; set; }
        [ForeignKey("MyTaskId")]
        public virtual MyTask MyTask { get; set; }
    }
}
