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
        public List<TopTrendingsDTO> TopTrendingsSection { get; set; }
        public List<PostSectionDTO> PostSection { get; set; }        

        public TimelineDTO()
        {
            this.ProfileSection = new ProfileSectionDTO();
            this.TopTrendingsSection = new List<TopTrendingsDTO>();
            this.PostSection = new List<PostSectionDTO>();            
        }
    }

    public class ProfileSectionDTO
    {
        public int PersonID { get; set; }
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
        public int? UseCount { get; set; }
    }

    public class PostSectionDTO
    {
        public int PostID { get; set; }
        public string Comment { get; set; }
        public string GIFImage { get; set; }
        public byte[] VideoFile { get; set; }
        public List<string> Thumbnails { get; set; }
        //public string ImageFirstSlot { get; set; }
        //public string ImageSecondSlot { get; set; }
        //public string ImageThirdSlot { get; set; }
        //public string ImageFourthSlot { get; set; }
        public DateTime PublicationDate { get; set; }
        public int InReplyTo { get; set; }
        public int ID_Person { get; set; }
        public int CreatedBy { get; set; }
        public string NickName { get; set; }
        public string UserName { get; set; }
        public string ProfileAvatar { get; set; }
        public InteractButtonsDTO InteractButtons { get; set; }
        public string RepostedBy { get; set; }

        public PostSectionDTO()
        {
            this.Thumbnails = new List<string>();
            this.InteractButtons = new InteractButtonsDTO();            
        }
    }

    public class InteractButtonsDTO
    {
        public int ReplysCount { get; set; }
        public int RepostsCount { get; set; }
        public int LikesCount { get; set; }
    }
}
