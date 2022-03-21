
using NecklaceModels;

namespace NecklaceRepository
{
    public class NecklaceRepository : INecklaceRepository
    {
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

        public Task<Necklace> UpdateAsync(Necklace necklaceId)
        {
            throw new NotImplementedException();
        }
    }
}
