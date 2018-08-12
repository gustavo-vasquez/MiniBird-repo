using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MiniBird.DTO
{
    public class NewPostDTO
    {
        public int PostID { get; set; }

        [Required(ErrorMessage = "Primero cuéntanos que quieres decir")]
        [MaxLength(280, ErrorMessage = "Límite: 280 caracteres")]
        public string Comment { get; set; }
        public byte[] GifImage { get; set; }
        public byte[] VideoFile { get; set; }
        public string[] ImagesUploaded { get; set; }
        public int? InReplyTo { get; set; }
    }
}