using ImageStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nightowlsign.data;


namespace nightowlsign.data.Models.Image
{
    public class ImageManager
    {
        public ImageManager()
        {
            ValidationErrors = new List<KeyValuePair<string, string>>();
        }
        //Properties
        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }


        public List<ImagesAndSign> Get(ImagesAndSign Entity)
        {
            List<ImagesAndSign> ret = new List<ImagesAndSign>();
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                ret = db.ImagesAndSigns.OrderBy(x=>x.Model).ThenBy(x => x.Caption).ToList<ImagesAndSign>();
            }
            if (!string.IsNullOrEmpty(Entity.Caption))
            {
                ret = ret.FindAll(p => p.Caption.ToLower().StartsWith(Entity.Caption));
            }
            if (Entity.SignSize>0)
            {
                ret = ret.FindAll(p => p.SignSize.Equals(Entity.SignSize));
            }

            return ret;
        }

        public data.Image Find(int imageId)
        {
            data.Image ret = null;
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                ret = db.Images.Find(imageId);
            }
            return ret;

        }

        public bool Validate(data.Image entity)
        {
            ValidationErrors.Clear();

            if (!string.IsNullOrEmpty(entity.Caption))
            {
                //if (entity.Caption.ToLower() == entity.Caption)
                //{
                //    ValidationErrors.Add(new KeyValuePair<string, string>("Caption", "Caption cannot be all lower case"));
                //}

            }
            return (ValidationErrors.Count == 0);

        }

        public bool Validate(UploadedImage entity)
        {
            ValidationErrors.Clear();

            if (!string.IsNullOrEmpty(entity.Caption))
            {
                if (entity.Caption.ToLower() == entity.Caption)
                {
               //     ValidationErrors.Add(new KeyValuePair<string, string>("Caption", "Caption cannot be all lower case"));
                }
            }
            return (ValidationErrors.Count == 0);
        }


        public Boolean Update(UploadedImage imageToUpDate)
        {
            bool ret = false;
            if (Validate(imageToUpDate))
            {
                try
                {
                    data.Image entity = new data.Image()
                    {
                        Id = imageToUpDate.Id,
                        Caption = imageToUpDate.Caption,
                        DateTaken = imageToUpDate.DateTaken,
                        SignSize=imageToUpDate.SignId
                    };
                    using (nightowlsign_Entities db = new nightowlsign_Entities())
                    {
                        db.Images.Attach(entity);
                        var modifiedImage = db.Entry(entity);
                        modifiedImage.Property(e => e.Caption).IsModified = true;
                        modifiedImage.Property(e => e.DateTaken).IsModified = true;
                        modifiedImage.Property(e => e.SignSize).IsModified = true;
                        // modifiedImage.Property(e => e.ImageURL).IsModified = true;
                        db.SaveChanges();
                        ret = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.InnerException);
                    ret = false;
                }
            }

            return ret;
        }


        public async Task<Boolean> Insert(string fileName, UploadedImage imageToUpload)
        {
            bool ret = false;
            try
            {
                data.Image entity = new data.Image();
                entity.Caption = fileName;
                entity.ImageURL = imageToUpload.Url;
                entity.ThumbNailLarge = ImageService.ImageToByte(imageToUpload.Thumbnails[1].Bitmap);
                entity.ThumbNailSmall = ImageService.ImageToByte(imageToUpload.Thumbnails[0].Bitmap);
                entity.DateTaken = imageToUpload.DateTaken;
                entity.SignSize = imageToUpload.SignId;
                ret = Validate(entity);
                if (ret)
                {
                    using (nightowlsign_Entities db = new nightowlsign_Entities())
                    {
                        db.Images.Add(entity);
                        await db.SaveChangesAsync();
                        ret = true;
                    }
                }
                return ret;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                return ret;
            }
        }


        public bool Delete(data.Image entity)
        {
            bool ret = false;
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                db.Images.Attach(entity);
                db.Images.Remove(entity);
                db.SaveChanges();
                ret = true;
            }
            return ret;

        }
    }
}
