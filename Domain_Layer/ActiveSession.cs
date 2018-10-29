using Domain_Layer.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_Layer
{
    public static class ActiveSession
    {
        private static int _PersonID { get; set; }
        private static string _UserName { get; set; }
        private static string _Email { get; set; }
        private static string _NickName { get; set; }
        private static string _ProfileAvatar { get; set; }
        private static string _ProfileHeader { get; set; }
        public static Theme Theme { get; set; }
        public static bool IsAuthenticated { get; set; }

        public static void Fill(object session)
        {
            if (session != null && session is SessionInformation)
            {
                var active = (SessionInformation)session;                
                _PersonID = active.PersonID;
                _UserName = active.UserName;
                _Email = active.Email;
                _NickName = active.NickName;
                _ProfileAvatar = active.ProfileAvatar;
                _ProfileHeader = active.ProfileHeader;
                Theme = active.Theme;
                IsAuthenticated = true;
            }
        }

        public static void Clear()
        {            
            _PersonID = 0;
            _UserName = null;
            _Email = null;
            _NickName = null;
            _ProfileAvatar = null;
            _ProfileHeader = null;
            Theme = Theme.Light;
            IsAuthenticated = false;            
        }

        public static int GetPersonID()
        {
            return _PersonID;
        }

        public static string GetUserName()
        {
            return _UserName;
        }

        public static string GetEmail()
        {
            return _Email;
        }

        public static string GetNickName()
        {
            return _NickName;
        }

        public static string GetProfileAvatar()
        {
            return _ProfileAvatar;
        }

        public static string GetProfileHeader()
        {
            return _ProfileHeader;
        }        
    }
}
