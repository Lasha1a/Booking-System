using BookingSystem.Application.Interfaces.GenericRepo;
using BookingSystem.Application.Interfaces.Specifications;
using BookingSystem.Domain.Common;
using BookingSystem.Persistence.Data;
using BookingSystem.Persistence.Specifications;
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

    public IQueryable<T> GetAll() =>
        _context.Set<T>().AsQueryable();

    public async Task<T?> GetByIdAsync(Guid id) =>
        await _context.Set<T>().FirstOrDefaultAsync(e => e.Id == id);

    public void Update(T entity) =>
          _context.Set<T>().Update(entity);

    public async Task<int> SaveChangesAsync() =>
          await _context.SaveChangesAsync();

    //using the specification pattern to get an entity based on the criteria defined in the specification
    public async Task<T?> GetEntityWithSpec(ISpecification<T> spec) =>
        await SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), spec)
        .FirstOrDefaultAsync();

    //using the specification pattern to get a list of entities based on the criteria defined in the specification
    public IQueryable<T> GetQueryWithSpec(ISpecification<T> spec) =>
        SpecificationEvaluator<T>.GetQuery(_context.Set<T>(), spec);
}
