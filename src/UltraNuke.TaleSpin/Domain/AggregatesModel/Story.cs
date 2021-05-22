using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UltraNuke.TaleSpin.Domain.AggregatesModel
{
    public class Story
    {
        private static readonly ILogger logger = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        public Story(Actor mainActor, List<Goal> inMemory, List<Actor> storyCharactersList, List<string> locations)
        {
            MainActor = mainActor;
            inMemory.Add(new Goal { A = "Story starts" });
            InMemory = inMemory;
            StoryCharactersList = storyCharactersList;
            Locations = locations;
            ProblemSolver();
        }

        /// <summary>
        /// 主角
        /// </summary>
        public Actor MainActor { set; get; }

        /// <summary>
        /// 目标集合
        /// </summary>
        public List<Goal> InMemory { set; get; }

        /// <summary>
        /// 角色列表
        /// </summary>
        public List<Actor> StoryCharactersList { set; get; }

        /// <summary>
        /// 位置
        /// </summary>
        public List<string> Locations { set; get; }

        /// <summary>
        /// 问题解决
        /// 执行每个计划，直到一个计划可以执行并且目标可以被删除为止，或者直到一个都没有执行并且角色未能达到目标为止。 如果目标已经实现（演员知道），则立即返回成功。 如果演员已经有了目标，那么他就陷入了循环并且失败了。 否则，请设定目标并继续前进。
        /// </summary>
        public void ProblemSolver()
        {
            while (MainActor.InMemoryGoal.Count > 0)
            {
                ExecutePlans();
                if (InMemory.Where(w => w.A.Contains("目标未实现")).Any())
                {
                    break;
                }
            }
            InMemory.Add(new Goal { A = "THE END" });
            logger.Information("----------Stack Memory result story -----------------");
            logger.Debug($"{string.Join(",", InMemory.Select(s => s.FullGoal))}");
        }

        /// <summary>
        /// 执行计划
        /// 该功能执行可用于实现目标的所有计划行动。
        /// </summary>
        public void ExecutePlans()
        {
            var goal2 = MainActor.InMemoryGoal[0].State;
            var lent = MainActor.InMemoryGoal.Count;
            var a = MainActor.InMemoryGoal;
            if (goal2 == "口渴")
            {
                SigmaThirst();
            }
            if (goal2 == "饥饿")
            {
                SigmaHunger();
            }
            if (goal2 == "休息")
            {
                SigmaRest();
            }
            if (goal2 == "想成为")
            {
                DeltaProx();
            }
            if (goal2 == "想知道")
            {
                DeltaKnow();
            }
            if (goal2 == "告诉")
            {
                DeltaTell();
            }
            if (goal2 == "询问信息")
            {
                DeltaAsk();
            }
        }

        /// <summary>
        /// 口渴
        /// 为了满足口渴，请去喝点水喝。
        /// 前提条件：[演员，'在'，'河']
        /// 后置条件：
        /// 功效：演员喝水，演员不渴
        /// </summary>
        public void SigmaThirst()
        {
            if (MainActor.InMemoryGoal.Count == 0) return;

            var precondition = new Goal { A = MainActor, State = "在", ThingB = "河" };
            logger.Debug("口渴！！！！！");

            var effect = DeepCopy(MainActor.InMemoryGoal[0]);
            effect.Negative = true;
            logger.Information(effect.ToString());

            if (FindGoal(InMemory, precondition) != null)
            {
                var consequence = new List<Goal> {
                        new Goal { A = MainActor, State = "知道可以喝", ThingB = "水" },
                        new Goal { A = MainActor, State = "喝", ThingB = "水" },
                        effect
                    };
                AssertionMechanism(consequence, effect);
            }
            else
            {
                MainActor.InMemoryGoal.Insert(0, new Goal { A = MainActor, State = "想成为", ThingB = "河" });
                logger.Debug("alternate");
            }
        }

        /// <summary>
        /// 饥饿
        /// 为了满足饥饿感，您可以看到食物，然后把食物上吃掉。
        /// 前提条件：角色所在的地方有食物例如，熊可以在洞穴里看到食物，他没有身体可以吃
        /// 后置条件：
        /// 功效：演员吃东西，演员不饿
        /// </summary>
        public void SigmaHunger()
        {
            if (MainActor.InMemoryGoal.Count == 0) return;

            var newGoal = MainActor.InMemoryGoal[0];
            var precondition1 = new Goal { A = MainActor.Food, State = "在", ThingB = MainActor.Location };
            var precondition2 = new Goal { A = MainActor.Name, State = "知道", ThingB = newGoal.ThingB };
            var precondition3 = new Goal { A = StoryCharactersList[1].Name, State = "说谎", ThingB = MainActor.Name };

            logger.Debug($"饥饿！！！！！{MainActor.InMemoryGoal.Count}");
            logger.Debug(MainActor.Location);

            var effect = DeepCopy(MainActor.InMemoryGoal[0]);
            effect.Negative = true;
            if (FindGoal(InMemory, precondition1) != null)
            {
                if (FindGoal(InMemory, precondition3) != null)
                {
                    var consequence = new List<Goal> {
                            new Goal { A = MainActor.Name, State = "知道食物的位置", ThingB = MainActor.Food },
                            new Goal { A = MainActor.Name, State = "吃", ThingB = MainActor.Food },
                            effect
                        };
                    AssertionMechanism(consequence, effect);
                }
                else
                {
                    var consequence = new List<Goal> {
                            new Goal { A = MainActor.Name, State = "说谎"},
                            new Goal { A = "目标未实现"}
                        };
                    AssertionMechanism(consequence, null);
                }
            }
            else if (FindGoal(InMemory, precondition2) != null)
            {
                var location = SearchObject(InMemory, newGoal.ThingB);
                var moveToLocation = new Goal { A = MainActor.Name, State = "想成为", ThingB = location };
                MainActor.InMemoryGoal.Insert(0, moveToLocation);
            }
            else
            {
                var goal = new Goal { A = MainActor.Name, State = "想知道", ThingB = MainActor.Food };
                MainActor.InMemoryGoal.Insert(0, goal);
                logger.Debug("alternate");
            }
        }

        /// <summary>
        /// 休息
        /// 为了满足休息，必须多吃。
        /// 前提条件：角色所在的地方有食物，例如，熊可以在他的洞穴里看到食物
        /// 后置条件：[演员，“又饱又累”]
        /// 功效： 
        /// </summary>
        public void SigmaRest()
        {
            if (MainActor.InMemoryGoal.Count == 0) return;

            var precondition1 = new Goal { A = MainActor.Name, State = "想休息" };
            var newGoal = new Goal { A = MainActor.Name, State = "饥饿", ThingB = MainActor.Food };

            logger.Debug($"休息！！！！！{MainActor.InMemoryGoal.Count}");
            logger.Debug(MainActor.Location);

            var effect = DeepCopy(MainActor.InMemoryGoal[0]);
            effect.Negative = true;

            if (FindGoal(InMemory, precondition1) != null)
            {
                var consequence = new List<Goal> {
                            new Goal { A = MainActor.Name, State = "在休息", ThingB = MainActor.Location}
                        };
                AssertionMechanism(consequence, effect);
            }
            else
            {
                MainActor.InMemoryGoal.Insert(0, newGoal);
                var consequence = new List<Goal> {
                            precondition1
                        };
                AssertionMechanism(consequence, null);
            }
        }

        /// <summary>
        /// 移动
        /// 将对象（包括您自己）移动到其他位置
        /// 人或对象是：获取第一个对象（如果不是您自己）
        /// 前提条件：移动目标在堆栈中
        /// 后置条件：
        /// 效果：演员移至要求的位置，目标已完成。
        /// </summary>
        public void DeltaProx()
        {
            if (MainActor.InMemoryGoal.Count == 0) return;

            var newGoal = MainActor.InMemoryGoal[0];

            logger.Debug($"Delta-Prox");
            logger.Information(newGoal.ToString());
            if (FindGoal(InMemory, newGoal) == null)
            {
                var effect = DeepCopy(MainActor.InMemoryGoal[0]);
                effect.Negative = true;

                var moveToLocation1 = new Goal { A = MainActor.Name, State = "移至", ThingB = newGoal.ThingB };
                var moveToLocation2 = new Goal { A = MainActor.Name, State = "在", ThingB = newGoal.ThingB };
                MainActor.Location = newGoal.ThingB;

                var consequence = new List<Goal> {
                            moveToLocation1,
                            moveToLocation2
                        };
                AssertionMechanism(consequence, effect);
            }

        }

        /// <summary>
        /// 知道
        /// 要找出某事：找一个朋友告诉你
        /// 前提条件：演员需要答案对象[演员想知道对象是]
        /// </summary>
        public void DeltaKnow()
        {
            if (MainActor.InMemoryGoal.Count == 0) return;

            var newGoal = MainActor.InMemoryGoal[0];

            if (FindGoal(InMemory, newGoal) == null)
            {
                logger.Debug($"delta know");
                logger.Information(newGoal.ToString());

                var effect = DeepCopy(MainActor.InMemoryGoal[0]);
                effect.Negative = true;

                logger.Debug($"{StoryCharactersList.Count}");

                if (MainActor.Location == StoryCharactersList[1].Location)
                {
                    var askActorforInformation = new Goal { A = MainActor.Name, State = "询问信息", ThingB = newGoal.ThingB };
                    MainActor.InMemoryGoal.Insert(0, askActorforInformation);
                    AssertionMechanism(null, effect);
                }
                else
                {
                    var moveToLocation1 = new Goal { A = MainActor.Name, State = "想成为", ThingB = StoryCharactersList[1].Location };
                    var askActorforInformation = new Goal { A = MainActor.Name, State = "询问信息", ThingB = newGoal.ThingB };
                    MainActor.InMemoryGoal.Insert(0, askActorforInformation);
                    MainActor.InMemoryGoal.Insert(0, moveToLocation1);
                    AssertionMechanism(null, effect);
                }
            }
        }

        /// <summary>
        /// 根据特征验证其他参与者是否会与信息合作
        /// </summary>
        public void DeltaAsk()
        {
            if (MainActor.InMemoryGoal.Count == 0) return;

            var newGoal = MainActor.InMemoryGoal[0];

            if (FindGoal(InMemory, newGoal) == null)
            {
                logger.Debug($"Delta Ask");
                logger.Information(newGoal.ToString());

                var effect = DeepCopy(MainActor.InMemoryGoal[0]);
                effect.Negative = true;

                var noMorePersecution = false;
                var a = MainActor.SocialRelation;
                var b = StoryCharactersList[1].SocialRelation;
                var c = MainActor.CurrentFeelingState;
                string[] s1 = { "furious", "physco", "deppresive" };
                string[] s2 = { "depressive", "sick" };
                string[] s3 = { "furious", "physco", "depressive", "sick" };

                if (a == "好" && b == "好" && !s1.Contains(c))
                {
                    logger.Debug("Delta Ask1");

                    var answer = new Goal { A = MainActor.Name, State = "是的朋友", ThingB = StoryCharactersList[1].Name };
                    var tellInformation = new Goal { A = StoryCharactersList[1].Name, State = "告诉", ThingB = newGoal.ThingB };
                    MainActor.InMemoryGoal.Insert(0, tellInformation);
                    var consequence = new List<Goal> {
                            answer
                        };
                    AssertionMechanism(consequence, effect);
                }
                else if (a == "好" && s1.Contains(c))
                {
                    logger.Debug("Delta Ask2");

                    var answer = new Goal { A = StoryCharactersList[1].Name, State = "感到抱歉", ThingB = MainActor.Name };
                    var tellInformation = new Goal { A = StoryCharactersList[1].Name, State = "告诉", ThingB = newGoal.ThingB };
                    MainActor.InMemoryGoal.Insert(0, tellInformation);
                    var consequence = new List<Goal> {
                            answer
                        };
                    AssertionMechanism(consequence, effect);
                    noMorePersecution = true;
                }
                else if (b == "坏" && !noMorePersecution)
                {
                    logger.Debug("Delta Ask3");
                    var scapeIndex = RandomUtil.Next(0, Locations.Count - 1);
                    var scape = Locations[scapeIndex];
                    StoryCharactersList[1].Location = scape;
                    var currentFeelingStateIndex = RandomUtil.Next(0, s3.Length - 1);
                    var currentFeelingState = s3[currentFeelingStateIndex];
                    MainActor.CurrentFeelingState = currentFeelingState;

                    var answer1 = new Goal { A = StoryCharactersList[1].Name, State = "离家出走", ThingB = StoryCharactersList[1].Location };
                    var answer2 = new Goal { A = MainActor.Name, State = "感觉", ThingB = MainActor.CurrentFeelingState };
                    var tellInformation = new Goal { A = MainActor.Name, State = "想成为", ThingB = StoryCharactersList[1].Location };
                    MainActor.InMemoryGoal.Insert(0, tellInformation);
                    var consequence = new List<Goal> {
                        answer1,answer2
                    };
                    AssertionMechanism(consequence, null);
                }
                else
                {
                    MainActor.SocialRelation = "好";
                    var consequence = new List<Goal> {
                        new Goal { A = MainActor.Name, State = "送礼物", ThingB = StoryCharactersList[1].Name },
                        new Goal { A = MainActor.Name, State = "离家出走", ThingB = StoryCharactersList[1].Name },
                    };
                    AssertionMechanism(consequence, null);
                }
            }
        }

        /// <summary>
        /// 告诉对象的位置（如果信息在故事中并且是诚实的）
        /// 如果没有有关该对象的信息，则演员位于并设置一个随机位置
        /// </summary>
        public void DeltaTell()
        {
            if (MainActor.InMemoryGoal.Count == 0) return;

            var newGoal = MainActor.InMemoryGoal[0];

            if (FindGoal(InMemory, newGoal) == null)
            {
                logger.Debug($"DeltaTell");
                logger.Information(newGoal.ToString());

                var effect = DeepCopy(MainActor.InMemoryGoal[0]);
                effect.Negative = true;

                var answer1 = new Goal { A = StoryCharactersList[1].Name, State = "告诉信息", ThingB = MainActor.Name };
                var answer2 = new Goal { A = MainActor.Name, State = "知道", ThingB = newGoal.ThingB };
                var objectLocation = "";

                if (StoryCharactersList[1].Honest)
                {
                    objectLocation = SearchObject(InMemory, newGoal.ThingB);
                    if (!string.IsNullOrEmpty(objectLocation))
                    {
                        var scapeIndex = RandomUtil.Next(0, Locations.Count - 1);
                        objectLocation = Locations[scapeIndex];
                        StoryCharactersList[1].Honest = false;
                        var answer3 = new Goal { A = newGoal.ThingB, State = "在", ThingB = objectLocation };
                        var answer4 = new Goal { A = StoryCharactersList[1].Name, State = "说谎", ThingB = MainActor.Name };
                        var consequence = new List<Goal> {
                            answer1,answer2,answer3,answer4
                        };
                        AssertionMechanism(consequence, effect);
                    }
                    else
                    {
                        var answer3 = new Goal { A = newGoal.ThingB, State = "在", ThingB = objectLocation };
                        var consequence = new List<Goal> {
                            answer1,answer2,answer3
                        };
                        AssertionMechanism(consequence, effect);
                    }
                }
                else if (!StoryCharactersList[1].Honest)
                {
                    var scapeIndex = RandomUtil.Next(0, Locations.Count - 1);
                    objectLocation = Locations[scapeIndex];
                    StoryCharactersList[1].Honest = false;
                    var answer3 = new Goal { A = newGoal.ThingB, State = "在", ThingB = objectLocation };
                    var answer4 = new Goal { A = StoryCharactersList[1].Name, State = "说谎", ThingB = MainActor.Name };
                    var consequence = new List<Goal> {
                            answer1,answer2,answer3,answer4
                        };
                    AssertionMechanism(consequence, effect);
                }
                else
                {
                    var consequence = new List<Goal> {
                        answer1,answer2
                    };
                    AssertionMechanism(consequence, effect);
                }
            }
        }

        /// <summary>
        /// 功能断言机制
        /// 断言控制效果，如果没有市长负责到主体堆栈，则应用于故事状态的内存堆栈，然后应用效果后果确实影响目标堆栈，仅将信息添加到故事堆栈中
        /// </summary>
        /// <param name="consequence"></param>
        /// <param name="effect"></param>
        /// <returns></returns>
        private string AssertionMechanism(List<Goal> consequence, Goal effect)
        {
            var result = "";
            if (consequence == null || consequence.Count == 0)
            {
                if (effect != null)
                {
                    ApplyEffect(effect);
                }
            }
            else
            {
                if (consequence != null)
                {
                    foreach (var goal in consequence)
                    {
                        InMemory.Add(goal);
                    }
                }
                if (effect != null)
                {
                    ApplyEffect(effect);
                }
            }
            return result;
        }

        private string ApplyEffect(Goal goalAchieved)
        {
            var response = "not goal achieved";
            var goalAchieved1 = DeepCopy(goalAchieved);
            goalAchieved1.Negative = false;
            var positivegoalAchieved = goalAchieved1;
            Goal removeItem = FindGoal(MainActor.InMemoryGoal, positivegoalAchieved);
            if (removeItem != null)
            {
                MainActor.InMemoryGoal.Remove(removeItem);
                response = "goal achieved";
            }
            return response;
        }

        private Goal FindGoal(List<Goal> items, Goal item)
        {
            return items.Where(w => w.Negative == item.Negative && w.A == item.A && w.State == item.State && w.ThingB == item.ThingB).FirstOrDefault();
        }

        /// <summary>
        /// 在我们所知道的故事中搜索对象
        /// </summary>
        /// <param name="items"></param>
        /// <param name="objectToSearch"></param>
        /// <returns></returns>
        private string SearchObject(List<Goal> items, string objectToSearch)
        {
            var result = string.Empty;
            var item = items.Where(w => w.State == "在" && w.A == objectToSearch).FirstOrDefault();
            if (item != null)
            {
                result = item.ThingB;
            }
            return result;
        }

        private T DeepCopy<T>(T obj)
        {
            var memberwiseClone = typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);

            var newCopy = memberwiseClone.Invoke(obj, new object[0]);

            foreach (var field in newCopy.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!field.FieldType.IsPrimitive && field.FieldType != typeof(string))
                {
                    var fieldCopy = DeepCopy(field.GetValue(newCopy));
                    field.SetValue(newCopy, fieldCopy);
                }
            }
            return (T)newCopy;
        }
    }
}
