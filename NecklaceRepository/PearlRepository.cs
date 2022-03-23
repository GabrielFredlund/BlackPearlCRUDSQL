using NecklaceDB;

using NecklaceModels;

namespace NecklaceRepository
{
    
    internal class PearlRepository : IPearlRepository
    {
        NecklaceDbContext _db = null;
        public Task<Pearl> CreateAsync(Pearl pearl)
        {
            throw new NotImplementedException();
        }

        public Task<Pearl> DeleteAsync(int pearlId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Pearl>> ReadAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Pearl> ReadAsync(int pearlId)
        {
            throw new NotImplementedException();
        }

        public async Task<Pearl> UpdateAsync(Pearl pearl)
        {
            _db.Pearls.Update(pearl);
            int affected = await _db.SaveChangesAsync();
            if (affected == 1)
                return pearl;
            else
                return null;
 
        }

        public PearlRepository(NecklaceDbContext db)
        {
            _db = db;
        }
    }
}
