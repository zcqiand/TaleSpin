using Serilog;
using System.Collections.Generic;
using System.Reflection;

namespace UltraNuke.TaleSpin.Domain.AggregatesModel
{
    public class Actor : Thing
    {
        private static readonly ILogger logger = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
        public Actor(string name, string socialRelation, string currentFeelingState, Thing food, Thing location, Goal goal, bool honest)
        {
            Name = name;
            SocialRelation = socialRelation;
            CurrentFeelingState = currentFeelingState;
            Food = food;
            Location = location;
            Goal = goal;
            InMemoryGoal.Add(goal);
            Honest = honest;

            logger.Debug($"{name}-{socialRelation}-{currentFeelingState}-{food.Name}-{location.Name}-{goal}");
        }
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
        public Thing Food { set; get; }
        /// <summary>
        /// 目标
        /// </summary>
        public Goal Goal { set; get; }
        /// <summary>
        /// 内存中的目标
        /// </summary>
        public List<Goal> InMemoryGoal { set; get; } = new List<Goal>();
        /// <summary>
        /// 诚实的
        /// </summary>
        public bool Honest { set; get; }
    }
}
