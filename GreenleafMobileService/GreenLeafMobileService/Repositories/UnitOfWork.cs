using System;
using GreenLeafMobileService.Models;

namespace GreenLeafMobileService.Repositories {
    public class UnitOfWork : IDisposable {
        private readonly GreenLeafMobileContext _context = new GreenLeafMobileContext();
        private CustomerRepository _customerRepository;
        private bool _disposed;
        private ItemRepository _itemRepository;
        private LocationRepository _locationRepository;

        public CustomerRepository CustomerRepository
            => _customerRepository
               ?? ( _customerRepository = new CustomerRepository( _context ) );

        public ItemRepository ItemRepository
            => _itemRepository
               ?? ( _itemRepository = new ItemRepository( _context ) );

        public LocationRepository LocationRepository
            => _locationRepository
               ?? ( _locationRepository = new LocationRepository( _context ) );

        public void Dispose () {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        protected virtual void Dispose ( bool disposing ) {
            if ( !_disposed ) {
                if ( disposing ) {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }
    }
}