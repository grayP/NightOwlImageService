using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nightowlsign.data;


namespace nightowlsign.data.Models.Image
{
    public class ImageManager : IImageManager
    {
        private  nightowlsign_Entities _context;
        public ImageManager()
        {
            _context = new nightowlsign_Entities();
            ValidationErrors = new List<KeyValuePair<string, string>>();
        }
        //Properties
        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }


        public List<ImagesAndSign> Get(ImagesAndSign entity)
        {
            List<ImagesAndSign> ret = new List<ImagesAndSign>();
            ret = _context.ImagesAndSigns.OrderBy(x => x.Model).ThenBy(x => x.Caption).ToList<ImagesAndSign>();
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
            return _context.Images.Find(imageId);
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
            _context.Images.Attach(entity);
            _context.Images.Remove(entity);
            _context.SaveChanges();
            return true;
        }
    }
}
