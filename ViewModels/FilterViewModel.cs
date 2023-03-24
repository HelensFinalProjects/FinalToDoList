using Microsoft.AspNetCore.Mvc.Rendering;
using FinalToDoList.Models;

namespace FinalToDoList.ViewModels
{
    public class FilterViewModel
    {
        public IEnumerable<MyTask> MyTasks { get; set; }
        public SelectList Categories { get; set; }
        public SelectList Statuses { get; set; }
        public PageViewModel PageViewModel { get; set; }
    }
}
