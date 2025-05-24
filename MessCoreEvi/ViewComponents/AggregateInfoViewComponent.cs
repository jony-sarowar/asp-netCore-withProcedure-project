using MessCoreEvi.Models;
using Microsoft.AspNetCore.Mvc;
using MessCoreEvi.ViewModels;

namespace MessCoreEvi.ViewComponents
{
    public class AggregateInfoViewComponent: ViewComponent
    {
        private readonly AppDbContext _db;
        public AggregateInfoViewComponent(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var data = _db.Members
                .Join(_db.Rooms, p => p.RoomId, c => c.RoomId,
                      (p, c) => new { Member = p, Room = c })
                .ToList();

            if (data.Count > 0)
            {
                var min = data.Min(p => p.Member.Amount);
                var max = data.Max(p => p.Member.Amount);
                var sum = data.Sum(p => p.Member.Amount);
                var avg = data.Average(p => p.Member.Amount);

                var groupByResult = data
                    .GroupBy(p => new { p.Member.RoomId, p.Room.RoomName })
                    .Select(c => new GroupByViewModel
                    {
                        RoomId = c.Key.RoomId,
                        RoomName = c.Key.RoomName,
                        MaxValue = c.Max(p => p.Member.Amount),
                        MinValue = c.Min(p => p.Member.Amount),
                        SumValue = c.Sum(p => p.Member.Amount),
                        AvgValue = c.Average(p => p.Member.Amount),
                        Count = c.Count()
                    }).ToList();

                var aggregateViewModel = new AggregateViewModel
                {
                    MinValue = min,
                    MaxValue = max,
                    SumValue = sum,
                    AvgValue = avg,
                    GroupByResult = groupByResult
                };

                return View(aggregateViewModel);
            }


            return View(new AggregateViewModel
            {
                MinValue = 0,
                MaxValue = 0,
                SumValue = 0,
                AvgValue = 0,
                GroupByResult = new List<GroupByViewModel>()
            });
        }
    }
}
