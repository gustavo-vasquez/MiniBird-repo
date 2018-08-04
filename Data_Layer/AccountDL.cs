using Domain_Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Data_Layer
{
    public class AccountDL
    {
        public bool RegisterDL(string userName, string email, string password)
        {
            if(!AccountExists(userName, email))
            {
                using (var context = new MiniBirdEntities())
                {
                    var newPerson = new Person()
                    {
                        UserName = "@" + userName,
                        Email = email,
                        NickName = userName,
                        Password = password,
                        RegistrationDate = DateTime.Now
                    };
                    context.Person.Add(newPerson);
                    context.SaveChanges();
                    newPerson.PersonCryptID = EncryptToSHA256(newPerson.PersonID);
                    context.SaveChanges();

                    return true;
                }
            }

            return false;
        }

        public bool LoginDL(string email, string password)
        {
            using (var context = new MiniBirdEntities())
            {
                var person = context.Person.Where(p => p.Email == email && p.Password == password).FirstOrDefault();

                if(person != null)
                {
                    return true;
                }
            }                
                           
            return false;
        }

        public SessionInformation CreateSessionDL(string email)
        {
            try
            {
                const string defaultAvatar = "/Content/images/defaultAvatar.png";
                const string defaultHeader = "/Content/images/defaultHeader.jpg";

                using (var context = new MiniBirdEntities())
                {
                    var person = context.Person.Where(p => p.Email == email).First();

                    return new SessionInformation()
                    {
                        PersonID = person.PersonID,
                        UserName = person.UserName,
                        Email = person.Email,
                        NickName = person.NickName,
                        ProfileAvatar = (person.ProfileAvatar != null) ? ImageToBase64(person.ProfileAvatar, person.ProfileAvatar_MimeType) : defaultAvatar,
                        ProfileHeader = (person.ProfileAvatar != null) ? ImageToBase64(person.ProfileHeader, person.ProfileHeader_MimeType) : defaultHeader
                    };
                }
            }
            catch
            {
                throw;
            }
        }

        public SessionInformation CreateSessionFromCookieDL(string hash)
        {
            try
            {
                const string defaultAvatar = "/Content/images/defaultAvatar.png";
                const string defaultHeader = "/Content/images/defaultHeader.jpg";

                using (var context = new MiniBirdEntities())
                {
                    var person = context.Person.Where(p => p.PersonCryptID == hash).First();

                    return new SessionInformation()
                    {
                        PersonID = person.PersonID,
                        UserName = person.UserName,
                        Email = person.Email,
                        NickName = person.NickName,
                        ProfileAvatar = (person.ProfileAvatar != null) ? ImageToBase64(person.ProfileAvatar, person.ProfileAvatar_MimeType) : defaultAvatar,
                        ProfileHeader = (person.ProfileAvatar != null) ? ImageToBase64(person.ProfileHeader, person.ProfileHeader_MimeType) : defaultHeader
                    };
                }
            }
            catch
            {
                throw;
            }
        }

        #region TAREAS AUXILIARES

        private bool AccountExists(string userName, string email)
        {
            using (var context = new MiniBirdEntities())
            {
                return context.Person.Any(p => p.Email == email || p.UserName == userName);
            }
        }        

        private string ImageToBase64(byte[] profileAvatar, string mimeType)
        {
            return String.Concat("data:", mimeType, ";base64,", Convert.ToBase64String(profileAvatar));
        }

        public bool UserNameExistsDL(string username)
        {
            using (var context = new MiniBirdEntities())
            {
                return context.Person.Any(p => p.UserName == username);
            }
        }

        public string EncryptCookieValueDL(string email)
        {
            try
            {
                using (var context = new MiniBirdEntities())
                {
                    return EncryptToSHA256(context.Person.Where(p => p.Email == email).First().PersonID);
                }
            }    
            catch
            {
                throw;
            }        
        }

        private string EncryptToSHA256(int accountID)
        {
            try
            {
                HashAlgorithm hasher = null;
                StringBuilder hash = new StringBuilder();

                try
                {
                    hasher = new SHA256Managed();
                }
                catch
                {
                    hasher = new SHA256CryptoServiceProvider();
                }

                byte[] plainBytes = Encoding.UTF8.GetBytes(accountID.ToString());
                byte[] hashedBytes = hasher.ComputeHash(plainBytes);
                hasher.Clear();

                foreach (byte theByte in hashedBytes)
                {
                    hash.Append(theByte.ToString("x2"));
                }                                

                return hash.ToString();
            }
            catch
            {
                throw;
            }
        }

        #endregion
    }
}
