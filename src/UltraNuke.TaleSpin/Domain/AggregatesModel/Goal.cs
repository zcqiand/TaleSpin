namespace UltraNuke.TaleSpin.Domain.AggregatesModel
{
    public class Goal
    {
        public Thing A { set; get; }
        public string State { set; get; }
        public Thing B { set; get; }
        public bool Negative { set; get; }
    }
}
