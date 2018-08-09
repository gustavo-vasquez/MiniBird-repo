using Data_Layer;
using Domain_Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Service_Layer
{
    public class AccountSL
    {
        static AccountDL Account = new AccountDL();

        public bool RegisterSL(string userName, string email, string password)
        {
            return Account.RegisterDL(userName, email, password);
        }

        public bool LoginSL(string email, string password)
        {
            return Account.LoginDL(email, password);
        }

        public SessionInformation CreateSessionSL(string email)
        {
            return Account.CreateSessionDL(email);
        }

        public SessionInformation CreateSessionFromCookieSL(string hash)
        {
            return Account.CreateSessionFromCookieDL(hash);
        }


        #region TAREAS AUXILIARES

        public bool UserNameExistsSL(string username)
        {
            return Account.UserNameExistsDL(username);
        }

        public string EncryptCookieValueSL(string email)
        {
            return Account.EncryptCookieValueDL(email);
        }

        public string TemporaryPostImageSL(HttpPostedFile tempImage, HttpServerUtilityBase localServer, int personID)
        {
            return Account.TemporaryPostImageDL(tempImage, localServer, personID);
        }

        #endregion
    }
}
