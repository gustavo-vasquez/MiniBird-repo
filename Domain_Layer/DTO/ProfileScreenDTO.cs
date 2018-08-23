using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_Layer.DTO
{
    public class ProfileScreenDTO
    {
        public ProfileInformationDTO ProfileInformation { get; set; }
        public List<TopTrendingsDTO> TopTrendings { get; set; }
        public List<PostSectionDTO> PostsSection { get; set; }
        public StatisticsBar StatisticsBar { get; set; }
        public List<ListDTO> MyLists { get; set; }

        public ProfileScreenDTO()
        {
            this.ProfileInformation = new ProfileInformationDTO();
            this.TopTrendings = new List<TopTrendingsDTO>();
            this.PostsSection = new List<PostSectionDTO>();
            this.StatisticsBar = new StatisticsBar();
            this.MyLists = new List<ListDTO>();
        }
    }

    public class ProfileInformationDTO
    {
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string PersonalDescription { get; set; }
        public string WebSiteURL { get; set; }
        public DateTime? Birthdate { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string ProfileAvatar { get; set; }
        public string ProfileHeader { get; set; }
    }

    public class ProfileDetailsDTO
    {
        [Required(ErrorMessage = "Escribe algo")]
        public string PersonalDescription { get; set; }
        public string WebSiteURL { get; set; }
        public DateTime? Birthdate { get; set; }
    }

    public class StatisticsBar
    {
        public int PostsCount { get; set; }
        public int FollowingCount { get; set; }
        public int FollowersCount { get; set; }
        public int LikesCount { get; set; }
        public int ListsCount { get; set; }
    }
}
