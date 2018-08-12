using Domain_Layer;
using Domain_Layer.DTO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Data_Layer
{
    public class AccountDL
    {
        const string defaultAvatar = "/Content/images/defaultAvatar.png";
        const string defaultHeader = "/Content/images/defaultHeader.jpg";

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
                using (var context = new MiniBirdEntities())
                {
                    var person = context.Person.Where(p => p.Email == email).First();

                    return new SessionInformation()
                    {
                        PersonID = person.PersonID,
                        UserName = person.UserName,
                        Email = person.Email,
                        NickName = person.NickName,
                        ProfileAvatar = (person.ProfileAvatar != null) ? ByteArrayToBase64(person.ProfileAvatar, person.ProfileAvatar_MimeType) : defaultAvatar,
                        ProfileHeader = (person.ProfileHeader != null) ? ByteArrayToBase64(person.ProfileHeader, person.ProfileHeader_MimeType) : defaultHeader
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
                        ProfileAvatar = (person.ProfileAvatar != null) ? ByteArrayToBase64(person.ProfileAvatar, person.ProfileAvatar_MimeType) : defaultAvatar,
                        ProfileHeader = (person.ProfileHeader != null) ? ByteArrayToBase64(person.ProfileHeader, person.ProfileHeader_MimeType) : defaultHeader
                    };
                }
            }
            catch
            {
                throw;
            }
        }

        public bool CreateNewPostDL(string comment, byte[] gifImage, byte[] videoFile, string[] imagesUploaded, int personID, int? inReplyTo)
        {
            //try
            //{
                using (var context = new MiniBirdEntities())
                {
                    if(context.Person.Any(p => p.PersonID == personID))
                    {
                        var post = new Post();
                        post.Comment = comment;
                        if (imagesUploaded != null && imagesUploaded.Length != 0)
                        {
                            for(int i = 0; i < imagesUploaded.Length; i++)
                            {
                                switch(i)
                                {
                                    case 0: post.ImageFirstSlot = imgBase64ToByteArray(imagesUploaded[0]);
                                        break;
                                    case 1: post.ImageSecondSlot = imgBase64ToByteArray(imagesUploaded[1]);
                                        break;
                                    case 2: post.ImageThirdSlot = imgBase64ToByteArray(imagesUploaded[2]);
                                        break;
                                    case 3: post.ImageFourthSlot = imgBase64ToByteArray(imagesUploaded[3]);
                                        break;
                                }
                            }
                        }
                        post.PublicationDate = DateTime.Now;
                        if(inReplyTo != null && inReplyTo != 0)
                            post.InReplyTo = inReplyTo;
                        post.ID_Person = personID;
                        context.Post.Add(post);
                        context.SaveChanges();

                        return true;
                    }

                    return false;
                }
            //}
            //catch
            //{
            //    return false;
            //}
        }

        public TimelineDTO TimelineCollectionDataDL(int personID)
        {
            try
            {
                using(var context = new MiniBirdEntities())
                {
                    var person = context.Person.Where(p => p.PersonID == personID).First();
                    var post = context.Post.Where(ps => ps.ID_Person == personID).ToList();                    

                    var timelineDTO = new TimelineDTO();
                    timelineDTO.ProfileSection.UserName = person.UserName;
                    timelineDTO.ProfileSection.NickName = person.NickName;
                    timelineDTO.ProfileSection.ProfileHeader = (person.ProfileHeader != null) ? ByteArrayToBase64(person.ProfileHeader, person.ProfileHeader_MimeType) : defaultHeader;
                    timelineDTO.ProfileSection.ProfileAvatar = (person.ProfileAvatar != null) ? ByteArrayToBase64(person.ProfileAvatar, person.ProfileAvatar_MimeType) : defaultAvatar;
                    timelineDTO.ProfileSection.PostCount = post.Count();

                    return timelineDTO;
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

        private string ByteArrayToBase64(byte[] profileAvatar, string mimeType)
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

        private byte[] imgBase64ToByteArray(string imgBase64)
        {
            const string word = "base64,";
            int start = imgBase64.IndexOf(word) + word.Length;
            imgBase64 = imgBase64.Substring(start);

            return Convert.FromBase64String(imgBase64);
        }

        public string TemporaryPostImageDL(HttpPostedFile tempImage, HttpServerUtilityBase localServer, int personID)
        {
            try
            {
                if (tempImage == null)
                    throw new ArgumentNullException("Debe elegir una imágen.");

                string imgDir;
                using (var context = new MiniBirdEntities())
                {
                    imgDir = "/Content/images/temporary" + context.Person.Where(p => p.PersonID == personID).Single().UserName;
                }

                Directory.CreateDirectory(localServer.MapPath(imgDir));
                string imgFullPath = localServer.MapPath(imgDir) + "/" + tempImage.FileName;

                // Get file data
                byte[] data = new byte[] { };
                using (var binaryReader = new BinaryReader(tempImage.InputStream))
                {
                    data = binaryReader.ReadBytes(tempImage.ContentLength);
                }

                // Guardar imagen en el servidor
                using (FileStream image = File.Create(imgFullPath, data.Length))
                {
                    image.Write(data, 0, data.Length);
                }

                // Verifica si la imágen cumple las condiciones de validación
                const int _maxSize = 2 * 1024 * 1024;
                const int _maxWidth = 1000;
                const int _maxHeight = 1000;
                List<string> _fileTypes = new List<string>() { "jpg", "jpeg", "gif", "png" };
                string fileExt = Path.GetExtension(tempImage.FileName);

                if (new FileInfo(imgFullPath).Length > _maxSize)
                    throw new FormatException("El avatar no debe superar los 2mb.");

                if (!_fileTypes.Contains(fileExt.Substring(1), StringComparer.OrdinalIgnoreCase))
                    throw new FormatException("Para el avatar solo se admiten imágenes JPG, JPEG, GIF Y PNG.");

                using (Image img = Image.FromFile(imgFullPath))
                {
                    if (img.Width > _maxWidth || img.Height > _maxHeight)
                        throw new FormatException("El avatar admite hasta una resolución de 1000x1000.");
                }

                return imgDir + "/" + tempImage.FileName;
            }
            catch
            {
                throw;
            }
        }

        #endregion
    }
}
