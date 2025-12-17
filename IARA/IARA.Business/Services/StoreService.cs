﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using IARA.Business.DTOs;
using IARA.Data;
using IARA.Domain.Entities;

namespace IARA.Business.Services
{
    public class StoreService : IStoreService
    {
        private readonly ApplicationDbContext _context;

        public StoreService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<StoreDto> GetStoreByIdAsync(Guid id)
        {
            var store = await _context.Stores.FindAsync(id);
            if (store == null) return null;

            return MapToDto(store);
        }

        public async Task<IEnumerable<StoreDto>> GetAllStoresAsync()
        {
            var stores = await _context.Stores.ToListAsync();
            return stores.Select(MapToDto);
        }

        public async Task<StoreDto> CreateStoreAsync(CreateStoreDto createStoreDto)
        {
            // Check for duplicate name
            var existingStore = await _context.Stores
                .FirstOrDefaultAsync(s => s.Name == createStoreDto.Name);
            
            if (existingStore != null)
                throw new Exception("A store with this name already exists.");

            var store = new Store
            {
                Id = Guid.NewGuid(),
                Name = createStoreDto.Name,
                Address = createStoreDto.Address,
                Manager = createStoreDto.Manager,
                Region = createStoreDto.Region,
                Territory = createStoreDto.Territory,
                District = createStoreDto.District,
                Settlement = createStoreDto.Settlement,
                CreatedAt = DateTime.UtcNow
            };

            _context.Stores.Add(store);
            await _context.SaveChangesAsync();

            return MapToDto(store);
        }

        public async Task<StoreDto> UpdateStoreAsync(Guid id, CreateStoreDto updateStoreDto)
        {
            var store = await _context.Stores.FindAsync(id);
            if (store == null) return null;

            // Check for duplicate name (excluding current store)
            var existingStore = await _context.Stores
                .FirstOrDefaultAsync(s => s.Name == updateStoreDto.Name && s.Id != id);
            
            if (existingStore != null)
                throw new Exception("A store with this name already exists.");

            store.Name = updateStoreDto.Name;
            store.Address = updateStoreDto.Address;
            store.Manager = updateStoreDto.Manager;
            store.Region = updateStoreDto.Region;
            store.Territory = updateStoreDto.Territory;
            store.District = updateStoreDto.District;
            store.Settlement = updateStoreDto.Settlement;
            store.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToDto(store);
        }

        public async Task DeleteStoreAsync(Guid id)
        {
            var store = await _context.Stores.FindAsync(id);
            if (store != null)
            {
                _context.Stores.Remove(store);
                await _context.SaveChangesAsync();
            }
        }


        private StoreDto MapToDto(Store store)
        {
            return new StoreDto
            {
                Id = store.Id,
                Name = store.Name,
                Address = store.Address,
                Manager = store.Manager,
                Region = store.Region,
                Territory = store.Territory,
                District = store.District,
                Settlement = store.Settlement
            };
        }
    }
}