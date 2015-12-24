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
    public class DiningHallController : TableController<DiningHall>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            GoucherDiningServiceContext context = new GoucherDiningServiceContext();
            DomainManager = new EntityDomainManager<DiningHall>(context, Request, Services);
        }

        // GET tables/DiningHall
        public IQueryable<DiningHall> GetAllDiningHall()
        {
            return Query(); 
        }

        // GET tables/DiningHall/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<DiningHall> GetDiningHall(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/DiningHall/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<DiningHall> PatchDiningHall(string id, Delta<DiningHall> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/DiningHall/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<IHttpActionResult> PostDiningHall(DiningHall item)
        {
            DiningHall current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/DiningHall/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteDiningHall(string id)
        {
             return DeleteAsync(id);
        }

    }
}