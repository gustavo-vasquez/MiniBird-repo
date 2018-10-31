using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_Layer.DTO
{
    public class ListScreenDTO
    {
        public ListDTO CurrentListSection { get; set; }
        public List<ListDTO> MyListsSection { get; set; }
        public List<PostSectionDTO> PostSection { get; set; }
        public bool CanEdit { get; set; }

        public ListScreenDTO()
        {
            this.CurrentListSection = new ListDTO();
            this.MyListsSection = new List<ListDTO>();
            this.PostSection = new List<PostSectionDTO>();
        }
    }    
}