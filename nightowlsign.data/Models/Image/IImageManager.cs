using System.Collections.Generic;

namespace nightowlsign.data.Models.Image
{
    public interface IImageManager
    {
        List<KeyValuePair<string, string>> ValidationErrors { get; set; }
        List<ImagesAndSign> Get(ImagesAndSign Entity);
        data.Image Find(int imageId);
        bool Validate(data.Image entity);
        bool Delete(data.Image entity);
    }
}