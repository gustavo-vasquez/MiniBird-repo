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

        public bool LoginSL(string emailOrUsername, string password)
        {
            return Account.LoginDL(emailOrUsername, password);
        }

        public SessionInformation CreateSessionSL(string emailOrUsername)
        {
            return Account.CreateSessionDL(emailOrUsername);
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

        public InteractButtonsDTO GetInteractsCountSL(int postID, int personID)
        {
            return Account.GetInteractsCountDL(postID, personID);
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

        public bool FollowUserSL(int personID, int follow)
        {
            return Account.FollowUserDL(personID, follow);
        }

        public List<CheckboxListsDTO> CheckboxListsSL(int currentProfileID, int activeUser)
        {
            return Account.CheckboxListsDL(currentProfileID, activeUser);
        }

        public void AddProfileToListsSL(List<CheckboxListsDTO> model, int currentProfileID)
        {
            Account.AddProfileToListsDL(model, currentProfileID);
        }


        #region TAREAS AUXILIARES

        public bool UserNameExistsSL(string username)
        {
            return Account.UserNameExistsDL(username);
        }

        public string EncryptCookieValueSL(string emailOrUsername)
        {
            return Account.EncryptCookieValueDL(emailOrUsername);
        }

        public bool ToggleThemeSL(int userID)
        {
            return Account.ToggleThemeDL(userID);
        }

        #endregion
    }
}
