namespace MessCoreEvi.Models
{
    public class RoomHeadCount
    {
        public int RoomId { get; set; }
        public int Count { get; set; }
        public virtual Room Room { get; set; }
        public string RoomName { get; set; }
    }
}
