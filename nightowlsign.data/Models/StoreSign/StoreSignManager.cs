using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using nightowlsign.data.Models.Signs;
using nightowlsign.data;


namespace nightowlsign.data.Models.StoreSign
{
    public class StoreSignManager
    {
        public StoreSignManager()
        {
            ValidationErrors = new List<KeyValuePair<string, string>>();

        }
        //Properties
        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }

        public List<int?> Get(Store entity)
        {
            int storeId = entity.id;
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                var query = (from c in db.StoreSigns
                             where c.StoreId == storeId
                             select c.SignId);
                return query.ToList();
            }

        }

        public List<SelectListItem> GetAllSigns(int storeId)
        {
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                var query = (from s in db.Signs
                             orderby s.Model
                             select new SelectListItem() { SignId = s.id, Model = s.Model, StoreId = storeId });
                return query.ToList();
            }
        }


        public Store Find(int storeId)
        {
            Store ret = null;
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                ret = db.Store.Find(storeId);
            }
            return ret;

        }

        public void UpdateSignList(SelectListItem signSelect, Store store)
        {
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                data.StoreSign storeSign = new data.StoreSign()
                {
                    id = signSelect.Id,
                    SignId = signSelect.SignId,
                    StoreId = store.id,
                    DateUpdated = DateTime.Now,
                    InstallDate = DateTime.Now,
                    IPAddress = signSelect.IpAddress,
                    SubMask = signSelect.SubMask,
                    Port = signSelect.Port
                };
                if (signSelect.selected)
                {
                    db.Set<data.StoreSign>().AddOrUpdate(storeSign);
                    db.SaveChanges();
                }
                else
                {
                    List<data.StoreSign> storeSigns =
                    db.StoreSigns.Where(
                        x => x.SignId.Value == storeSign.SignId.Value && x.StoreId == storeSign.StoreId).ToList();
                    foreach (data.StoreSign sign in storeSigns)
                    {
                        db.StoreSigns.Attach(sign);
                        db.StoreSigns.Remove(sign);
                        db.SaveChanges();
                    }
                }
            }
        }

        internal bool IsSelected(int? signId, int storeId)
        {
            using (var db = new nightowlsign_Entities())
            {
                return db.StoreSigns.Any(x => x.StoreId == storeId && x.SignId == signId);
            }

        }

        internal data.StoreSign GetValues(SelectListItem signSelect)
        {
            //var storeSign = null;
            using (var db = new nightowlsign_Entities())
            {
                 return  db.StoreSigns.FirstOrDefault(x => x.StoreId == signSelect.StoreId && x.SignId == signSelect.SignId);
            }
          }

    }

}
