using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace nightowlsign.data.Models
{
    public class StoreSignIPManager
    {
        public StoreSignIPManager()
        {
            ValidationErrors = new List<KeyValuePair<string, string>>();

        }
        //Properties
        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }

        public StoreSignIP Get(StoreSignIP Entity)
        {
            var ret = new StoreSignIP();
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
               return ret = db.StoreSignIPs.Find(Entity.id);
            }
      }



        public StoreSignIP Find(int storeSignIpId)
        {
            StoreSignIP ret = null;
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                ret = db.StoreSignIPs.Find(storeSignIpId);
            }
            return ret;

        }

        public void UpdateSignList(SignSelect signSelect, Store store)
        {
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
               StoreSign storeSign = new StoreSign()
                {
                    SignId = signSelect.Id,
                    StoreId = store.id,
                    DateUpdated = DateTime.Now

                };
                if (signSelect.selected)
                {
                    db.Set<StoreSign>().AddOrUpdate(storeSign);
                    db.SaveChanges();
                }
                else
                {
                    if (db.StoreSigns.Any(x => x.SignId == storeSign.SignId && x.StoreId == storeSign.StoreId))
                    {
                        db.StoreSigns.Attach(storeSign);
                        db.StoreSigns.Remove(storeSign);
                        db.SaveChanges();
                    }
                }
            }
        }

        internal bool IsSelected(int signId, int storeId)
        {
            bool ret = false;
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                ret = db.StoreSigns.Any(x => x.StoreId == storeId && x.SignId == signId);
            }
            return ret;
        }

       
    }

}
