using Microsoft.AspNetCore.Mvc;
using UltraNuke.TaleSpin.Application.DTO;

namespace UltraNuke.TaleSpin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoriesController : ControllerBase
    {
        [HttpPost]
        public void Create(StoryForCreateParam param)
        {

        }
    }
}
