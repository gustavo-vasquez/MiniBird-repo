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
        public byte[] Image1stSlot { get; set; }
        public byte[] Image2ndSlot { get; set; }
        public byte[] Image3rdSlot { get; set; }
        public byte[] Image4thSlot { get; set; }
        public byte[] UploadImage { get; set; }
        public int InReplyTo { get; set; }
    }
}