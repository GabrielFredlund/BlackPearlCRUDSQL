

using NecklaceModels;

namespace NecklaceRepository
{
    internal class PearlRepository : IPearlRepository
    {
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

        public Task<Pearl> UpdateAsync(Pearl pearl)
        {
            throw new NotImplementedException();
        }
    }
}
