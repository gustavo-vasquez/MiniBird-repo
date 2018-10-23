using Domain_Layer.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_Layer.DTO
{
    public class ListDTO
    {
        public int MyListID { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Máximo 100 caracteres")]
        public string Name { get; set; }

        [MaxLength(100, ErrorMessage = "Máximo 100 caracteres")]
        public string Description { get; set; }
        public Privacy Privacy { get; set; }
        public int MembersCount { get; set; }
    }

    public class CheckboxListsDTO : ListDTO
    {
        public bool PersonalList { get; set; }
    }
}
