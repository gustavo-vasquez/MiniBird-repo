using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_Layer.DTO
{
    public class FollowingDTO
    {
        public int PersonID { get; set; }
        public string NickName { get; set; }
        public string UserName { get; set; }
        public string ProfileAvatar { get; set; }
        public string Description { get; set; }
        public int FollowingCount { get; set; }
        public int FollowersCount { get; set; }
        public bool Following { get; set; }
    }
}
