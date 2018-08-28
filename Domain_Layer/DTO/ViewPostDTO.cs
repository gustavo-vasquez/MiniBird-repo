using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_Layer.DTO
{
    public class ViewPostDTO
    {
        public PostSectionDTO PostSection { get; set; }
        public List<PostSectionDTO> RepliesToPost { get; set; }

        public ViewPostDTO()
        {
            this.PostSection = new PostSectionDTO();
            this.RepliesToPost = new List<PostSectionDTO>();
        }
    }    
}
