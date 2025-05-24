using MessCoreEvi.Models;
using System.ComponentModel.DataAnnotations;

namespace MessCoreEvi.ViewModels
{
    public class MemberViewModel
    {
        public int MemberId { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public decimal Amount { get; set; }
        public bool IsPermanent { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; }
        public string? ImgUrl { get; set; }
        public int RoomId { get; set; }
        public string? RoomName { get; set; }
        public IFormFile? ProfileFile { get; set; }

        public virtual IList<Member>? Members { get; set; }
        public virtual IList<Room>? Rooms { get; set; }
        public virtual IList<Facility>? Facilities { get; set; } = new List<Facility>();
    }
}
