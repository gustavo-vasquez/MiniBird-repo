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

        public ProfileScreenDTO()
        {
            this.ProfileInformation = new ProfileInformationDTO();
            this.TopTrendings = new List<TopTrendingsDTO>();
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
}
