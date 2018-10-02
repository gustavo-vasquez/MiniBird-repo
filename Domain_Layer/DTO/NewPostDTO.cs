using Domain_Layer.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Domain_Layer.DTO
{
    public class NewPostDTO
    {
        public int PostID { get; set; }

        [Required(ErrorMessage = "Primero cuéntanos que quieres decir")]
        [MaxLength(280, ErrorMessage = "Límite: 280 caracteres")]        
        public string Comment { get; set; }
        
        [MultipleFilesMaxSize(30*1024)]
        //[FileValidExtension("jpg","jpeg")]
        public IEnumerable<HttpPostedFileBase> ImageFiles { get; set; }

        [FileMaxSize(1*1024*1024)]
        [FileValidExtension("gif")]
        public HttpPostedFileBase GifImage { get; set; }

        [FileMaxSize(5*1024*1024)]
        [FileValidExtension("mp4","avi")]
        public HttpPostedFileBase VideoFile { get; set; }
        public string[] ImagesUploaded { get; set; }        
        public int? InReplyTo { get; set; }
    }    
}