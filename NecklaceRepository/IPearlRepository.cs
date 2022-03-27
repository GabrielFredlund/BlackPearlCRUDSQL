using NecklaceModels;

namespace NecklaceCRUD
{
    public interface IPearlRepository
    {
        Task<Pearl> CreateAsync(Pearl pearl);
        Task<IEnumerable<Pearl>> ReadAllAsync();
        Task<Pearl> ReadAsync(int pearlId);
        Task<Pearl> UpdateAsync(Pearl pearl);
        Task<Pearl> DeleteAsync(int pearlId);
    }
}