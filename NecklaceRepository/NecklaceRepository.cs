using NecklaceDB;
using NecklaceModels;
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
            var necklaces = await Task.Run(() => _db.Necklaces);
            var pearls = _db.Pearls.ToList();
            return necklaces;
        }

        public async Task<Necklace> ReadAsync(int necklaceId)
        {
            var necklace = await _db.Necklaces.FindAsync(necklaceId);
            var pearls = _db.Pearls.ToList();
            return necklace;
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
