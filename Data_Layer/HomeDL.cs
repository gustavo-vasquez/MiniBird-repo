using Domain_Layer;
using Domain_Layer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Layer
{
    public class HomeDL
    {
        public MatchesFoundDTO FindMatchesDL(string queryString, int personID)
        {
            try
            {
                MatchesFoundDTO matchesFound = new MatchesFoundDTO();

                if (!string.IsNullOrWhiteSpace(queryString))
                {
                    using (var context = new MiniBirdEntities())
                    {                        
                        IQueryable<Hashtag> hashtagMatches = context.Hashtag.Where(h => h.Name.Contains(queryString)).Take(4);
                        IQueryable<Person> profileMatches = context.Person.Where(p => p.NickName.Contains(queryString) || p.UserName.Contains(queryString)).Take(4);

                        if(hashtagMatches.Count() > 0 || profileMatches.Count() > 0)
                        {
                            foreach (var hashtag in hashtagMatches)
                            {
                                matchesFound.hashtagMatches.Add(new MatchesFoundDTO.HashtagMatchesDTO()
                                {
                                    HashtagID = hashtag.HashtagID,
                                    HashtagName = hashtag.Name
                                });
                            }

                            foreach (var profile in profileMatches)
                            {
                                matchesFound.profileMatches.Add(new MatchesFoundDTO.ProfileMatchesDTO()
                                {
                                    PersonID = profile.PersonID,
                                    NickName = profile.NickName,
                                    UserName = profile.UserName,
                                    ProfileAvatar = (profile.ProfileAvatar != null) ? ByteArrayToBase64(profile.ProfileAvatar, profile.ProfileAvatar_MimeType) : "/Content/images/defaultAvatar.png",
                                    Following = context.Follow.Any(f => f.ID_Person == personID && f.ID_PersonFollowed == profile.PersonID)
                                });
                            }                            
                        }
                    }
                }

                return matchesFound;
            }
            catch
            {
                throw;
            }
        }

        public HashtagDTO GetPostsUsingHashtagDL(string name)
        {
            try
            {
                using(var context = new MiniBirdEntities())
                {
                    IQueryable<Post> posts = context.Post.Where(p => p.Hashtag.Any(h => h.Name == name));
                    HashtagDTO hashtagDTO = new HashtagDTO();
                    hashtagDTO.Name = name;

                    foreach(var post in posts)
                    {
                        var createdBy = context.Person.Find(post.ID_Person);

                        hashtagDTO.PostSection.Add(new PostSectionDTO()
                        {
                            PostID = post.PostID,
                            Comment = post.Comment,
                            GIFImage = post.GIFImage,
                            VideoFile = post.VideoFile,
                            Thumbnails = new AccountDL().GetPostedThumbnails(post.PostID),
                            PublicationDate = post.PublicationDate,
                            CreatedBy = createdBy.PersonID,
                            NickName = createdBy.NickName,
                            UserName = createdBy.UserName,
                            ProfileAvatar = (createdBy.ProfileAvatar != null) ? ByteArrayToBase64(createdBy.ProfileAvatar, createdBy.ProfileAvatar_MimeType) : "/Content/images/defaultAvatar.png",
                            InteractButtons = new AccountDL().GetInteractsCountDL(post.PostID, ActiveSession.GetPersonID()),
                        });
                    }

                    return hashtagDTO;
                }
            }
            catch
            {
                throw;
            }
        }


        #region

        private string ByteArrayToBase64(byte[] profileAvatar, string mimeType)
        {
            if (profileAvatar != null)
                return String.Concat("data:", mimeType, ";base64,", Convert.ToBase64String(profileAvatar));

            return null;
        }

        #endregion
    }
}
