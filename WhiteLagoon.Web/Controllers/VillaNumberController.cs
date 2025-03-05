using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly ApplicationDbContext _db;
        public VillaNumberController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var villaNumbers = _db.VillaNumbers.Include(u => u.Villa).ToList();
            return View(villaNumbers);
        }

        public IActionResult Create()
        {
            VillaNumberVM villaNumberVM = new()
            {
                VillaList = _db.Villas.ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
            return View(villaNumberVM);
        }

        [HttpPost]
        public IActionResult Create(VillaNumberVM obj)
        {
            if (ModelState.IsValid)
            {
                bool isVillaNumberUnique = _db.VillaNumbers.Any(u => u.Villa_Number == obj.VillaNumber.Villa_Number);
                if (!isVillaNumberUnique)
                {
                    _db.VillaNumbers.Add(obj.VillaNumber);
                    _db.SaveChanges();
                    TempData["success"] = "The Villa Number created successfuly.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = "The Villa Number already exists.";
                    obj.VillaList = _db.Villas.ToList().Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    });
                    return View(obj);
                }
            }
            TempData["error"] = "The Villa Number could not be created.";
            return View(obj);
        }


        public IActionResult Update(int villaNumberId)
        {
            VillaNumberVM villaNumberVM = new()
            {
                VillaList = _db.Villas.ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                VillaNumber = _db.VillaNumbers.FirstOrDefault(x => x.Villa_Number == villaNumberId)
            };

            if (villaNumberVM.VillaNumber is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(villaNumberVM);
        }

        [HttpPost]
        public IActionResult Update(VillaNumberVM villaNumberVM)
        {
            if (ModelState.IsValid)
            {
                _db.VillaNumbers.Update(villaNumberVM.VillaNumber);
                _db.SaveChanges();
                TempData["success"] = "The Villa Number has been updated successfuly.";
                return RedirectToAction(nameof(Index));
            }
            villaNumberVM.VillaList = _db.Villas.ToList().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            TempData["error"] = "The Villa Number could not be updated.";
            return View(villaNumberVM);
        }

        public IActionResult Delete(int villaNumberId)
        {
            VillaNumberVM villaNumberVM = new()
            {
                VillaList = _db.Villas.ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                VillaNumber = _db.VillaNumbers.FirstOrDefault(x => x.Villa_Number == villaNumberId)
            };

            if (villaNumberVM.VillaNumber is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(villaNumberVM);
        }

        [HttpPost]
        public IActionResult Delete(VillaNumberVM villaNumberVM)
        {
            VillaNumber? objFromDb = _db.VillaNumbers.FirstOrDefault(u => u.Villa_Number == villaNumberVM.VillaNumber.Villa_Number);
            if (ModelState.IsValid && objFromDb is not null)
            {
                _db.VillaNumbers.Remove(objFromDb);
                _db.SaveChanges();
                TempData["success"] = "The Villa Number has been deleted successfuly.";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "The Villa Number could not be updated.";
            return RedirectToAction(nameof(Index));
        }
    }
}
