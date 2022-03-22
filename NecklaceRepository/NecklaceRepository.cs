using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NecklaceDB;
using NecklaceModels;

namespace NecklaceRepository
{
    public class NecklaceRepository : INecklaceRepository
    {
        NecklaceDbContext _db = null;


        public NecklaceRepository(NecklaceDbContext db)
        {
            _db = db;
        }

        // CREATE
        public async Task<Necklace> CreateAsync(Necklace necklace)
        {
            var added = await _db.Necklaces.AddAsync(necklace);

            int affected = await _db.SaveChangesAsync();
            if (affected == 1)
                return necklace;
            else
                return null;
        }

        //DELETE
        public async Task<Necklace> DeleteAsync(int necklaceId)
        {
            var cusDel = await _db.Necklaces.FindAsync(necklaceId);
            _db.Necklaces.Remove(cusDel);

            int affected = await _db.SaveChangesAsync();
            if (affected == 1)
                return cusDel;
            else
                return null;
        }

        // ReadAllAsync  // hör blir det fel
        public async Task<IEnumerable<Necklace>> ReadAllAsync()
        {
            return await Task.Run(() => _db.Necklaces);
        }

        // ReadAsync
        public async Task<Necklace> ReadAsync(int necklaceId)
        {
            return await _db.Necklaces.FindAsync(necklaceId);
        }

        // UpdateAsync
        public async Task<Necklace> UpdateAsync(Necklace necklaceId)
        {
            _db.Necklaces.Update(necklaceId); //No db interaction until SaveChangesAsync
            int affected = await _db.SaveChangesAsync();
            if (affected == 1)
                return necklaceId;
            else
                return null;
        }
    }
}
