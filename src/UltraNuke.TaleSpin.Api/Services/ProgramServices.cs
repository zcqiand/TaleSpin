using UltraNuke.TaleSpin.Domain;

namespace UltraNuke.TaleSpin.Api.Services;

public class ProgramServices
{
    private readonly ILogger _logger;

    public ProgramServices(List<Goal> worldInitialStage, List<string> locations, ILogger logger)
    {
        _logger = logger;

        InitialStage = worldInitialStage;
        InMemory = InitialStage;
        Locations = locations;
    }

    public List<Goal> InitialStage { set; get; } = new List<Goal>();

    public List<Goal> InMemory { set; get; } = new List<Goal>();
    public List<Actor> StoryCharactersList { set; get; } = new List<Actor>();

    public List<string> Locations { set; get; }

    /// <summary>
    /// 创建故事人物
    /// 为故事定义全局角色并设置它们的属性。
    /// </summary>
    /// <param name="name"></param>
    /// <param name="socialRelation"></param>
    /// <param name="currentFeelingState"></param>
    /// <param name="food"></param>
    /// <param name="currentLocation"></param>
    /// <param name="goal"></param>
    /// <param name="honest"></param>
    public void CreateCharacters(string name, string socialRelation, string currentFeelingState, string food, string currentLocation, Goal goal, bool honest)
    {
        var newActor = new Actor(name, socialRelation, currentFeelingState, food, currentLocation, goal, honest, _logger);
        StoryCharactersList.Add(newActor);
    }

    /// <summary>
    /// 生成故事
    /// 从将目标添加到目标堆栈并执行 problemSolver 开始
    /// </summary>
    public Story? CreateStory()
    {
        Story? newStory = null;
        foreach (var i in StoryCharactersList)
        {
            newStory = new Story(i, InMemory, StoryCharactersList, Locations, _logger);
        }
        return newStory ?? null;
    }
}
