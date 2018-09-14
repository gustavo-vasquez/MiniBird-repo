using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_Layer.DTO
{
    public class SearchDTO
    {
        [Required(ErrorMessage = "¿Que estás buscando?")]
        public string WordToSearch { get; set; }
        public MatchesFoundDTO MatchesFound { get; set; }
    }
}
