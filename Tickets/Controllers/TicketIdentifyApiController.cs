using System.Web.Http;

namespace Tickets.Controllers
{
    [Authorize]
    [RoutePrefix("ticket/identifyBachApi")]
    public class TicketIdentifyApiController : ApiController
    {

    }
}
