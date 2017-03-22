using System.Collections.Generic;
using System.Linq;

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


        public List<ImagesAndSign> Get(ImagesAndSign entity)
        {
            var ret = new List<ImagesAndSign>();
            using (var db = new nightowlsign_Entities())
            {
                ret = db.ImagesAndSigns.OrderBy(x => x.Model).ThenBy(x => x.Caption).ToList<ImagesAndSign>();
            }
            if (!string.IsNullOrEmpty(entity.Caption))
            {
                ret = ret.FindAll(p => p.Caption.ToLower().StartsWith(entity.Caption));
            }
            if (entity.SignSize > 0)
            {
                ret = ret.FindAll(p => p.SignSize.Equals(entity.SignSize));
            }
           return ret;
        }

        public data.Image Find(int imageId)
        {
            using (var db = new nightowlsign_Entities())
            {
                return db.Images?.Find(imageId);
            }
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


        public bool Delete(data.Image entity)
        {
            using (var db = new nightowlsign_Entities())
            {
                db.Images.Attach(entity);
                db.Images.Remove(entity);
                db.SaveChanges();
                return true;
            }
        }
    }
}
