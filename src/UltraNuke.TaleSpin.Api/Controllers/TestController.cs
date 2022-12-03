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
            new Goal {ThingA="С��", State="��", ThingB="��" },
            new Goal {ThingA="ˮ", State="��", ThingB="��" },
            new Goal {ThingA="����", State="��", ThingB="��" },
            new Goal {ThingA="��", State="��", ThingB="����" },
            new Goal {ThingA="��", State="��", ThingB="��Ѩ" },
            new Goal {ThingA="��", State="��", ThingB="��" },
            new Goal {ThingA="��", State="��һ��", ThingB="��" },
            new Goal {ThingA="��", State="��", ThingB="��Ѩ" },
            new Goal {ThingA="С��", State="��һ��", ThingB="��" },
        };

        List<string> locations = new() { "��Ѩ", "��", "����", "����", "��" };

        var story1 = new ProgramServices(worldInitialStage, locations, logger);

        var joeGoal = new Goal { ThingA = "��", State = "����", ThingB = "��" };
        story1.CreateCharacters("��", "��", "����", "��", "��Ѩ", joeGoal, true);

        var birdyGoal = new Goal { ThingA = "С��", State = "�ڿ�", ThingB = "ˮ" };
        story1.CreateCharacters("С��", "��", "����", "��", "��", birdyGoal, true);

        var s = story1.CreateStory();

        StringBuilder stringBuilder = new StringBuilder();
        if (s != null)
        {
            foreach (var g in s.InMemory)
            {
                var n = g.Negative ? "��" : "";
                stringBuilder.AppendLine($"{g.ThingA}{n}{g.State}{g.ThingB}");
            }
        }
        return stringBuilder.ToString();
    }
}