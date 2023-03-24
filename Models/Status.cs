using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FinalToDoList.Models
{
    public class Status
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Статус")]
        public string Name { get; set; }

        public virtual List<MyTask> MyTasks { get; set; }
    }
}
