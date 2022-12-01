namespace UltraNuke.TaleSpin.Domain;

/// <summary>
/// 随机数
/// </summary>
public class RandomUtil
{
    /// <summary>
    /// 指定范围内的随机数
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int Next(int min, int max)
    {
        var buffer = Guid.NewGuid().ToByteArray();
        var iSeed = BitConverter.ToInt32(buffer, 0);
        var r = new Random(iSeed);
        var rtn = r.Next(min, max + 1);
        return rtn;
    }

    /// <summary>
    /// 按权重从列表中随机选择靠前几项数据项
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    public static IList<T> Choice<T>(IList<(T, int)> list, int n)
    {
        var totalWeights = list.Sum(s => s.Item2);
        var choiceList = list.Select(s => (s.Item1, s.Item2 + Next(0, totalWeights)))
                            .OrderByDescending(o => o.Item2)
                            .Take(n)
                            .Select(s => s.Item1)
                            .ToList();
        return choiceList;
    }

    /// <summary>
    /// 按权重从列表中随机选择数据项
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T Choice<T>(IList<(T, int)> list)
    {
        var choiceList = Choice(list, 1);
        var item = choiceList[0];
        return item;
    }
}
