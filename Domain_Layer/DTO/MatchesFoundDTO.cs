using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_Layer.DTO
{
    public class MatchesFoundDTO
    {
        public List<HashtagMatchesDTO> hashtagMatches { get; set; }
        public List<ProfileMatchesDTO> profileMatches { get; set; }

        public MatchesFoundDTO()
        {
            this.hashtagMatches = new List<HashtagMatchesDTO>();
            this.profileMatches = new List<ProfileMatchesDTO>();
        }

        public class HashtagMatchesDTO
        {
            public int HashtagID { get; set; }
            public string HashtagName { get; set; }
        }

        public class ProfileMatchesDTO
        {
            public int PersonID { get; set; }
            public string NickName { get; set; }
            public string UserName { get; set; }
            public string ProfileAvatar { get; set; }
        }
    }
}
