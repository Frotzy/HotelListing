using HotelListing.Data;
using HotelListing.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DatabaseContext _databaseContext;
        private readonly DbSet<T> _db;

        public GenericRepository(DatabaseContext databaseContext) {
            _databaseContext = databaseContext;
            _db = databaseContext.Set<T>();
        }
        public async Task Delete(int id)
        {
            var entity = await _db.FindAsync(id); // Finds a data with the 'id' as index
            _db.Remove(entity);  // Removes the entity found
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _db.RemoveRange(entities); // Removes the list of entities
        }

        public async Task<T> Get(System.Linq.Expressions.Expression<Func<T, bool>> expression, List<string> includes = null)
        {
            // The parameter 'expression' that is the condition of how we want to retrieve data (by Id, by name, etc)
            // the parameter 'includes' is optional, that is why the use of the expression '= null'

            IQueryable<T> query = _db; // Obtains all of the records of _db

            if (includes != null) {
                // It is going to include what the variable 'includeProperty' has
                foreach (var includeProperty in includes) {                    
                    query = query.Include(includeProperty); 
                }
            }

            // AsNoTracking = Any record that has been retrieve has not been tracked
            // FirstOrDefault = Gets the first or default record
            return await query.AsNoTracking().FirstOrDefaultAsync(expression);
        }

        public async Task<IList<T>> GetAll(System.Linq.Expressions.Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<string> includes = null)
        {
            IQueryable<T> query = _db; // Obtains the data of _db

            // If there is a condition, the query is going to filter
            if (expression != null) {
                query = query.Where(expression);
            }

            if (includes != null) {
                // query is going to include everythin that 'includeProperty' has
                foreach (var includeProperty in includes){
                    query = query.Include(includeProperty);
                }
            }

            // If 'orderBy' is not null, it orders the query
            if (orderBy != null) {
                query = orderBy(query);
            }
            return await query.AsNoTracking().ToListAsync();
        }

        public async Task Insert(T entity)
        {
            await _db.AddAsync(entity);
        }

        public async Task InsertRange(IEnumerable<T> entities)
        {
            await _db.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            _db.Attach(entity);
            _databaseContext.Entry(entity).State = EntityState.Modified;
        }
    }

}
