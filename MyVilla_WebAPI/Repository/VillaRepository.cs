using Microsoft.EntityFrameworkCore;
using MyVilla_WebAPI.Data;
using MyVilla_WebAPI.Models;
using MyVilla_WebAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MyVilla_WebAPI.Repository
{
    public class VillaRepository : Repository<Villa>, IVillaRepository
    {
        private readonly ApplicationDbContext _db;
        public VillaRepository(ApplicationDbContext db) 
            : base(db)
        {
            _db = db;
        }
        public async Task<Villa> UpdateVillaAsync(Villa entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _db.Villas.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        //public async Task<List<Villa>> GetAllVillaAsync(Expression<Func<Villa, bool>> filter = null)
        //{
        //    IQueryable<Villa> query = _db.Villas;
        //    if (filter != null)
        //        query = query.Where(filter);

        //    return await query.ToListAsync();
        //}

        //public async Task<Villa> GetVillaAsync(Expression<Func<Villa, bool>> filter = null, bool isTracked = true)
        //{
        //    IQueryable<Villa> query = _db.Villas;
        //    if (!isTracked)
        //        query = query.AsNoTracking();
        //    if (filter != null)
        //        query = query.Where(filter);

        //    return await query.FirstOrDefaultAsync();
        //}

        //public async Task CreateVillaAsync(Villa entity)
        //{
        //    await _db.Villas.AddAsync(entity);
        //    await SaveAsync();
        //}

        //public async Task UpdateVillaAsync(Villa entity)
        //{
        //    _db.Villas.Update(entity);
        //    await SaveAsync();
        //}

        //public async Task RemoveVillaAsync(Villa entity)
        //{
        //    _db.Villas.Remove(entity);
        //    await SaveAsync();
        //}

        //public async Task SaveAsync()
        //{
        //    await _db.SaveChangesAsync();
        //}
    }
}
