using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_Layer.DTO
{
    public class HashtagDTO
    {
        public string Name { get; set; }
        public List<PostSectionDTO> PostSection { get; set; }

        public HashtagDTO()
        {
            this.PostSection = new List<PostSectionDTO>();
        }
    }
}
