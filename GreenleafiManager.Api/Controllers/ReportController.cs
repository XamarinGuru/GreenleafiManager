using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GreenleafiManager.Api.Models;
using GreenleafiManager.Api.ViewModels;
using Microsoft.Azure.Mobile.Server.Authentication;
using System;

namespace GreenleafiManager.Api.Controllers {
    [RoutePrefix ( "api/reports" )]
    //[AuthorizeLevel ( AuthorizationLevel.User )]
    public class ReportController : ApiController {
        private readonly MobileServiceContext _context = new MobileServiceContext();

        [HttpGet]
        [Route ( "stockReport" )]
        public HttpResponseMessage GetStockReport ( HttpRequestMessage request, string locationId ) {
            var location = _context.Locations.SingleOrDefault( l => l.Id == locationId );

            if ( location == null ) {
                return request.CreateResponse( HttpStatusCode.BadRequest );
            }

            var data = new StockReportViewModel {
                ItemAmount = location.Items.Count( item => !item.IsSold ),
                TotalCost = location.Items.Where( item => !item.IsSold ).Sum( i => i.Price ),
                TotalTagPrice = location.Items.Where( item => !item.IsSold ).Sum( i => i.TagPrice )
            };

            return request.CreateResponse( HttpStatusCode.OK, data );
        }

        [HttpPost]
        [Route ( "salesReport" )]
        public HttpResponseMessage GetSalesReport ( HttpRequestMessage request, SalesReportFiltersViewModel filters ) {
            if ( !ModelState.IsValid || filters == null ) {
                return request.CreateResponse( HttpStatusCode.BadRequest );
            }

            var invoicesByFilters = _context.Invoices
                .Where( inv => inv.LocationId == filters.LocationId
                               && inv.CustomerId == filters.CustomerId
                               && DateTime.Parse(inv.Date) >= filters.DateFrom
                               && DateTime.Parse(inv.Date) <= filters.DateTo
                               && inv.Items.Any() )
                .ToList();

            var location = _context.Locations.FirstOrDefault( l => l.Id == filters.LocationId );
            var customer = _context.Customers.FirstOrDefault( c => c.Id == filters.CustomerId );

            var data = new SalesReportViewModel {
                TotalCost = invoicesByFilters.Select( i => i.Items ).Sum( items => items.Sum( item => item.Price ) ),
                DateTo = filters.DateTo,
                DateFrom = filters.DateFrom,
                LocationName = location != null ? location.Name : string.Empty,
                CustomerFullName = customer != null ? $"{customer.FirstName} {customer.LastName}" : string.Empty,
                ItemsSold = invoicesByFilters.Select( i => i.Items ).Count()
            };

            return request.CreateResponse( HttpStatusCode.OK, data );
        }
    }
}