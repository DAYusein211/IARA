using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IARA.Business.DTOs;

namespace IARA.Business.Services
{
    public interface IStoreService
    {
        Task<StoreDto> GetStoreByIdAsync(Guid id);
        Task<IEnumerable<StoreDto>> GetAllStoresAsync();
        Task<StoreDto> CreateStoreAsync(CreateStoreDto createStoreDto);
        Task<StoreDto> UpdateStoreAsync(Guid id, CreateStoreDto updateStoreDto);
        Task DeleteStoreAsync(Guid id);
    }
}