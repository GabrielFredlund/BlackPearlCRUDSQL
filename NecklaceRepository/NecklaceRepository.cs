using NecklaceDB;
using NecklaceModels;
using Microsoft.EntityFrameworkCore;

namespace NecklaceCRUD
{
    public class NecklaceRepository : INecklaceRepository
    {
        NecklaceDbContext _db = null;
        public async Task<Necklace> CreateAsync(Necklace necklace)
        {
            await _db.Necklaces.AddAsync(necklace);

            int affected = await _db.SaveChangesAsync();
            if (affected >= 1)
                return necklace;
            else
                return null;

        }

        public async Task<Necklace> DeleteAsync(int necklaceId)
        {
            var cusDel = await _db.Necklaces.FindAsync(necklaceId);
            _db.Necklaces.Remove(cusDel);

            int affected = await _db.SaveChangesAsync();
            if (affected >= 1)
                return cusDel;
            else
                return null;
        }

        public async Task<IEnumerable<Necklace>> ReadAllAsync()
        {
            // Force pearls?
            var pearls = _db.Pearls.ToList();
            return await Task.Run(() => _db.Necklaces);
        }

        public async Task<IEnumerable<Necklace>> ReadAllWithoutPearlsAsync()
        {
            return await Task.Run(() => _db.Necklaces);
        }

        public async Task<Necklace> ReadAsync(int necklaceId)
        {
            var pearls = _db.Pearls.ToList();
            var neck = await _db.Necklaces.FindAsync(necklaceId);


            if (neck != null)
            {
                return neck;
            }

            else
            {
                return null;
            }
        }

        public async Task<Necklace> UpdateAsync(Necklace NecklaceList)
        {
            _db.Necklaces.Update(NecklaceList);
            int affected = await _db.SaveChangesAsync();
            if (affected >= 1)
                return NecklaceList;
            else
                return null;
        }
        public NecklaceRepository(NecklaceDbContext db)
        {
            _db = db;
        }
    }
}
