using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_Layer.DTO
{
    public class TimelineDTO
    {
        public ProfileSectionDTO ProfileSection { get; set; }
        public TopTrendingsDTO TopTrendingsSection { get; set; }
        public PostSectionDTO PostSection { get; set; }
    }

    public class ProfileSectionDTO
    {
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string ProfileAvatar { get; set; }
        public string ProfileHeader { get; set; }
        public int PostCount { get; set; }
        public int FollowingCount { get; set; }
        public int FollowerCount { get; set; }
    }

    public class TopTrendingsDTO
    {
        public string Name { get; set; }
        public int UsageCount { get; set; }
    }

    public class PostSectionDTO
    {
        public int PostID { get; set; }
        public string Comment { get; set; }
        public byte[] GIFImage { get; set; }
        public byte[] VideoFile { get; set; }
        public string ImageFirstSlot { get; set; }
        public string ImageSecondSlot { get; set; }
        public string ImageThirdSlot { get; set; }
        public string ImageFourthSlot { get; set; }
        public DateTime PublicationDate { get; set; }
        public int InReplyTo { get; set; }
        public int ID_Person { get; set; }
    }
}
