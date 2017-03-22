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
