using Demo.DAL.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Demo.PL.ViewModels
{
    public class DepartmentViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Code Is Required!!")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Name Is Required!!")]
        [MaxLength(50)]
        public string Name { get; set; }
        public DateTime DateOfCreation { get; set; }

        //Navigational Property [Many ]
        public ICollection<Employee> Employees { get; set; } = new HashSet<Employee>();
    }
}
