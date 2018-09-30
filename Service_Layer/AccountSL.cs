using Data_Layer;
using Domain_Layer;
using Domain_Layer.DTO;
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

        public bool CreateNewPostSL(NewPostDTO model, int personID, HttpServerUtilityBase localServer)
        {            
            return Account.CreateNewPostDL(model, personID, localServer);
        }

        public ProfileScreenDTO ProfileScreenCollectionDataSL(int personID, string v)
        {
            return Account.ProfileScreenCollectionDataDL(personID, v);
        }

        public ProfileDetailsDTO ChangeProfileDetailsSL(int personID)
        {
            return Account.ChangeProfileDetailsDL(personID);
        }

        public ProfileDetailsDTO ChangeProfileDetailsSL(ProfileDetailsDTO data, int personID)
        {
            return Account.ChangeProfileDetailsDL(data, personID);
        }

        public TimelineDTO TimelineCollectionDataSL(int personID)
        {
            return Account.TimelineCollectionDataDL(personID);
        }

        public string ChangeHeaderSL(HttpPostedFile img, int personID)
        {
            return Account.ChangeHeaderDL(img, personID);
        }

        public string ChangeAvatarSL(HttpPostedFile img, int personID)
        {
            return Account.ChangeAvatarDL(img, personID);
        }

        public void SendRepostSL(int postID, int personID)
        {
            Account.SendRepostDL(postID, personID);
        }

        public void GiveALikeSL(int postID, int personID)
        {
            Account.GiveALikeDL(postID, personID);
        }

        public InteractButtonsDTO GetInteractsCountSL(int postID)
        {
            return Account.GetInteractsCountDL(postID);
        }

        public void NewListSL(ListDTO data, int personID)
        {
            Account.NewListDL(data, personID);
        }

        public ListScreenDTO ListScreenCollectionDataSL(int listID, int personID)
        {
            return Account.ListScreenCollectionDataDL(listID, personID);
        }

        public void EditListSL(ListDTO data)
        {
            Account.EditListDL(data);
        }

        public void RemoveListSL(int listID, int personID)
        {
            Account.RemoveListDL(listID, personID);
        }

        public ViewPostDTO ViewPostAjaxCollectionDataSL(int postID)
        {
            return Account.ViewPostAjaxCollectionDataDL(postID);
        }

        public FullViewPostDTO ViewPostCollectionDataSL(int postID)
        {
            return Account.ViewPostCollectionDataDL(postID);
        }

        public bool CreateNewReplySL(NewPostDTO data, int personID, HttpServerUtilityBase localServer)
        {
            return Account.CreateNewReplyDL(data, personID, localServer);
        }


        public bool FollowUserSL(int personID, int follow)
        {
            return Account.FollowUserDL(personID, follow);
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

        //public string TemporaryPostImageSL(HttpPostedFile tempImage, HttpServerUtilityBase localServer, int personID)
        //{
        //    return Account.TemporaryPostImageDL(tempImage, localServer, personID);
        //}

        #endregion
    }
}
