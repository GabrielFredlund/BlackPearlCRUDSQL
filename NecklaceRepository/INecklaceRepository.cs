using NecklaceModels;

namespace NecklaceCRUD
{
    public interface INecklaceRepository
    {
        Task<Necklace> CreateAsync(Necklace necklace);
        Task<IEnumerable<Necklace>> ReadAllAsync();
        Task<IEnumerable<Necklace>> ReadAllWithoutPearlsAsync();
        Task<Necklace> ReadAsync(int necklaceId);
        Task<Necklace> UpdateAsync(Necklace necklaceId);
        Task<Necklace> DeleteAsync(int necklaceId);
    }
}