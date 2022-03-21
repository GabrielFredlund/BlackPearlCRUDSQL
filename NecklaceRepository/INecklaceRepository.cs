using NecklaceModels;

namespace NecklaceRepository
{
    public interface INecklaceRepository
    {
        Task<Necklace> CreateAsync(Necklace necklace);
        Task<IEnumerable<Necklace>> ReadAllAsync();
        Task<Necklace> ReadAsync(int necklaceId);
        Task<Necklace> UpdateAsync(Necklace necklaceId);
        Task<Necklace> DeleteAsync(int necklaceId);
    }
}