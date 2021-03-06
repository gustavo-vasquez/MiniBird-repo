﻿using Domain_Layer;
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
                        DarkMode = false,
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

        public bool LoginDL(string emailOrUsername, string password)
        {
            using (var context = new MiniBirdEntities())
            {
                var person = context.Person.Where(p => (p.UserName == "@" + emailOrUsername || p.Email == emailOrUsername) && p.Password == password).FirstOrDefault();

                if(person != null)                
                    return true;
            }
                           
            return false;
        }

        public SessionInformation CreateSessionDL(string emailOrUsername)
        {
            try
            {                
                using (var context = new MiniBirdEntities())
                {
                    var person = context.Person.Where(p => p.UserName == "@" + emailOrUsername || p.Email == emailOrUsername).First();

                    return new SessionInformation()
                    {
                        PersonID = person.PersonID,
                        UserName = person.UserName,
                        Email = person.Email,
                        NickName = person.NickName,
                        ProfileAvatar = (person.ProfileAvatar != null) ? ByteArrayToBase64(person.ProfileAvatar, person.ProfileAvatar_MimeType) : defaultAvatar,
                        ProfileHeader = (person.ProfileHeader != null) ? ByteArrayToBase64(person.ProfileHeader, person.ProfileHeader_MimeType) : defaultHeader,
                        Theme = (person.DarkMode == true) ? Theme.Dark : Theme.Light
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
                        
                        context.Post.Add(post);
                        context.SaveChanges();
                        
                        if (model.ImagesUploaded != null && model.ImagesUploaded.Length > 0)
                        {
                            int iteration = 1;

                            foreach (string image in model.ImagesUploaded)
                            {
                                context.Thumbnail.Add(new Thumbnail() { FilePath = SaveThumbnailOnServer(image, post.PostID, localServer, iteration), ID_Post = post.PostID });
                                context.SaveChanges();
                                iteration++;
                            }
                        }
                        else if (model.GifImage != null && model.GifImage.ContentLength > 0)
                        {
                            post.GIFImage = SaveGifOnServer(post.PostID, model.GifImage, localServer);
                            context.SaveChanges();
                        }
                        else if(model.VideoFile != null && model.VideoFile.ContentLength > 0)
                        {
                            post.VideoFile = SaveGifOnServer(post.PostID, model.VideoFile, localServer);
                            context.SaveChanges();
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
                    Person person = context.Person.Find(personID);
                    var follows = context.Follow;

                    var profileScreenDTO = new ProfileScreenDTO();
                    profileScreenDTO.ProfileInformation.PersonID = person.PersonID;
                    profileScreenDTO.ProfileInformation.UserName = person.UserName;
                    profileScreenDTO.ProfileInformation.NickName = person.NickName;
                    profileScreenDTO.ProfileInformation.PersonalDescription = person.PersonalDescription;
                    profileScreenDTO.ProfileInformation.WebSiteURL = person.WebSiteURL;
                    profileScreenDTO.ProfileInformation.Birthdate = (person.Birthdate.HasValue) ? BirthDatePhrase(person.Birthdate) : "";
                    profileScreenDTO.ProfileInformation.RegistrationDate = person.RegistrationDate;
                    profileScreenDTO.ProfileInformation.ProfileAvatar = (person.ProfileAvatar != null) ? ByteArrayToBase64(person.ProfileAvatar, person.ProfileAvatar_MimeType) : defaultAvatar;
                    profileScreenDTO.ProfileInformation.ProfileHeader = (person.ProfileHeader != null) ? ByteArrayToBase64(person.ProfileHeader, person.ProfileHeader_MimeType) : defaultHeader;
                    profileScreenDTO.TopTrendings = TopTrendings();
                    profileScreenDTO.StatisticsBar.PostsCount = context.Post.Where(ps => ps.ID_Person == person.PersonID && ps.InReplyTo == null).Count();
                    profileScreenDTO.StatisticsBar.FollowingCount = GetFollowingCount(follows, person.PersonID);
                    profileScreenDTO.StatisticsBar.FollowersCount = GetFollowersCount(follows, person.PersonID);
                    profileScreenDTO.StatisticsBar.LikesCount = context.LikePost.Where(lp => lp.ID_PersonThatLikesPost == person.PersonID).Count();
                    profileScreenDTO.StatisticsBar.ListsCount = context.List.Where(ml => ml.ID_Person == person.PersonID).Count();

                    if(ActiveSession.GetPersonID() != person.PersonID)
                    {
                        Person activeUser = context.Person.Find(ActiveSession.GetPersonID());
                        profileScreenDTO.Following = follows.Any(f => f.ID_Person == activeUser.PersonID && f.ID_PersonFollowed == person.PersonID);
                    }                    

                    switch(v)
                    {
                        case "following":
                            var myFollowings = follows.Where(f => f.ID_Person == person.PersonID);

                            foreach(var following in myFollowings)
                            {
                                var personFollowed = context.Person.Find(following.ID_PersonFollowed);

                                profileScreenDTO.Followings.Add(new FollowingDTO()
                                {
                                    PersonID = personFollowed.PersonID,
                                    NickName = personFollowed.NickName,
                                    UserName = personFollowed.UserName,
                                    ProfileAvatar = (personFollowed.ProfileAvatar != null) ? ByteArrayToBase64(personFollowed.ProfileAvatar, personFollowed.ProfileAvatar_MimeType) : defaultAvatar,
                                    Description = personFollowed.PersonalDescription,
                                    FollowingCount = GetFollowingCount(follows, personFollowed.PersonID),
                                    FollowersCount = GetFollowersCount(follows, personFollowed.PersonID)
                                });
                            }                            

                            break;
                        case "followers":
                            var myFollowers = follows.Where(f => f.ID_PersonFollowed == person.PersonID);

                            foreach (var follower in myFollowers)
                            {
                                var personThatFollowMe = context.Person.Find(follower.ID_Person);

                                profileScreenDTO.Followers.Add(new FollowingDTO()
                                {
                                    PersonID = personThatFollowMe.PersonID,
                                    NickName = personThatFollowMe.NickName,
                                    UserName = personThatFollowMe.UserName,
                                    ProfileAvatar = (personThatFollowMe.ProfileAvatar != null) ? ByteArrayToBase64(personThatFollowMe.ProfileAvatar, personThatFollowMe.ProfileAvatar_MimeType) : defaultAvatar,
                                    Description = personThatFollowMe.PersonalDescription,
                                    FollowingCount = follows.Where(f => f.ID_Person == personThatFollowMe.PersonID).Count(),
                                    FollowersCount = follows.Where(f => f.ID_PersonFollowed == personThatFollowMe.PersonID).Count(),
                                    Following = context.Follow.Any(f => f.ID_Person == person.PersonID && f.ID_PersonFollowed == personThatFollowMe.PersonID)
                                });
                            }

                            break;
                        case "likes":

                            var myLikes = context.LikePost.Where(lp => lp.ID_PersonThatLikesPost == person.PersonID).OrderByDescending(lp => lp.DateOfAction);

                            foreach(var like in myLikes)
                            {
                                var postLiked = context.Post.Find(like.ID_Post);
                                var createdBy = context.Person.Find(postLiked.ID_Person);

                                profileScreenDTO.LikesSection.Add(new PostSectionDTO()
                                {
                                    PostID = postLiked.PostID,
                                    Comment = postLiked.Comment,
                                    GIFImage = postLiked.GIFImage,
                                    VideoFile = postLiked.VideoFile,
                                    Thumbnails = GetPostedThumbnails(postLiked.PostID),
                                    PublicationDate = postLiked.PublicationDate,
                                    CreatedBy = createdBy.PersonID,
                                    NickName = createdBy.NickName,
                                    UserName = createdBy.UserName,
                                    ProfileAvatar = (createdBy.ProfileAvatar != null) ? ByteArrayToBase64(createdBy.ProfileAvatar, createdBy.ProfileAvatar_MimeType) : defaultAvatar,
                                    InteractButtons = GetInteractsCountDL(postLiked.PostID, person.PersonID)                                    
                                });
                            }

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
                                    MembersCount = context.UserToList.Where(ul => ul.ID_List == list.ListID).Count()
                                });
                            }

                            break;
                        default:
                            var myPosts = context.Post.Where(mp => mp.ID_Person == personID && mp.InReplyTo == null).ToList();
                            var myReposts = context.RePost.Where(rp => rp.ID_PersonThatRePost == person.PersonID).ToList();

                            profileScreenDTO.PostsSection.AddRange(FillPostSection(myPosts, person.PersonID, myReposts));
                            profileScreenDTO.PostsSection = profileScreenDTO.PostsSection.OrderByDescending(ps => ps.PublicationDate).ToList();                            
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

        private List<PostSectionDTO> FillPostSection(List<Post> posts, int currentPersonID, List<RePost> reposts)
        {
            using (var context = new MiniBirdEntities())
            {
                List<PostSectionDTO> postSection = new List<PostSectionDTO>();

                foreach (var post in posts)
                {
                    var createdBy = context.Person.Find(post.ID_Person);

                    postSection.Add(new PostSectionDTO()
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
                        InteractButtons = GetInteractsCountDL(post.PostID, currentPersonID)
                    });
                }

                foreach (var repost in reposts)
                {
                    var post = context.Post.Find(repost.ID_Post);
                    var createdBy = context.Person.Find(post.ID_Person);

                    postSection.Add(new PostSectionDTO()
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
                        InteractButtons = GetInteractsCountDL(post.PostID, currentPersonID),
                        RepostedBy = (repost.ID_PersonThatRePost != currentPersonID) ? context.Person.Find(repost.ID_PersonThatRePost).NickName : "ti"
                    });
                }                                                    

                return postSection;
            }
        }

        public TimelineDTO TimelineCollectionDataDL(int personID)
        {
            try
            {
                using(var context = new MiniBirdEntities())
                {
                    var person = context.Person.Find(personID);
                    var posts = context.Post.Where(ps => ps.ID_Person == personID && ps.InReplyTo == null).ToList();
                    var reposts = context.RePost.Where(rp => rp.ID_PersonThatRePost == person.PersonID).ToList();

                    var timelineDTO = new TimelineDTO();
                    timelineDTO.ProfileSection.PersonID = person.PersonID;
                    timelineDTO.ProfileSection.UserName = person.UserName;
                    timelineDTO.ProfileSection.NickName = person.NickName;
                    timelineDTO.ProfileSection.ProfileHeader = (person.ProfileHeader != null) ? ByteArrayToBase64(person.ProfileHeader, person.ProfileHeader_MimeType) : defaultHeader;
                    timelineDTO.ProfileSection.ProfileAvatar = (person.ProfileAvatar != null) ? ByteArrayToBase64(person.ProfileAvatar, person.ProfileAvatar_MimeType) : defaultAvatar;
                    timelineDTO.ProfileSection.PostCount = posts.Count();
                    timelineDTO.ProfileSection.FollowingCount = GetFollowingCount(context.Follow, person.PersonID);
                    timelineDTO.ProfileSection.FollowerCount = GetFollowersCount(context.Follow, person.PersonID);
                    timelineDTO.TopTrendingsSection = TopTrendings();                    

                    timelineDTO.PostSection.AddRange(FillPostSection(posts, person.PersonID, reposts));

                    var myFollowings = context.Follow.Where(f => f.ID_Person == personID);

                    foreach (var follow in myFollowings)
                    {
                        var personFollowedID = context.Person.Find(follow.ID_PersonFollowed).PersonID;
                        var postsOfFollowing = context.Post.Where(ps => ps.ID_Person == personFollowedID && ps.InReplyTo == null).ToList();
                        var repostsOfFollowing = context.RePost.Where(rp => rp.ID_PersonThatRePost == personFollowedID).ToList();

                        timelineDTO.PostSection.AddRange(FillPostSection(postsOfFollowing, person.PersonID, repostsOfFollowing));
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

        public InteractButtonsDTO GetInteractsCountDL(int postID, int personID)
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
                    LikesCount = likes,
                    IReposted = context.RePost.Any(r => r.ID_PersonThatRePost == personID && r.ID_Post == postID),
                    ILiked = context.LikePost.Any(l => l.ID_PersonThatLikesPost == personID && l.ID_Post == postID)
                };
            }
        }

        public void SendRepostDL(int postID, int personID)
        {
            using (var context = new MiniBirdEntities())
            {
                var repost = context.RePost.Any(lp => lp.ID_Post == postID && lp.ID_PersonThatRePost == personID);

                if (repost)
                    context.RePost.Remove(context.RePost.Where(rp => rp.ID_Post == postID && rp.ID_PersonThatRePost == personID).FirstOrDefault());
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
                    CreationDate = DateTime.Now,
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
                    var currentList = context.List.Find(listID);

                    var listScreenDTO = new ListScreenDTO();
                    listScreenDTO.CurrentListSection.MyListID = currentList.ListID;
                    listScreenDTO.CurrentListSection.Name = currentList.Name;
                    listScreenDTO.CurrentListSection.Description = currentList.Description;
                    listScreenDTO.CurrentListSection.MembersCount = context.UserToList.Where(ul => ul.ID_List == currentList.ListID).Count();
                    listScreenDTO.CanEdit = (personID == currentList.ID_Person) ? true : false;

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

                    IQueryable<UserToList> personsInThisList = context.UserToList.Where(ul => ul.ID_List == currentList.ListID);

                    foreach(var personITL in personsInThisList)
                    {
                        var posts = context.Post.Where(p => p.ID_Person == personITL.ID_Person).ToList();
                        var reposts = context.RePost.Where(rp => rp.ID_PersonThatRePost == personITL.ID_Person).ToList();

                        listScreenDTO.PostSection.AddRange(FillPostSection(posts, personID, reposts));                        
                    }

                    listScreenDTO.PostSection = listScreenDTO.PostSection.OrderByDescending(ps => ps.PublicationDate).ToList();

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
                    List listToRemove = context.List.Find(listID);

                    foreach(var row in context.UserToList.Where(ul => ul.ID_List == listID))
                    {
                        context.UserToList.Remove(row);                        
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

        public ViewPostDTO ViewPostAjaxCollectionDataDL(int postID)
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
                    ViewPost.PostSection.InteractButtons = GetInteractsCountDL(post.PostID, ActiveSession.GetPersonID());

                    if (post.InReplyTo > 0)
                    {
                        var toProfile = context.Post.Find(post.InReplyTo).ID_Person;

                        ViewPost.IsReply = true;
                        ViewPost.ReplyData.ToProfile = toProfile;
                        ViewPost.ReplyData.ToUsername = context.Person.Find(toProfile).UserName;
                        ViewPost.ReplyData.ToPost = Convert.ToInt32(post.InReplyTo);
                    }
                    else
                        ViewPost.IsReply = false;

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
                            InteractButtons = GetInteractsCountDL(reply.PostID, ActiveSession.GetPersonID())
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

        public FullViewPostDTO ViewPostCollectionDataDL(int postID)
        {
            try
            {
                var fullViewPost = new FullViewPostDTO();
                ViewPostDTO viewPost = this.ViewPostAjaxCollectionDataDL(postID);
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

        public bool FollowUserDL(int personID, int follow)
        {
            try
            {
                using (var context = new MiniBirdEntities())
                {
                    var person = context.Person.Find(personID);
                    var personToFollow = context.Person.Find(follow);

                    if (context.Follow.Any(f => f.ID_Person == personID && f.ID_PersonFollowed == follow))
                    {
                        Follow followToRemove = context.Follow.Where(f => f.ID_Person == personID && f.ID_PersonFollowed == follow).FirstOrDefault();
                        context.Follow.Remove(followToRemove);
                        context.SaveChanges();
                        return false;
                    }
                        
                    else
                    {
                        context.Follow.Add(new Follow() { ID_Person = personID, ID_PersonFollowed = follow, DateOfAction = DateTime.Now });
                        context.SaveChanges();
                        return true;
                    }                        
                }
            }
            catch
            {
                throw;
            }
        }


        public List<CheckboxListsDTO> CheckboxListsDL(int currentProfileID, int activeUser)
        {
            using(var context = new MiniBirdEntities())
            {
                var myLists = context.List.Where(ml => ml.ID_Person == activeUser);
                List<CheckboxListsDTO> checkboxListsDTO = new List<CheckboxListsDTO>();

                foreach (var list in myLists)
                {
                    checkboxListsDTO.Add(new CheckboxListsDTO()
                    {
                        MyListID = list.ListID,
                        Name = list.Name,
                        Description = list.Description,
                        PersonalList = (context.UserToList.Any(ul => ul.ID_Person == currentProfileID && ul.ID_List == list.ListID)) ? true : false
                    });
                }

                return checkboxListsDTO;
            }            
        }

        public void AddProfileToListsDL(List<CheckboxListsDTO> model, int currentProfileID)
        {
            using(var context = new MiniBirdEntities())
            {
                foreach(var list in model)
                {
                    if(context.UserToList.Any(ul => ul.ID_List == list.MyListID && ul.ID_Person == currentProfileID) != list.PersonalList)
                    {
                        if(list.PersonalList)
                        {
                            context.UserToList.Add(new UserToList() { ID_List = list.MyListID, ID_Person = currentProfileID, DateOfAggregate = DateTime.Now });
                            context.SaveChanges();
                        }
                        else
                        {
                            context.UserToList.Remove(context.UserToList.Find(list.MyListID, currentProfileID));
                            context.SaveChanges();
                        }
                    }
                }
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

        public string EncryptCookieValueDL(string emailOrUsername)
        {
            try
            {
                using (var context = new MiniBirdEntities())
                {
                    return EncryptToSHA256(context.Person.Where(p => p.UserName == "@" + emailOrUsername || p.Email == emailOrUsername).First().PersonID);
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

        private string SaveGifOnServer(int postID, HttpPostedFileBase gifImage, HttpServerUtilityBase localServer)
        {
            try
            {
                if (gifImage == null && gifImage.ContentLength <= 0)
                    throw new ArgumentException("Ninguna imágen seleccionada");

                string thumbnailsPath = "/Content/thumbnails/" + postID;

                if (!Directory.Exists(thumbnailsPath))
                    Directory.CreateDirectory(localServer.MapPath(thumbnailsPath));

                // Obtiene datos del archivo
                byte[] data = new byte[] { };
                using (var binaryReader = new BinaryReader(gifImage.InputStream))
                {
                    data = binaryReader.ReadBytes(gifImage.ContentLength);
                }

                // Guarda imagen en el servidor
                string pathToNewFile = thumbnailsPath + "/" + gifImage.FileName;
                using (FileStream image = System.IO.File.Create(localServer.MapPath(pathToNewFile), data.Length))
                {
                    image.Write(data, 0, data.Length);
                    image.Flush();
                }

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


        public string SaveThumbnailOnServer(string imgBase64, int postID, HttpServerUtilityBase localServer, int iteration)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imgBase64))
                    throw new ArgumentNullException("Debe elegir una imágen.");

                string thumbnailsPath = "/Content/thumbnails/" + postID;

                if (!Directory.Exists(thumbnailsPath))
                    Directory.CreateDirectory(localServer.MapPath(thumbnailsPath));                

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
                    postedThumbnails.Add(thumbnail.FilePath);
                }

                return postedThumbnails;
            }
        }

        private int GetFollowingCount(IEnumerable<Follow> follows, int personID)
        {
            return follows.Where(f => f.ID_Person == personID).Count();
        }

        private int GetFollowersCount(IEnumerable<Follow> follows, int personID)
        {
            return follows.Where(f => f.ID_PersonFollowed == personID).Count();
        }

        public bool ToggleThemeDL(int userID)
        {
            using(var context = new MiniBirdEntities())
            {
                Person person = context.Person.Find(userID);

                if (person.DarkMode == true)
                {
                    person.DarkMode = false;                    
                    ActiveSession.Theme = Theme.Light;
                }
                else
                {
                    person.DarkMode = true;
                    ActiveSession.Theme = Theme.Dark;
                }

                context.SaveChanges();
                return Convert.ToBoolean(person.DarkMode);
            }
        }

        #endregion
    }
}
