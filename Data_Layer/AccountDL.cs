using Domain_Layer;
using Domain_Layer.DTO;
using Domain_Layer.Enum;
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

        public bool CreateNewPostDL(NewPostDTO model, int personID, HttpServerUtilityBase localServer)
        {
            try
            {
                using (var context = new MiniBirdEntities())
                {
                    if(context.Person.Any(p => p.PersonID == personID))
                    {
                        var post = new Post();
                        post.Comment = model.Comment;                        
                        post.PublicationDate = DateTime.Now;
                        post.ID_Person = personID;

                        if (model.InReplyTo != null && model.InReplyTo > 0)
                            post.InReplyTo = model.InReplyTo;

                        List<Hashtag> hashtags = DiscoverHashtag(model.Comment, context);

                        if (hashtags.Count > 0)
                            foreach (var hashtag in hashtags)
                                post.Hashtag.Add(hashtag);

                        //var hash = context.Hashtag.First(h => h.Name == "#UnaFrase");
                        //post.Hashtag.Add(hash);
                        context.Post.Add(post);
                        context.SaveChanges();

                        if (model.ImagesUploaded != null && model.ImagesUploaded.Length > 0)
                        {
                            int iteration = 1;

                            foreach (string image in model.ImagesUploaded)
                            {
                                context.Thumbnail.Add(new Thumbnail() { ImagePath = SaveThumbnailInFolder(image, post.PostID, localServer, iteration), ID_Post = post.PostID });
                                context.SaveChanges();
                                iteration++;
                            }                            
                        }

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
                    profileScreenDTO.ProfileInformation.Birthdate = (person.Birthdate.HasValue) ? BirthDatePhrase(person.Birthdate) : "";
                    profileScreenDTO.ProfileInformation.RegistrationDate = person.RegistrationDate;
                    profileScreenDTO.ProfileInformation.ProfileAvatar = (person.ProfileAvatar != null) ? ByteArrayToBase64(person.ProfileAvatar, person.ProfileAvatar_MimeType) : defaultAvatar;
                    profileScreenDTO.ProfileInformation.ProfileHeader = (person.ProfileHeader != null) ? ByteArrayToBase64(person.ProfileHeader, person.ProfileHeader_MimeType) : defaultHeader;
                    profileScreenDTO.TopTrendings = TopTrendings();
                    profileScreenDTO.StatisticsBar.PostsCount = context.Post.Where(ps => ps.ID_Person == person.PersonID).Count();
                    profileScreenDTO.StatisticsBar.FollowingCount = person.Person3.Count;
                    profileScreenDTO.StatisticsBar.FollowersCount = person.Person11.Count;
                    profileScreenDTO.StatisticsBar.LikesCount = context.LikePost.Where(lp => lp.ID_PersonThatLikesPost == person.PersonID).Count();
                    profileScreenDTO.StatisticsBar.ListsCount = context.List.Where(ml => ml.ID_Person == person.PersonID).Count();

                    switch(v)
                    {
                        case "following":
                            break;
                        case "followers":
                            break;
                        case "likes":
                            break;
                        case "lists":
                            var myLists = context.List.Where(ml => ml.ID_Person == person.PersonID);

                            foreach(var list in myLists)
                            {
                                profileScreenDTO.MyLists.Add(new ListDTO()
                                {
                                    MyListID = list.ListID,
                                    Name = list.Name,
                                    Description = list.Description,
                                    Privacy = (list.IsPrivate != true) ? Privacy.Public : Privacy.Private,
                                    MembersCount = list.Person1.Count
                                });
                            }

                            break;
                        default:
                            var myPosts = context.Post.Where(mp => mp.ID_Person == personID && mp.InReplyTo == null).ToList();
                            var myReposts = context.RePost.Where(rp => rp.ID_PersonThatRePost == person.PersonID).ToList();                            

                            foreach (var repost in myReposts)
                            {
                                var postReposted = context.Post.Find(repost.ID_Post);
                                var createdBy = context.Person.Where(p => p.PersonID == postReposted.ID_Person).First();

                                profileScreenDTO.PostsSection.Add(new PostSectionDTO()
                                {
                                    PostID = postReposted.PostID,
                                    Comment = postReposted.Comment,
                                    GIFImage = postReposted.GIFImage,
                                    VideoFile = postReposted.VideoFile,
                                    Thumbnails = GetPostedThumbnails(postReposted.PostID),                                    
                                    PublicationDate = repost.PublicationDate,
                                    CreatedBy = createdBy.PersonID,
                                    NickName = createdBy.NickName,
                                    UserName = createdBy.UserName,
                                    ProfileAvatar = (createdBy.ProfileAvatar != null) ? ByteArrayToBase64(createdBy.ProfileAvatar, createdBy.ProfileAvatar_MimeType) : defaultAvatar,
                                    InteractButtons = GetInteractsCountDL(postReposted.PostID),
                                    RepostedBy = (repost.ID_PersonThatRePost != person.PersonID) ? context.Person.Find(repost.ID_PersonThatRePost).NickName : "ti"
                                });
                            }


                            foreach (var post in myPosts)
                            {
                                profileScreenDTO.PostsSection.Add(new PostSectionDTO()
                                {
                                    PostID = post.PostID,
                                    Comment = post.Comment,
                                    GIFImage = post.GIFImage,
                                    VideoFile = post.VideoFile,
                                    Thumbnails = GetPostedThumbnails(post.PostID),                                    
                                    PublicationDate = post.PublicationDate,
                                    CreatedBy = person.PersonID,
                                    NickName = person.NickName,
                                    UserName = person.UserName,
                                    ProfileAvatar = (person.ProfileAvatar != null) ? ByteArrayToBase64(person.ProfileAvatar, person.ProfileAvatar_MimeType) : defaultAvatar,
                                    InteractButtons = GetInteractsCountDL(post.PostID)
                                });
                            }
                            break;
                    }

                    profileScreenDTO.PostsSection = profileScreenDTO.PostsSection.OrderByDescending(ps => ps.PublicationDate).ToList();
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

                    if(person.Birthdate.HasValue)
                    {
                        profileDetailsDTO.Birthdate = BirthDatePhrase(person.Birthdate);
                        profileDetailsDTO.Day = person.Birthdate.Value.Day.ToString();
                        profileDetailsDTO.Month = person.Birthdate.Value.Month.ToString();
                        profileDetailsDTO.Year = person.Birthdate.Value.Year.ToString();
                    }

                    return profileDetailsDTO;
                }
            }
            catch
            {
                throw;
            }
        }

        public ProfileDetailsDTO ChangeProfileDetailsDL(ProfileDetailsDTO data, int personID)
        {
            try
            {
                using(var context = new MiniBirdEntities())
                {
                    var person = context.Person.Where(p => p.PersonID == personID).First();
                    person.PersonalDescription = data.PersonalDescription;
                    person.WebSiteURL = data.WebSiteURL;

                    if (string.IsNullOrWhiteSpace(data.Year) && string.IsNullOrWhiteSpace(data.Month) && string.IsNullOrWhiteSpace(data.Day))
                    {
                        person.Birthdate = null;
                        data.Birthdate = null;
                    }                        
                    else
                    {
                        person.Birthdate = new DateTime(Convert.ToInt32(data.Year), Convert.ToInt32(data.Month), Convert.ToInt32(data.Day));
                        data.Birthdate = BirthDatePhrase(person.Birthdate);
                    }                        
                    context.SaveChanges();

                    return data;
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

                    var timelineDTO = new TimelineDTO();
                    timelineDTO.ProfileSection.UserName = person.UserName;
                    timelineDTO.ProfileSection.NickName = person.NickName;
                    timelineDTO.ProfileSection.ProfileHeader = (person.ProfileHeader != null) ? ByteArrayToBase64(person.ProfileHeader, person.ProfileHeader_MimeType) : defaultHeader;
                    timelineDTO.ProfileSection.ProfileAvatar = (person.ProfileAvatar != null) ? ByteArrayToBase64(person.ProfileAvatar, person.ProfileAvatar_MimeType) : defaultAvatar;
                    timelineDTO.ProfileSection.PostCount = posts.Count();
                    timelineDTO.ProfileSection.FollowerCount = person.Person11.Count;
                    timelineDTO.ProfileSection.FollowingCount = person.Person3.Count;
                    timelineDTO.TopTrendingsSection = TopTrendings();

                    var reposts = context.RePost.Where(rp => rp.ID_PersonThatRePost == person.PersonID).ToList();

                    foreach (var p in person.Person3)
                    {
                        var postsOfFollowing = context.Post.Where(ps => ps.ID_Person == p.PersonID && ps.InReplyTo == null).ToList();
                        var repostsOfFollowing = context.RePost.Where(rp => rp.ID_PersonThatRePost == p.PersonID).ToList();

                        if (postsOfFollowing.Count > 0)
                            posts.AddRange(postsOfFollowing);

                        if (repostsOfFollowing.Count > 0)
                            reposts.AddRange(repostsOfFollowing);
                    }

                    foreach (var post in posts)
                    {
                        var createdBy = context.Person.Where(p => p.PersonID == post.ID_Person).First();
                        var reposted = reposts.Where(rp => rp.ID_Post == post.PostID);

                        if (reposted != null)
                        {
                            foreach (var repost in reposted)
                            {
                                timelineDTO.PostSection.Add(new PostSectionDTO()
                                {
                                    PostID = post.PostID,
                                    Comment = post.Comment,
                                    GIFImage = post.GIFImage,
                                    VideoFile = post.VideoFile,
                                    Thumbnails = GetPostedThumbnails(post.PostID),                                    
                                    PublicationDate = repost.PublicationDate,
                                    CreatedBy = createdBy.PersonID,
                                    NickName = createdBy.NickName,
                                    UserName = createdBy.UserName,
                                    ProfileAvatar = (createdBy.ProfileAvatar != null) ? ByteArrayToBase64(createdBy.ProfileAvatar, createdBy.ProfileAvatar_MimeType) : defaultAvatar,
                                    InteractButtons = GetInteractsCountDL(post.PostID),
                                    RepostedBy = (repost.ID_PersonThatRePost != person.PersonID) ? context.Person.Find(repost.ID_PersonThatRePost).NickName : "ti"
                                });
                            }
                        }

                        timelineDTO.PostSection.Add(new PostSectionDTO()
                        {
                            PostID = post.PostID,
                            Comment = post.Comment,
                            GIFImage = post.GIFImage,
                            VideoFile = post.VideoFile,
                            Thumbnails = GetPostedThumbnails(post.PostID),                            
                            PublicationDate = post.PublicationDate,
                            CreatedBy = createdBy.PersonID,
                            NickName = createdBy.NickName,
                            UserName = createdBy.UserName,
                            ProfileAvatar = (createdBy.ProfileAvatar != null) ? ByteArrayToBase64(createdBy.ProfileAvatar, createdBy.ProfileAvatar_MimeType) : defaultAvatar,
                            InteractButtons = GetInteractsCountDL(post.PostID)
                        });
                    }

                    timelineDTO.PostSection = timelineDTO.PostSection.OrderByDescending(ps => ps.PublicationDate).ToList();
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

        public void NewListDL(ListDTO data, int personID)
        {
            using(var context = new MiniBirdEntities())
            {
                context.List.Add(new List()
                {
                    Name = data.Name,
                    Description = data.Description,
                    IsPrivate = (data.Privacy != Privacy.Public) ? true : false,
                    ID_Person = personID
                });

                context.SaveChanges();
            }
        }

        public ListScreenDTO ListScreenCollectionDataDL(int listID, int personID)
        {
            try
            {
                using (var context = new MiniBirdEntities())
                {
                    var currentList = context.List.Where(l => l.ListID == listID).First();

                    var listScreenDTO = new ListScreenDTO();
                    listScreenDTO.CurrentListSection.MyListID = currentList.ListID;
                    listScreenDTO.CurrentListSection.Name = currentList.Name;
                    listScreenDTO.CurrentListSection.Description = currentList.Description;
                    listScreenDTO.CurrentListSection.MembersCount = currentList.Person1.Count;

                    var myLists = context.List.Where(ml => ml.ID_Person == personID);

                    foreach (var list in myLists)
                    {
                        listScreenDTO.MyListsSection.Add(new ListDTO()
                        {
                            MyListID = list.ListID,
                            Name = list.Name,
                            Description = list.Description,
                            Privacy = (list.IsPrivate != true) ? Privacy.Public : Privacy.Private
                        });
                    }

                    foreach(var pe in currentList.Person1)
                    {
                        var posts = context.Post.Where(p => p.ID_Person == pe.PersonID);
                        posts = posts.OrderByDescending(ps => ps.PublicationDate);

                        foreach (var post in posts)
                        {
                            var createdBy = context.Person.Where(p => p.PersonID == post.ID_Person).First();

                            listScreenDTO.PostSection.Add(new PostSectionDTO()
                            {
                                PostID = post.PostID,
                                Comment = post.Comment,
                                GIFImage = post.GIFImage,
                                VideoFile = post.VideoFile,
                                Thumbnails = GetPostedThumbnails(post.PostID),                                
                                PublicationDate = post.PublicationDate,
                                CreatedBy = createdBy.PersonID,
                                NickName = createdBy.NickName,
                                UserName = createdBy.UserName,
                                ProfileAvatar = (createdBy.ProfileAvatar != null) ? ByteArrayToBase64(createdBy.ProfileAvatar, createdBy.ProfileAvatar_MimeType) : defaultAvatar,
                                InteractButtons = GetInteractsCountDL(post.PostID)
                            });
                        }                        
                    }

                    return listScreenDTO;
                }
            }
            catch
            {
                throw;
            }        
        }

        public void EditListDL(ListDTO data)
        {
            try
            {
                using (var context = new MiniBirdEntities())
                {
                    var list = context.List.Find(data.MyListID);
                    list.Name = data.Name;
                    list.Description = data.Description;
                    list.IsPrivate = (data.Privacy != Privacy.Public) ? true : false;
                    context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void RemoveListDL(int listID, int personID)
        {
            try
            {
                using (var context = new MiniBirdEntities())
                {
                    var listToRemove = context.List.Find(listID);                    

                    foreach(var p in context.Person)
                    {
                        if(p.List1.Any(l => l.ListID == listID))
                            p.List1.Remove(listToRemove);
                    }
                    context.List.Remove(listToRemove);
                    context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }            
        }

        public ViewPostDTO ViewPostCollectionDataDL(int postID)
        {
            try
            {
                using(var context = new MiniBirdEntities())
                {
                    var post = context.Post.Find(postID);
                    var createdBy = context.Person.Find(post.ID_Person);

                    var ViewPost = new ViewPostDTO();
                    ViewPost.PostSection.PostID = post.PostID;
                    ViewPost.PostSection.Comment = post.Comment;
                    ViewPost.PostSection.GIFImage = post.GIFImage;
                    ViewPost.PostSection.VideoFile = post.VideoFile;
                    ViewPost.PostSection.Thumbnails = GetPostedThumbnails(post.PostID);                    
                    ViewPost.PostSection.PublicationDate = post.PublicationDate;
                    ViewPost.PostSection.CreatedBy = createdBy.PersonID;
                    ViewPost.PostSection.NickName = createdBy.NickName;
                    ViewPost.PostSection.UserName = createdBy.UserName;
                    ViewPost.PostSection.ProfileAvatar = (createdBy.ProfileAvatar != null) ? ByteArrayToBase64(createdBy.ProfileAvatar, createdBy.ProfileAvatar_MimeType) : defaultAvatar;
                    ViewPost.PostSection.InteractButtons = GetInteractsCountDL(post.PostID);

                    var replies = context.Post.Where(r => r.InReplyTo == postID).OrderByDescending(r => r.PublicationDate);

                    foreach(var reply in replies)
                    {
                        var replyCreatedBy = context.Person.Find(reply.ID_Person);

                        ViewPost.RepliesToPost.Add(new PostSectionDTO()
                        {
                            PostID = reply.PostID,
                            Comment = reply.Comment,
                            GIFImage = reply.GIFImage,
                            VideoFile = reply.VideoFile,
                            Thumbnails = GetPostedThumbnails(reply.PostID),                            
                            PublicationDate = reply.PublicationDate,
                            CreatedBy = replyCreatedBy.PersonID,
                            NickName = replyCreatedBy.NickName,
                            UserName = replyCreatedBy.UserName,
                            ProfileAvatar = (replyCreatedBy.ProfileAvatar != null) ? ByteArrayToBase64(replyCreatedBy.ProfileAvatar, replyCreatedBy.ProfileAvatar_MimeType) : defaultAvatar,
                            InteractButtons = GetInteractsCountDL(reply.PostID)
                        });
                    }

                    return ViewPost;
                }
            }
            catch
            {
                throw;
            }
        }

        public FullViewPostDTO FullViewPostCollectionDataDL(int postID)
        {
            try
            {
                var fullViewPost = new FullViewPostDTO();
                ViewPostDTO viewPost = this.ViewPostCollectionDataDL(postID);
                fullViewPost.PostSection = viewPost.PostSection;
                fullViewPost.RepliesToPost = viewPost.RepliesToPost;

                using(var context = new MiniBirdEntities())
                {
                    var person = context.Person.Find(fullViewPost.PostSection.CreatedBy);
                    fullViewPost.ProfileInformation.Birthdate = (person.Birthdate.HasValue) ? BirthDatePhrase(person.Birthdate) : "";
                    fullViewPost.ProfileInformation.NickName = person.NickName;
                    fullViewPost.ProfileInformation.PersonalDescription = person.PersonalDescription;
                    fullViewPost.ProfileInformation.ProfileAvatar = (person.ProfileAvatar != null) ? ByteArrayToBase64(person.ProfileAvatar, person.ProfileAvatar_MimeType) : defaultAvatar;
                    fullViewPost.ProfileInformation.ProfileHeader = (person.ProfileHeader != null) ? ByteArrayToBase64(person.ProfileHeader, person.ProfileHeader_MimeType) : defaultHeader;
                    fullViewPost.ProfileInformation.RegistrationDate = person.RegistrationDate;
                    fullViewPost.ProfileInformation.UserName = person.UserName;
                    fullViewPost.ProfileInformation.WebSiteURL = person.WebSiteURL;

                    return fullViewPost;
                }                               
            }
            catch
            {
                throw;
            }
        }

        public bool CreateNewReplyDL(NewPostDTO model, int personID, HttpServerUtilityBase localServer)
        {
            try
            {
                using (var context = new MiniBirdEntities())
                {
                    if (context.Person.Any(p => p.PersonID == personID))
                    {
                        var post = new Post();
                        post.Comment = model.Comment;

                        if (model.ImagesUploaded != null && model.ImagesUploaded.Length > 0)
                        {
                            int iteration = 1;

                            foreach (string image in model.ImagesUploaded)
                            {
                                context.Thumbnail.Add(new Thumbnail() { ImagePath = SaveThumbnailInFolder(image, post.PostID, localServer, iteration), ID_Post = post.PostID });
                                context.SaveChanges();
                                iteration++;
                            }
                        }

                        post.PublicationDate = DateTime.Now;

                        if (model.InReplyTo != null && model.InReplyTo != 0)
                            post.InReplyTo = model.InReplyTo;

                        post.ID_Person = personID;

                        List<Hashtag> hashtags = DiscoverHashtag(model.Comment, context);

                        if (hashtags.Count > 0)
                            foreach (var hashtag in hashtags)
                                post.Hashtag.Add(hashtag);

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

        private string BirthDatePhrase(DateTime? birthDate)
        {
            return "Nació el " + birthDate.Value.Day + " de " + birthDate.Value.ToString("MMMM") + " de " + birthDate.Value.Year;
        }

        private List<Hashtag> DiscoverHashtag(string comment, MiniBirdEntities context)
        {
            string[] words = comment.Split(' ');
            List<Hashtag> hashtags = new List<Hashtag>();
            
            foreach (string word in words)
            {
                if (word.Length >= 3)
                {
                    if (word.StartsWith("#") && word.IndexOf('#', 1) < 1)
                    {
                        if (!context.Hashtag.Any(h => h.Name == word))
                            hashtags.Add(new Hashtag() { Name = word, CreationDate = DateTime.Now, UseCount = 1 });
                        else
                        {
                            Hashtag existingHashtag = context.Hashtag.Single(h => h.Name == word);

                            if (!hashtags.Any(hs => hs.Name == word))
                                hashtags.Add(existingHashtag);

                            existingHashtag.UseCount = existingHashtag.UseCount + 1;
                            context.SaveChanges();
                        }
                    }
                }
            }

            return hashtags;
        }

        private List<TopTrendingsDTO> TopTrendings()
        {
            using (var context = new MiniBirdEntities())
            {
                IQueryable<Hashtag> topTrendings = context.Hashtag.OrderByDescending(h => h.UseCount).Take(10);
                List<TopTrendingsDTO> topTrendingsDTO = new List<TopTrendingsDTO>();

                if (topTrendings.Count() > 0)
                {
                    foreach (var trending in topTrendings)
                    {
                        topTrendingsDTO.Add(new TopTrendingsDTO()
                        {
                            Name = trending.Name,
                            UseCount = trending.UseCount
                        });
                    }
                }

                return topTrendingsDTO;
            }            
        }


        public string SaveThumbnailInFolder(string imgBase64, int postID, HttpServerUtilityBase localServer, int iteration)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imgBase64))
                    throw new ArgumentNullException("Debe elegir una imágen.");

                string thumbnailsPath = "/Content/thumbnails/" + postID;

                if (!Directory.Exists(thumbnailsPath))
                    Directory.CreateDirectory(localServer.MapPath(thumbnailsPath));

                //Directory.CreateDirectory(localServer.MapPath(imgDir));
                //string imgFullPath = localServer.MapPath(imgDir) + "/" + tempImage.FileName;

                // Get file data
                byte[] bytes = imgBase64ToByteArray(imgBase64);
                string ext = GetExtensionFromImgBase64(imgBase64);
                string pathToNewFile = thumbnailsPath + "/posted_thumbnail_" + iteration + '.' + ext;

                // Guardar imagen en el servidor
                using (FileStream image = File.Create(localServer.MapPath(pathToNewFile), bytes.Length))
                {
                    image.Write(bytes, 0, bytes.Length);
                    image.Flush();
                }

                // Verifica si la imágen cumple las condiciones de validación
                //const int _maxSize = 2 * 1024 * 1024;
                //const int _maxWidth = 1000;
                //const int _maxHeight = 1000;
                //List<string> _fileTypes = new List<string>() { "jpg", "jpeg", "gif", "png" };
                //string fileExt = Path.GetExtension(tempImage.FileName);

                //if (new FileInfo(imgFullPath).Length > _maxSize)
                //    throw new FormatException("El avatar no debe superar los 2mb.");

                //if (!_fileTypes.Contains(fileExt.Substring(1), StringComparer.OrdinalIgnoreCase))
                //    throw new FormatException("Para el avatar solo se admiten imágenes JPG, JPEG, GIF Y PNG.");

                //using (Image img = Image.FromFile(imgFullPath))
                //{
                //    if (img.Width > _maxWidth || img.Height > _maxHeight)
                //        throw new FormatException("El avatar admite hasta una resolución de 1000x1000.");
                //}

                return pathToNewFile;
            }
            catch
            {
                using (var context = new MiniBirdEntities())
                {
                    Post post = context.Post.Find(postID);
                    context.Post.Remove(post);
                    context.SaveChanges();
                }

                throw;
            }
        }

        public string GetExtensionFromImgBase64(string imgBase64)
        {
            const string startWord = "data:image/";
            const string endWord = ";base64,";
            int start = imgBase64.IndexOf(startWord) + startWord.Length;
            int end = imgBase64.IndexOf(endWord);

            return imgBase64.Substring(start, end - start);
        }

        public List<string> GetPostedThumbnails(int postID)
        {
            using (var context = new MiniBirdEntities())
            {
                List<string> postedThumbnails = new List<string>();
                var thumbnails = context.Thumbnail.Where(t => t.ID_Post == postID);

                foreach(var thumbnail in thumbnails)
                {
                    postedThumbnails.Add(thumbnail.ImagePath);
                }

                return postedThumbnails;
            }
        }

        #endregion
    }
}
