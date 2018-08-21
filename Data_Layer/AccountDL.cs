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
            try
            {
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
                                    case 0:
                                        post.ImageFirstSlot = imgBase64ToByteArray(imagesUploaded[i]);
                                        post.ImageFirstSlot_MimeType = ExtractMimeType(imagesUploaded[i]);
                                        break;
                                    case 1:
                                        post.ImageSecondSlot = imgBase64ToByteArray(imagesUploaded[i]);
                                        post.ImageSecondSlot_MimeType = ExtractMimeType(imagesUploaded[i]);
                                        break;
                                    case 2:
                                        post.ImageThirdSlot = imgBase64ToByteArray(imagesUploaded[i]);
                                        post.ImageThirdSlot_MimeType = ExtractMimeType(imagesUploaded[i]);
                                        break;
                                    case 3:
                                        post.ImageFourthSlot = imgBase64ToByteArray(imagesUploaded[i]);
                                        post.ImageFourthSlot_MimeType = ExtractMimeType(imagesUploaded[i]);
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
            }
            catch
            {
                return false;
            }
        }

        public ProfileScreenDTO ProfileScreenCollectionDataDL(int personID, string v)
        {
            try
            {
                using(var context = new MiniBirdEntities())
                {
                    var person = context.Person.Where(p => p.PersonID == personID).First();                    

                    var profileScreenDTO = new ProfileScreenDTO();
                    profileScreenDTO.ProfileInformation.UserName = person.UserName;
                    profileScreenDTO.ProfileInformation.NickName = person.NickName;
                    profileScreenDTO.ProfileInformation.PersonalDescription = person.PersonalDescription;
                    profileScreenDTO.ProfileInformation.WebSiteURL = person.WebSiteURL;
                    profileScreenDTO.ProfileInformation.Birthdate = person.Birthdate;
                    profileScreenDTO.ProfileInformation.RegistrationDate = person.RegistrationDate;
                    profileScreenDTO.ProfileInformation.ProfileAvatar = (person.ProfileAvatar != null) ? ByteArrayToBase64(person.ProfileAvatar, person.ProfileAvatar_MimeType) : defaultAvatar;
                    profileScreenDTO.ProfileInformation.ProfileHeader = (person.ProfileHeader != null) ? ByteArrayToBase64(person.ProfileHeader, person.ProfileHeader_MimeType) : defaultHeader;
                    profileScreenDTO.StatisticsBar.PostsCount = context.Post.Where(ps => ps.ID_Person == person.PersonID).Count();
                    profileScreenDTO.StatisticsBar.FollowingCount = person.Person3.Count;
                    profileScreenDTO.StatisticsBar.FollowersCount = person.Person11.Count;
                    profileScreenDTO.StatisticsBar.LikesCount = context.LikePost.Where(lp => lp.ID_PersonThatLikesPost == person.PersonID).Count();
                    profileScreenDTO.StatisticsBar.ListsCount = context.MyList.Where(ml => ml.ID_Person == person.PersonID).Count();

                    switch(v)
                    {
                        case "following":
                            break;
                        case "followers":
                            break;
                        case "likes":
                            break;
                        case "lists":
                            break;
                        default:
                            var myPosts = context.Post.Where(mp => mp.ID_Person == personID && mp.InReplyTo == null).OrderByDescending(mp => mp.PublicationDate);

                            foreach (var post in myPosts)
                            {
                                profileScreenDTO.PostsSection.Add(new PostSectionDTO()
                                {
                                    PostID = post.PostID,
                                    Comment = post.Comment,
                                    GIFImage = post.GIFImage,
                                    VideoFile = post.VideoFile,
                                    ImageFirstSlot = ByteArrayToBase64(post.ImageFirstSlot, post.ImageFirstSlot_MimeType),
                                    ImageSecondSlot = ByteArrayToBase64(post.ImageSecondSlot, post.ImageSecondSlot_MimeType),
                                    ImageThirdSlot = ByteArrayToBase64(post.ImageThirdSlot, post.ImageThirdSlot_MimeType),
                                    ImageFourthSlot = ByteArrayToBase64(post.ImageFourthSlot, post.ImageFourthSlot_MimeType),
                                    PublicationDate = post.PublicationDate,
                                    CreatedBy = person.PersonID,
                                    NickName = person.NickName,
                                    UserName = person.UserName,
                                    ProfileAvatar = (person.ProfileAvatar != null) ? ByteArrayToBase64(person.ProfileAvatar, person.ProfileAvatar_MimeType) : defaultAvatar
                                });
                            }
                            break;
                    }

                    return profileScreenDTO;
                }
            }
            catch
            {
                throw;
            }
        }

        public ProfileDetailsDTO ChangeProfileDetailsDL(int personID)
        {
            try
            {
                using (var context = new MiniBirdEntities())
                {
                    var person = context.Person.Where(p => p.PersonID == personID).First();
                    var profileDetailsDTO = new ProfileDetailsDTO();
                    profileDetailsDTO.PersonalDescription = person.PersonalDescription;
                    profileDetailsDTO.WebSiteURL = person.WebSiteURL;
                    profileDetailsDTO.Birthdate = person.Birthdate;
                    context.SaveChanges();

                    return profileDetailsDTO;
                }
            }
            catch
            {
                throw;
            }
        }

        public void ChangeProfileDetailsDL(ProfileDetailsDTO data, int personID)
        {
            try
            {
                using(var context = new MiniBirdEntities())
                {
                    var person = context.Person.Where(p => p.PersonID == personID).First();
                    person.PersonalDescription = data.PersonalDescription;
                    person.WebSiteURL = data.WebSiteURL;
                    person.Birthdate = data.Birthdate;
                    context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public TimelineDTO TimelineCollectionDataDL(int personID)
        {
            try
            {
                using(var context = new MiniBirdEntities())
                {
                    var person = context.Person.Where(p => p.PersonID == personID).First();
                    var posts = context.Post.Where(ps => ps.ID_Person == personID && ps.InReplyTo == null).ToList();

                    foreach(var p in person.Person3)
                    {
                        var postsOfFollowing = context.Post.Where(ps => ps.ID_Person == p.PersonID && ps.InReplyTo == null).ToList();
                        if (postsOfFollowing.Count > 0)
                            posts.AddRange(postsOfFollowing);
                    }

                    var timelineDTO = new TimelineDTO();
                    timelineDTO.ProfileSection.UserName = person.UserName;
                    timelineDTO.ProfileSection.NickName = person.NickName;
                    timelineDTO.ProfileSection.ProfileHeader = (person.ProfileHeader != null) ? ByteArrayToBase64(person.ProfileHeader, person.ProfileHeader_MimeType) : defaultHeader;
                    timelineDTO.ProfileSection.ProfileAvatar = (person.ProfileAvatar != null) ? ByteArrayToBase64(person.ProfileAvatar, person.ProfileAvatar_MimeType) : defaultAvatar;
                    timelineDTO.ProfileSection.PostCount = posts.Count();
                    timelineDTO.ProfileSection.FollowerCount = person.Person11.Count;
                    timelineDTO.ProfileSection.FollowingCount = person.Person3.Count;

                    posts = posts.OrderByDescending(ps => ps.PublicationDate).ToList();

                    foreach (var post in posts)
                    {
                        var createdBy = context.Person.Where(p => p.PersonID == post.ID_Person).First();

                        timelineDTO.PostSection.Add(new PostSectionDTO()
                        {
                            PostID = post.PostID,
                            Comment = post.Comment,
                            GIFImage = post.GIFImage,
                            VideoFile = post.VideoFile,
                            ImageFirstSlot = ByteArrayToBase64(post.ImageFirstSlot, post.ImageFirstSlot_MimeType),
                            ImageSecondSlot = ByteArrayToBase64(post.ImageSecondSlot, post.ImageSecondSlot_MimeType),
                            ImageThirdSlot = ByteArrayToBase64(post.ImageThirdSlot, post.ImageThirdSlot_MimeType),
                            ImageFourthSlot = ByteArrayToBase64(post.ImageFourthSlot, post.ImageFourthSlot_MimeType),
                            PublicationDate = post.PublicationDate,
                            CreatedBy = createdBy.PersonID,
                            NickName = createdBy.NickName,
                            UserName = createdBy.UserName,
                            ProfileAvatar = (createdBy.ProfileAvatar != null) ? ByteArrayToBase64(createdBy.ProfileAvatar, createdBy.ProfileAvatar_MimeType) : defaultAvatar,
                            InteractButtons = GetInteractsCountDL(post.PostID)
                        });
                    }                    

                    return timelineDTO;
                }                
            }
            catch
            {
                throw;
            }
        }

        public string ChangeHeaderDL(HttpPostedFile img, int personID)
        {            
            using (var context = new MiniBirdEntities())
            {
                var person = context.Person.Where(p => p.PersonID == personID).First();
                var newHeader = PostedFileToByteArray(img);
                person.ProfileHeader = newHeader;
                person.ProfileHeader_MimeType = img.ContentType;
                context.SaveChanges();

                return ByteArrayToBase64(newHeader, img.ContentType);
            }
        }

        public string ChangeAvatarDL(HttpPostedFile img, int personID)
        {
            using (var context = new MiniBirdEntities())
            {
                var person = context.Person.Where(p => p.PersonID == personID).First();
                var newAvatar = PostedFileToByteArray(img);
                person.ProfileAvatar = newAvatar;
                person.ProfileAvatar_MimeType = img.ContentType;
                context.SaveChanges();

                return ByteArrayToBase64(newAvatar, img.ContentType);
            }
        }

        public InteractButtonsDTO GetInteractsCountDL(int postID)
        {
            using(var context = new MiniBirdEntities())
            {
                var replys = context.Post.Where(ps => ps.InReplyTo == postID).Count();
                var reposts = context.RePost.Where(rp => rp.ID_Post == postID).Count();
                var likes = context.LikePost.Where(lp => lp.ID_Post == postID).Count();

                return new InteractButtonsDTO()
                {
                    ReplysCount = replys,
                    RepostsCount = reposts,
                    LikesCount = likes
                };
            }
        }

        public void SendRepostDL(int postID, int personID)
        {
            using (var context = new MiniBirdEntities())
            {
                var repost = context.RePost.Where(lp => lp.ID_Post == postID && lp.ID_PersonThatRePost == personID).FirstOrDefault();

                if (repost != null)
                    context.RePost.Remove(repost);
                else
                    context.RePost.Add(new RePost() { ID_Post = postID, ID_PersonThatRePost = personID, PublicationDate = DateTime.Now });

                context.SaveChanges();
            }
        }

        public void GiveALikeDL(int postID, int personID)
        {
            using(var context = new MiniBirdEntities())
            {
                var like = context.LikePost.Where(lp => lp.ID_Post == postID && lp.ID_PersonThatLikesPost == personID).FirstOrDefault();

                if(like != null)                
                    context.LikePost.Remove(like);                                    
                else                
                    context.LikePost.Add(new LikePost() { ID_Post = postID, ID_PersonThatLikesPost = personID, DateOfAction = DateTime.Now });                                   

                context.SaveChanges();
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
            if(profileAvatar != null)
                return String.Concat("data:", mimeType, ";base64,", Convert.ToBase64String(profileAvatar));

            return null;
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

        public string ExtractMimeType(string imgBase64)
        {
            int end = imgBase64.IndexOf(';');
            return imgBase64.Substring(5, end - 5);
        }

        private byte[] PostedFileToByteArray(HttpPostedFile img)
        {
            MemoryStream ms = new MemoryStream();
            img.InputStream.CopyTo(ms);

            return ms.ToArray();
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

        //List<string> emailsperson1list = new List<string>();
        //List<string> emailsperson2list = new List<string>();

        //var person1list = person.Person1.ToList();

        //foreach (var p in person1list)
        //{
        //    emailsperson1list.Add(p.Email);
        //}

        //var person2list = person.Person2.ToList();

        //foreach (var p2 in person2list)
        //{
        //    emailsperson2list.Add(p2.Email);
        //}

        //List<int> blockedids = new List<int>();
        //var blocked = person.Person1.Union(person.Person2);
        //var blockedlist = blocked.ToList();

        //foreach (var b in blockedlist)
        //{
        //    blockedids.Add(b.PersonID);
        //}

        //var blocked2 = person.Person2.Union(person.Person1);

        //List<string> emailsperson11list = new List<string>();
        //List<string> emailsperson3list = new List<string>();

        //var person11list = person.Person11.ToList();

        //foreach (var p11 in person11list)
        //{
        //    emailsperson11list.Add(p11.Email);
        //}

        //var person3list = person.Person3.ToList();

        //foreach (var p3 in person3list)
        //{
        //    emailsperson3list.Add(p3.Email);
        //}

        //List<int> followids = new List<int>();
        //var follow = person.Person11.Union(person.Person3);
        //var followlist = follow.ToList();

        //foreach (var f in followlist)
        //{
        //    followids.Add(f.PersonID);
        //}

        //var follow = person.Person11.Union(person.Person3);
        //var follow2 = person.Person3.Union(person.Person11);

        //List<string> emailsperson12list = new List<string>();
        //List<string> emailsperson4list = new List<string>();

        //var person12list = person.Person12.ToList();

        //foreach (var p12 in person12list)
        //{
        //    emailsperson12list.Add(p12.Email);
        //}

        //var person4list = person.Person4.ToList();

        //foreach (var p4 in person4list)
        //{
        //    emailsperson4list.Add(p4.Email);
        //}

        //List<int> mutedids = new List<int>();                    
        //var muted = person.Person12.Union(person.Person4);
        //var mutedlist = muted.ToList();

        //foreach (var m in mutedlist)
        //{
        //    mutedids.Add(m.PersonID);
        //}

        //var muted = person.Person12.Union(person.Person4);
        //var muted2 = person.Person4.Union(person.Person12);
    }
}
