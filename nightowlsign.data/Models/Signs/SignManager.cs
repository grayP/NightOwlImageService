using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nightowlsign.data.Models.Signs;

namespace nightowlsign.data.Models.Signs
{
    public class SignManager
    {
 
            public SignManager()
            {
                ValidationErrors = new List<KeyValuePair<string, string>>();
            }
            //Properties
            public List<KeyValuePair<string, string>> ValidationErrors { get; set; }

            public List<Sign> Get(Sign Entity)
            {
                List<Sign> ret = new List<Sign>();
                using (nightowlsign_Entities db = new nightowlsign_Entities())
                {
                    ret = db.Signs.OrderBy(x => x.Model).ToList<Sign>();
                }

                if (!string.IsNullOrEmpty(Entity.Model))
                {
                    ret = ret.FindAll(p => p.Model.ToLower().StartsWith(Entity.Model));
                }
                return ret;
            }

            public Sign Find(int signID)
            {
                Sign ret = null;
                using (nightowlsign_Entities db = new nightowlsign_Entities())
                {
                    ret = db.Signs.Find(signID);
                }
                return ret;
            }

            public bool Validate(Sign entity)
            {
                ValidationErrors.Clear();

                if (!string.IsNullOrEmpty(entity.Model))
                {
                    //if (entity.Model.ToLower() == entity.Model)
                    //{
                    //    ValidationErrors.Add(new KeyValuePair<string, string>("Sign Name", "Sign Name cannot be all lower case"));
                    //}

                }
                return (ValidationErrors.Count == 0);
            }


            public Boolean Update(Sign entity)
            {
                bool ret = false;
                if (Validate(entity))
                {
                    try
                    {
                        using (nightowlsign_Entities db = new nightowlsign_Entities())
                        {
                            db.Signs.Attach(entity);
                            var modifiedSign = db.Entry(entity);
                            modifiedSign.Property(e => e.Model).IsModified = true;
                            modifiedSign.Property(e => e.Height).IsModified = true;
                            modifiedSign.Property(e => e.Width).IsModified = true;
                            modifiedSign.Property(e => e.IPAddress).IsModified = true;
                            modifiedSign.Property(e => e.InstallDate).IsModified = true;

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

            public Boolean Insert(Sign entity)
            {
                bool ret = false;
                try
                {
                    ret = Validate(entity);
                    if (ret)
                    {
                        using (nightowlsign_Entities db = new nightowlsign_Entities())
                        {
                            Sign NewSign = new Sign()
                            {
                                Model = entity.Model,
                                Height = entity.Height,
                                Width = entity.Width,
                                InstallDate = entity.InstallDate,
                                IPAddress = entity.IPAddress
                            };

                            db.Signs.Add(NewSign);
                            db.SaveChanges();
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


            public bool Delete(Sign entity)
            {
                bool ret = false;
                using (nightowlsign_Entities db = new nightowlsign_Entities())
                {
                    db.Signs.Attach(entity);
                    db.Signs.Remove(entity);
                    db.SaveChanges();
                    ret = true;
                }
                return ret;

            }
        }

    }
