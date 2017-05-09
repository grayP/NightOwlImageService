using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Threading;
using System.Threading.Tasks;

namespace nightowlsign.data
{
    public interface Inightowlsign_Entities
    {
        DbSet<Sign> Signs { get; set; }
        DbSet<Store> Store { get; set; }
        DbSet<Logging> Logging { get; set; }
        DbSet<Image> Images { get; set; }
        DbSet<Schedule> Schedules { get; set; }
        DbSet<ScheduleStore> ScheduleStores { get; set; }
        DbSet<ScheduleImage> ScheduleImages { get; set; }
        DbSet<ImagesAndSign> ImagesAndSigns { get; set; }
        DbSet<StoreAndSign> StoreAndSigns { get; set; }
        DbSet<ScheduleAndSign> ScheduleAndSigns { get; set; }
        DbSet<StoreScheduleLog> StoreScheduleLogs { get; set; }
        Database Database { get; }
        DbChangeTracker ChangeTracker { get; }
        DbContextConfiguration Configuration { get; }
        ObjectResult<FindCurrentPlayList_Result> FindCurrentPlayList();
        ObjectResult<FindCurrentPlayListForStore_Result> FindCurrentPlayListForStore();
        //DbSet Set() where TEntity : class;
        DbSet Set(Type entityType);
        int SaveChanges();
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        IEnumerable<DbEntityValidationResult> GetValidationErrors();
        //DbEntityEntry Entry(TEntity entity) where TEntity : class;
        DbEntityEntry Entry(object entity);
        void Dispose();
        string ToString();
        bool Equals(object obj);
        int GetHashCode();
        Type GetType();
        object Set<T>();
    }
}