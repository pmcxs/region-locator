using Microsoft.AspNetCore.Mvc;
using RegionLocator.Core;

namespace RegionLocator.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly IRegionsLookupHandler _handler;

        public RegionsController(IRegionsLookupHandler handler)
        {
            this._handler = handler;
        }

        // GET api/Regions/byCoordinates
        [HttpGet("byCoordinates")]
        public ActionResult<Region> Get(string longitude, string latitude)
        {
            double lon = double.Parse(longitude);
            double lat = double.Parse(latitude);

            Region dma = _handler.GetRegion(lon, lat);

            if(dma == null)
            {
                return NotFound();
            }

            return dma;
        }
    }
}
