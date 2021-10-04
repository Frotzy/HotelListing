using HotelListing.Data;
using HotelListing.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _dabaseContext;
        private IGenericRepository<Country> _countries;
        private IGenericRepository<Hotel> _hotels;

        public UnitOfWork(DatabaseContext databaseContext)
        {
            _dabaseContext = databaseContext;
        }
        public IGenericRepository<Country> Countries => _countries ??= new GenericRepository<Country>(_dabaseContext);

        public IGenericRepository<Hotel> Hotels => _hotels ??= new GenericRepository<Hotel>(_dabaseContext);

        public void Dispose()
        {
            _dabaseContext.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task Save()
        {
            await _dabaseContext.SaveChangesAsync();
        }
    }
}
