namespace MessCoreEvi.ViewModels
{
    public class AggregateViewModel
    {
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public decimal SumValue { get; set; }
        public decimal AvgValue { get; set; }
        public ICollection<GroupByViewModel> GroupByResult { get; set; }
    }

    public class GroupByViewModel
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public int Count { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public decimal SumValue { get; set; }
        public decimal AvgValue { get; set; }

        public string AvgValueFormatted => AvgValue.ToString("F2");
    }
}
