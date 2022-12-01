using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace UltraNuke.TaleSpin.Domain;

/// <summary>
/// 演员
/// </summary>
public class Actor
{
    private readonly ILogger _logger;

    public Actor(string name, string socialRelation, string currentFeelingState, string food, string currentLocation, Goal goal, bool honest, ILogger logger)
    {
        _logger = logger;

        Name = name;
        SocialRelation = socialRelation;
        CurrentFeelingState = currentFeelingState;
        Food = food;
        Location = currentLocation;
        Goal = goal;
        Honest = honest;
        InMemoryGoal.Add(goal);

        _logger.LogInformation($"{name}-{socialRelation}-{currentFeelingState}-{food}-{currentLocation}-{JsonConvert.SerializeObject(goal)}");
    }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { set; get; }
    /// <summary>
    /// 社交关系
    /// </summary>
    public string SocialRelation { set; get; }
    /// <summary>
    /// 当前感觉状态
    /// </summary>
    public string CurrentFeelingState { set; get; }
    /// <summary>
    /// 食物
    /// </summary>
    public string Food { set; get; }
    /// <summary>
    /// 位置
    /// </summary>
    public string Location { set; get; }
    /// <summary>
    /// 目标
    /// </summary>
    public Goal Goal { set; get; }
    /// <summary>
    /// 诚实的
    /// </summary>
    public bool Honest { set; get; }
    /// <summary>
    /// 内存中的目标
    /// </summary>
    public List<Goal> InMemoryGoal { set; get; } = new List<Goal>();
}
