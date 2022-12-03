using Microsoft.AspNetCore.Mvc;
using System.Text;
using UltraNuke.TaleSpin.Api.Services;
using UltraNuke.TaleSpin.Domain;

namespace UltraNuke.TaleSpin.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class TestController : ControllerBase
{

    private readonly ILogger<TestController> logger;

    public TestController(ILogger<TestController> logger)
    {
        this.logger = logger;
    }

    [HttpGet]
    public string GetExample()
    {
        var worldInitialStage = new List<Goal>{
            new Goal {ThingA="小鸟", State="家", ThingB="树" },
            new Goal {ThingA="水", State="在", ThingB="河" },
            new Goal {ThingA="蜂蜜", State="在", ThingB="树" },
            new Goal {ThingA="虫", State="在", ThingB="地面" },
            new Goal {ThingA="乔", State="在", ThingB="洞穴" },
            new Goal {ThingA="鱼", State="在", ThingB="河" },
            new Goal {ThingA="乔", State="是一个", ThingB="熊" },
            new Goal {ThingA="乔", State="家", ThingB="洞穴" },
            new Goal {ThingA="小鸟", State="是一个", ThingB="鸟" },
        };

        List<string> locations = new() { "洞穴", "树", "榆树", "地面", "河" };

        var story1 = new ProgramServices(worldInitialStage, locations, logger);

        var joeGoal = new Goal { ThingA = "乔", State = "饥饿", ThingB = "鱼" };
        story1.CreateCharacters("乔", "好", "快乐", "鱼", "洞穴", joeGoal, true);

        var birdyGoal = new Goal { ThingA = "小鸟", State = "口渴", ThingB = "水" };
        story1.CreateCharacters("小鸟", "好", "快乐", "虫", "树", birdyGoal, true);

        var s = story1.CreateStory();

        StringBuilder stringBuilder = new StringBuilder();
        if (s != null)
        {
            foreach (var g in s.InMemory)
            {
                var n = g.Negative ? "不" : "";
                stringBuilder.AppendLine($"{g.ThingA}{n}{g.State}{g.ThingB}");
            }
        }
        return stringBuilder.ToString();
    }
}