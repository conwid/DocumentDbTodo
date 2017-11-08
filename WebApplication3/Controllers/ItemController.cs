using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication3.Bll;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class ItemController : AsyncController
    {
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Item item)
        {
            await DocumentDbRepository.CreateItemAsync(item);
            return RedirectToAction("Index");
        }

        public ActionResult Index()
        {
            return View(DocumentDbRepository.GetIncompleteItems());
        }

        // Edit hívás a form kéréshez
        // id paramétert az index view-ban az edit linknél így neveztük el
        // típusa string, az adatbázisban is így van
        public ActionResult Edit(string id)
        {
            return View(DocumentDbRepository.GetItem(id));
        }

        // Ide postolódik vissza a form tartalma
        // végén redirect az Index oldalra
        [HttpPost]
        public async Task<ActionResult> Edit(Item item)
        {
            await DocumentDbRepository.UpdateItemAsync(item.Id, item);
            return RedirectToAction("Index");
        }
    }
}