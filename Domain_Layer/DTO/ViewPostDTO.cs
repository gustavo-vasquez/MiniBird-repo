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
        public IsReplyDTO ReplyData { get; set; }
        public bool IsReply { get; set; }

        public ViewPostDTO()
        {
            this.PostSection = new PostSectionDTO();
            this.RepliesToPost = new List<PostSectionDTO>();
            this.ReplyData = new IsReplyDTO();
        }
    }

    public class FullViewPostDTO : ViewPostDTO
    {
        public ProfileInformationDTO ProfileInformation { get; set; }
        
        public FullViewPostDTO()
        {
            this.ProfileInformation = new ProfileInformationDTO();
        }
    }

    public class IsReplyDTO
    {
        public int ToProfile { get; set; }
        public string ToUsername { get; set; }
        public int ToPost { get; set; }
    }
}