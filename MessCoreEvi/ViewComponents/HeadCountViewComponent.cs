using MessCoreEvi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MessCoreEvi.ViewComponents
{
    public class HeadCountViewComponent: ViewComponent
    {
        private readonly AppDbContext _db;
        public HeadCountViewComponent(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(int categoryId)
        {
            var RoomHeadCounts = await _db.Members
                .Include(p => p.Room)
                .GroupBy(p => new { p.Room.RoomId, p.Room.RoomName })
                .Select(s => new RoomHeadCount
                {
                    RoomId = s.Key.RoomId,
                    RoomName = s.Key.RoomName,
                    Count = s.Count()
                })
                .ToListAsync();

            return View(RoomHeadCounts ?? new List<RoomHeadCount>());
        }
    }
}
