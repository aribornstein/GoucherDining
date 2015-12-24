using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using GoucherDiningService.DataObjects;
using GoucherDiningService.Models;

namespace GoucherDiningService.Controllers
{
    public class EntreController : TableController<Entre>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            GoucherDiningServiceContext context = new GoucherDiningServiceContext();
            DomainManager = new EntityDomainManager<Entre>(context, Request, Services);
        }

        // GET tables/Entre
        public IQueryable<Entre> GetAllEntre()
        {
            return Query(); 
        }

        // GET tables/Entre/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Entre> GetEntre(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Entre/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Entre> PatchEntre(string id, Delta<Entre> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/Entre/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<IHttpActionResult> PostEntre(Entre item)
        {
            Entre current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Entre/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteEntre(string id)
        {
             return DeleteAsync(id);
        }

    }
}