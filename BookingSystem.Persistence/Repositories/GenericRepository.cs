using BookingSystem.Application.Interfaces.GenericRepo;
using BookingSystem.Domain.Common;
using BookingSystem.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly DataContext _context;

    public GenericRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<T> AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        return entity;
    }

    public void Delete(T entity) =>
         _context.Set<T>().Remove(entity);

    public async Task<IReadOnlyList<T>> GetAllAsync() =>
        await _context.Set<T>().ToListAsync();

    public async Task<T?> GetByIdAsync(Guid id) =>
        await _context.Set<T>().FirstOrDefaultAsync(e => e.Id == id);

    public void Update(T entity) =>
          _context.Set<T>().Update(entity);
}
