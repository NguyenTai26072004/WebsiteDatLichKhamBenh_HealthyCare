using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
 


namespace WebsiteDatLichKhamBenh.Models
{
    public class ImageModel
    {
        public string ImageUrl { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
    }
}