using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_Layer
{
    public class SessionInformation
    {
        public int PersonID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string NickName { get; set; }
        public string ProfileAvatar { get; set; }        
        public string ProfileHeader { get; set; }        
    }
}
