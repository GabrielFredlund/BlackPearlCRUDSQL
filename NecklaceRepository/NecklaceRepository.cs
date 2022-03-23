using NecklaceDB;
using NecklaceModels;

namespace NecklaceRepository
{
    public class NecklaceRepository : INecklaceRepository
    {
        NecklaceDbContext _db = null;
        public Task<Necklace> CreateAsync(Necklace necklace)
        {
            throw new NotImplementedException();
        }

        public Task<Necklace> DeleteAsync(int necklaceId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Necklace>> ReadAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Necklace> ReadAsync(int necklaceId)
        {
            throw new NotImplementedException();
        }

        public async Task<Necklace> UpdateAsync(Necklace NecklaceList)
        {          
            _db.Necklaces.Update(NecklaceList);
            int affected = await _db.SaveChangesAsync();
            if (affected == 1)
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
