using System.Text;
using System.Threading.Tasks;
using NecklaceDB;
using NecklaceModels;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NecklaceModels;

namespace NecklaceCRUD
{

    public class PearlRepository : IPearlRepository
    {
        NecklaceDbContext _db = null;
        public Task<Pearl> CreateAsync(Pearl pearl)
        {
            throw new NotImplementedException();
        }

        public async Task<Pearl> DeleteAsync(int pearlId)
        {
            var cusDel = await _db.Pearls.FindAsync(pearlId);// FindAsync är inte eget
            _db.Pearls.Remove(cusDel);

            int affected = await _db.SaveChangesAsync();// SaveChangesAsync är inte eget
            if (affected == 1)
                return cusDel;
            else
                return null;
        }

        public async Task<IEnumerable<Pearl>> ReadAllAsync()
        {
            return await Task.Run(() => _db.Pearls);
        }

        public async Task<Pearl> ReadAsync(int pearlId)
        {
            return await _db.Pearls.FindAsync(pearlId);
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
