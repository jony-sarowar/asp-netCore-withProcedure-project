using MessCoreEvi.Models;
using MessCoreEvi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection;
using System.Diagnostics.Metrics;

namespace MessCoreEvi.Controllers
{
    public class MembersController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _web;
        public MembersController(AppDbContext db, IWebHostEnvironment web)
        {
            _db = db;
            _web = web;
        }
        public IActionResult Index()
        {
            IEnumerable<Member> members = _db.Members.Include(r => r.Room).Include(f => f.Facilities).ToList();
            return View(members);
        }

        public IActionResult AggregatePage()
        {
            return View("AggregatePage");
        }

        [HttpGet]
        public IActionResult CreatePartial()
        {
            MemberViewModel viewModel = new MemberViewModel();
            viewModel.Rooms = _db.Rooms.ToList();
            viewModel.Facilities.Add(new Facility() { FacilityId = 1 });
            return PartialView("_CreateMemberPartial", viewModel);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var member = _db.Members.Find(id);
            if (member != null)
            {
                _db.Members.Remove(member);
                _db.SaveChanges();

            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateMember(MemberViewModel vobj)
        {
            if (!ModelState.IsValid)
            {
                vobj.Rooms = _db.Rooms.ToList();
                return View();
            }
            Member member = new Member
            {
                Name = vobj.Name,
                Mobile = vobj.Mobile,
                Amount = vobj.Amount,
                RoomId = vobj.RoomId,
                IsPermanent = vobj.IsPermanent,
                Facilities = vobj.Facilities,
                Date= vobj.Date
            };

            if (vobj.ProfileFile != null)
            {
                string uniqueFileName = await GetFileName(vobj.ProfileFile);
                member.ImgUrl = uniqueFileName;
            }
            else
            {
                member.ImgUrl = "noimage.png";
            }

            DataTable facilityTabel = new DataTable();
            facilityTabel.Columns.Add("FacilityName", typeof(string));
            if (member.Facilities != null && member.Facilities.Any())
            {
                foreach (var m in member.Facilities)
                {
                    facilityTabel.Rows.Add(m.FacilityName);
                }
            }
            var parameters = new[]
            {
                new SqlParameter("@Name",member.Name),
                new SqlParameter("@Date",member.Date),
                new SqlParameter("@Mobile",member.Mobile),
                new SqlParameter("@RoomId",member.RoomId),
                new SqlParameter("@IsPermanent",member.IsPermanent),
                new SqlParameter("@Amount",member.Amount),
                new SqlParameter("@ImgUrl",member.ImgUrl??(object)DBNull.Value),

                new SqlParameter
                {
                    ParameterName="@Facilities",
                    SqlDbType= SqlDbType.Structured,
                    TypeName="dbo.FParamFacilityType",
                    Value=facilityTabel
                }
            };
            await _db.Database.ExecuteSqlRawAsync("EXEC dbo.spInsertMember @Name,@Date,@Mobile,@RoomId,@IsPermanent,@Amount,@ImgUrl,@Facilities", parameters);
            return RedirectToAction("Index");
        }

        private async Task<string> GetFileName(IFormFile? profileFile)
        {
            string uniqueFileName = null;
            if (profileFile != null)
            {
                uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(profileFile.FileName);
                var uploadFolder = Path.Combine(_web.WebRootPath, "images");
                var filePath = Path.Combine(uploadFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await profileFile.CopyToAsync(fileStream);
                }
            }
            return uniqueFileName;
        }


        public IActionResult EditPartial(int id)
        {
            var member = _db.Members.Include(f => f.Facilities).FirstOrDefault(x => x.MemberId == id);
            var vobj = new MemberViewModel
            {
                MemberId = member.MemberId,
                Name = member.Name,
                Mobile = member.Mobile,
                Amount = member.Amount,
                IsPermanent = member.IsPermanent,
                Date = member.Date,
                RoomId = member.RoomId,
                ImgUrl = member.ImgUrl,
                Facilities = member.Facilities.ToList(),
                Rooms = _db.Rooms.ToList()
            };
            return PartialView("_EditMemberPartial", vobj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMember(MemberViewModel vobj, string OldImageUrl)
        {
            if (!ModelState.IsValid)
            {
                vobj.Rooms = _db.Rooms.ToList();
                return View();
            }
            Member member = _db.Members.FirstOrDefault(x => x.MemberId == vobj.MemberId);
            if (member != null)
            {
                member.Name = vobj.Name;
                member.Mobile = vobj.Mobile;
                member.Amount = vobj.Amount;
                member.RoomId = vobj.RoomId;
                member.IsPermanent = vobj.IsPermanent;
                member.Date = vobj.Date;
                if (vobj.ProfileFile != null)
                {
                    string fileName = await GetFileName(vobj.ProfileFile);
                    member.ImgUrl = fileName;
                }
                else
                {
                    member.ImgUrl = OldImageUrl;
                }
                var facilities = _db.Facilities.Where(x => x.MemberId == vobj.MemberId).ToList();
                DataTable facilityTabel = new DataTable();
                facilityTabel.Columns.Add("FacilityName", typeof(string));
                if (vobj.Facilities != null && vobj.Facilities.Any())
                {
                    foreach (var m in vobj.Facilities)
                    {
                        facilityTabel.Rows.Add(m.FacilityName);
                    }
                }
                var parameters = new[]
                {
                new SqlParameter("@Name",member.Name),
                new SqlParameter("@Date",member.Date),
                new SqlParameter("@Mobile",member.Mobile),
                new SqlParameter("@RoomId",member.RoomId),
                new SqlParameter("@IsPermanent",member.IsPermanent),
                new SqlParameter("@Amount",member.Amount),
                new SqlParameter("@ImgUrl",member.ImgUrl??(object)DBNull.Value),

                new SqlParameter
                {
                    ParameterName="@Facilities",
                    SqlDbType= SqlDbType.Structured,
                    TypeName="dbo.EditParamFacilityType",
                    Value=facilityTabel
                },
                new SqlParameter("@MemberId",vobj.MemberId)
            };
                await _db.Database.ExecuteSqlRawAsync("EXEC dbo.spUpdateMember @Name,@Date,@Mobile,@RoomId,@IsPermanent,@Amount,@ImgUrl,@Facilities,@MemberId", parameters);
                return RedirectToAction("Index");
            }
            vobj.Rooms = _db.Rooms.ToList();
            return View(vobj);
        }
    }
}
