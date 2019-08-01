using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XoaySoTrungThuong.Models;
using System.Data.Entity;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data.SqlClient;
using System.Collections;

namespace XoaySoTrungThuong.Controllers
{
    public class HomeController : Controller
    {
        XoaySoTrungThuongEntities db = new XoaySoTrungThuongEntities();

        public ActionResult Index()
        {
            return RedirectToAction("Roling");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ListPlayerGP()
        {
            ViewBag.ListPlayerGP = db.getPlayerGP().OrderByDescending(x=>x.ID_PlayerGP);
            return View();
        }

        public ActionResult ListPlayerIV()
        {
            ViewBag.ListPlayerIV = db.getPlayerIV().OrderByDescending(x=>x.ID_PlayerIV);
            return View();
        }

        [HttpPost]
        public ActionResult AddNewPrize(FormCollection collection, HttpPostedFileBase file,Prize p)
        {
            if (file != null && file.ContentLength > 0)
            {
                string excelPath = Server.MapPath("~/Content/Files/") + Path.GetFileName(file.FileName);
                file.SaveAs(excelPath);

                string path = "../Content/Files/" + Path.GetFileName(file.FileName);

                string cocaugiai = Convert.ToString(collection["cocaugiai"]);
                string chitietgiai = Convert.ToString(collection["chitietgiai"]);
                int number = Int32.Parse(Convert.ToString(collection["number"]));
                string ghichu = Convert.ToString(collection["ghichu"]);
                int luatchoi = Int32.Parse(collection["luatchoi"]);

                p.CoCauGiai = cocaugiai;
                p.ChiTietGiai = chitietgiai;
                p.Number = number;
                p.GhiChu = ghichu;
                p.HinhAnh = path;
                p.ID_Rule = luatchoi;
                p.Status = false;
                p.SoLanDaQuay = 0;
                db.Prizes.Add(p);
                db.SaveChanges();
            }
            return RedirectToAction("ListPrize");
        }

        public ActionResult DeletePrize(int id)
        {
            db.deletePrize(id);
            return RedirectToAction("ListPrize");
        }

        public ActionResult AddNewPrize()
        {
            ViewBag.Message = "Your contact page.";
            ViewBag.ListRule = db.GetAllRule().ToList();
            return View();
        }

        public ActionResult RoalingManager()
        {
            ViewBag.Message = "Your contact page.";
            ViewBag.ListRule = db.GetAllRule();
            return View();
        }

        public ActionResult ListPrize()
        {
            ViewBag.Message = "Your contact page.";
            ViewBag.ListPrize = db.getAllPrize();
            return View();
        }

        public ActionResult ListRuleDetail()
        {
            ViewBag.ListRule = db.getRuleToPlay();
            return View();
        }

        public ActionResult ViewAllPlayer()
        {
            ViewBag.Message = "Xem danh sách nhân viên";
            ViewBag.ListPlayer = db.getAllPlayer();

            return View();
        }

        public ActionResult ViewAllGuess()
        {
            ViewBag.Message = "Xem danh sách khách mời";
            ViewBag.ListPlayer = db.getAllPlayer();

            return View();
        }

        public ActionResult DeleteAllPlayer()
        {
            ViewBag.ListPlayer = db.deleteAllPlayer();

            return RedirectToAction("ViewAllPlayer");
        }

        public ActionResult DeleteAllGuess()
        {
            ViewBag.ListPlayer = db.deleteAllGuess();

            return RedirectToAction("ViewAllGuess");
        }

        [HttpPost]
        public ActionResult RegisterPlayerUpload(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                if (file.FileName.EndsWith("xls") || file.FileName.EndsWith("xlsx"))
                {
                    string path = Server.MapPath("~/Content/Files/") + Path.GetFileName(file.FileName);
                    file.SaveAs(path);
                    Excel.Application app = new Excel.Application();
                    Excel.Workbook workbook = app.Workbooks.Open(path);
                    Excel.Worksheet worksheet = workbook.ActiveSheet;
                    Excel.Range range = worksheet.UsedRange;
                    List<Player> listPlayer = new List<Player>();
                    for (int row = 2; row <= range.Rows.Count; row++)
                    {
                        Player player = new Player();
                        player.Code = (((Excel.Range)range.Cells[row, 1]).Text);
                        player.Name = ((Excel.Range)range.Cells[row, 2]).Text;
                        player.Room = ((Excel.Range)range.Cells[row, 3]).Text;
                        player.BelongType = "NV";
                        player.Flag = false;
                        db.Players.Add(player);
                        db.SaveChanges();
                    }
                    ViewBag.ListPlayer = listPlayer;
                    return RedirectToAction("ViewAllPlayer");
                }
                else
                {
                    ViewBag.Error = "File Excel không đúng";
                    return RedirectToAction("RegisterPlayer");
                }
            }
            else
            {
                ViewBag.Error = "Chưa Chọn File";
                return RedirectToAction("RegisterPlayer");
            }
        }

        [HttpPost]
        public ActionResult RegisterGuess(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                if (file.FileName.EndsWith("xls") || file.FileName.EndsWith("xlsx"))
                {
                    string path = Server.MapPath("~/Content/Files/") + Path.GetFileName(file.FileName);
                    file.SaveAs(path);
                    Excel.Application app = new Excel.Application();
                    Excel.Workbook workbook = app.Workbooks.Open(path);
                    Excel.Worksheet worksheet = workbook.Worksheets[5];
                    Excel.Range range = worksheet.UsedRange;
                    List<Player> listPlayer = new List<Player>();
                    //for (int row = 2; row <= range.Rows.Count; row++)
                    //{
                    //    Player player = new Player();
                    //    player.Code = (((Excel.Range)range.Cells[row, 2]).Text);
                    //    player.Name = ((Excel.Range)range.Cells[row, 3]).Text;
                    //    player.Room = ((Excel.Range)range.Cells[row, 4]).Text;
                    //    player.BelongType = "KM";
                    //    player.Flag = false;
                    //    db.Players.Add(player);
                    //    db.SaveChanges();
                    //}
                    int temp = 0;
                    for (int row = 3; row <= range.Rows.Count; row++)
                    {
                        Player player = new Player();
                        int row2 = 2;
                        //player.Code = (((Excel.Range)range.Cells[row,2]).Text);
                        for (int col = 2; col <= 6; col++)
                        {
                            if (row == 3)
                            {
                                player.Code = (((Excel.Range)range.Cells[row, 1]).Text);
                                player.Name = ((Excel.Range)range.Cells[row2, col]).Text;
                                int t1 = row2 + 1;
                                player.Room = ((Excel.Range)range.Cells[t1, col]).Text;
                                player.BelongType = "KM";
                                player.Flag = false;
                                db.Players.Add(player);
                                db.SaveChanges();
                            }
                            if (row > 3)
                            {
                                player.Code = (((Excel.Range)range.Cells[row, 1]).Text);
                                player.Name = ((Excel.Range)range.Cells[row2, col]).Text;
                                int t2 = temp + 3;
                                player.Room = ((Excel.Range)range.Cells[t2, col]).Text;
                                player.BelongType = "KM";
                                player.Flag = false;
                                db.Players.Add(player);
                                db.SaveChanges();
                            }
                        }
                        temp += 1;
                    }
                    ViewBag.ListPlayer = listPlayer;
                    return RedirectToAction("ViewAllGuess");
                }
                else
                {
                    ViewBag.Error = "File Excel không đúng";
                    return RedirectToAction("RegisterGuess");
                }
            }
            else
            {
                ViewBag.Error = "Chưa Chọn File";
                return RedirectToAction("RegisterGuess");
            }
        }

        public ActionResult RegisterPlayer()
        {
            return View();
        }

        public ActionResult RegisterGuess()
        {
            return View();
        }


        [HttpPost]
        public ActionResult RoalingManager(RuleToPlayThis model)
        {
            RuleToPlay rule = new RuleToPlay();
            rule.Status = false;
            rule.Stage = Int32.Parse(model.Stage);
            rule.RoundNumber = Int32.Parse(model.RoundNumber);
            rule.PrizeNumber = Int32.Parse(model.PrizeNumber);
            rule.GuessNumber = Int32.Parse(model.GuessNumber);
            rule.SoKMDaTrungGiai = 0;

            db.RuleToPlays.Add(rule);
            db.SaveChanges();

            return View();
        }


        public ActionResult EditRoalingManager(int id)
        {
            var rule = db.RuleToPlays.Where(i => i.ID_Rule == id).FirstOrDefault();
            ViewBag.Rule = rule;
            return View();
        }

        [HttpPost]
        public ActionResult EditRoalingManager(FormCollection collection)
        {
            int hdID = Convert.ToInt32(collection["hdID"]);
            int Stage = Int32.Parse(Convert.ToString(collection["Stage"]));
            int RoundNumber = Int32.Parse(Convert.ToString(collection["RoundNumber"]));
            int PrizeNumber = Int32.Parse(Convert.ToString(collection["PrizeNumber"]));
            int GuessNumber = Int32.Parse(Convert.ToString(collection["GuessNumber"]));

            //db.updateRuleToPlay(hdID, RoundNumber, PrizeNumber, GuessNumber);
            var rule = db.RuleToPlays.Where(i => i.ID_Rule == hdID).FirstOrDefault();
            rule.Stage = Stage;
            rule.RoundNumber = RoundNumber;
            rule.PrizeNumber = PrizeNumber;
            rule.GuessNumber = GuessNumber;
            db.SaveChanges();

            return RedirectToAction("ListRuleDetail");
        }

        public ActionResult EditNewPrize(int id)
        {
            var prize = db.Prizes.Where(i => i.ID_Prize == id).FirstOrDefault();
            ViewBag.Prize = prize;
            ViewBag.ListRule = db.GetAllRule().ToList();
            return View();
        }

        [HttpPost]
        public ActionResult EditNewPrize(Prize p, HttpPostedFileBase file)
        {
            string excelPath = Server.MapPath("~/Content/Files/") + Path.GetFileName(file.FileName);
            file.SaveAs(excelPath);

            string path = "../Content/Files/" + Path.GetFileName(file.FileName);

            var prize = db.Prizes.Find(p.ID_Prize);
            prize.CoCauGiai = p.CoCauGiai;
            prize.ChiTietGiai = p.ChiTietGiai;
            prize.Number = p.Number;
            prize.HinhAnh = path;
            prize.ID_Rule = p.ID_Rule;
            db.SaveChanges();
            return RedirectToAction("ListPrize");
        }

        public ActionResult DeleteRoalingManager(int id)
        {
            db.deleteRuleToPlay(id);
            return RedirectToAction("ListRuleDetail");
        }

        public ActionResult Roling()
        {   
            ViewBag.ListPlayerGP = getPlayerGP();
            ViewBag.ListPlayer = getAllPlayer();
            return View();
        }

        public ActionResult deletePlayerIV()
        {
            db.deletePlayerIV();
            return RedirectToAction("ListPlayerIV");
        }

        public ActionResult deletePlayerGP()
        {
            db.deletePlayerGP();
            return RedirectToAction("ListPlayerGP");
        }

        [HttpGet]
        public JsonResult getAllPlayerGP()
        {
            var data = db.getPlayerGP().OrderByDescending(x => x.ID_PlayerGP).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult getCount(int id=1)
        {
            var data = db.getSoLanDaQuay().Where(x=>x.ID_Prize==id).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public void UpdateSoLanDaQuay(int id=1)
        {
            var data = db.Prizes.Where(x => x.ID_Prize == id).Single();
            data.SoLanDaQuay += 1;
            db.SaveChanges();
        }

        [HttpGet]
        public JsonResult getSoKMDaTrungGiai(int id)
        {
            var data = db.getRule().Where(x => x.ID_Rule == id).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public void UpdateSoKMDaTrungGiai(int id)
        {
            var data = db.RuleToPlays.Where(x=>x.ID_Rule == id).Single();
            data.SoKMDaTrungGiai += 1;
            db.SaveChanges();
        }

        [HttpGet]
        public JsonResult UpdateRule(int id)
        {
            var data = db.RuleToPlays.Where(x => x.ID_Rule == id).Single();
            if (data.Status == true)
            {
                data.Status = false;
            }
            else
            {
                data.Status = true;
            }
            db.SaveChanges();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult getListRule()
        {
            var data = db.getRuleToPlay().ToList();
            return Json(data,JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult KiemTraKM(string code)
        {
            var data = db.Players.Where(x => x.Code == code ).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult getPlayerNV()
        {
            var data = db.Players.Where(x => x.BelongType == "NV" && x.Flag==false).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult getGuessNumber(int id)
        {
            var data = db.getRule().Where(x=>x.ID_Rule==id).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult getFlag()
        {
            var data = db.Players.Where(x => x.Flag == false).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //[HttpGet] Đây là để test
        //public JsonResult getFlag()
        //{
        //    var data = db.Players.Where(x => x.Flag == true).ToList();
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        [HttpGet]
        public JsonResult UpdatePrize(int id)
        {
            var data = db.Prizes.Where(x => x.ID_Prize == id).Single();
            data.Status = true;
            db.SaveChanges();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult getSoLanQuay(int id)
        {
            var data = db.getPrize().Where(x => x.ID_Prize == id).ToList();
            return Json(data,JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult getGiaiThuong(int id_rule)
        {
            var data = db.getGiaiThuong().Where(x => x.ID_Rule == id_rule).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult getLanQuay(int stage)
        {
            var data = db.getLanQuay().Where(x => x.Stage == stage).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getDotQuay()
        {
            var data = db.getDotQuay().ToList();
            return Json(data,JsonRequestBehavior.AllowGet);
        }

        public List<Player> getAllPlayer()
        {
            return db.Players.ToList();
        }

        public List<PlayerGP> getPlayerGP()
        {
            return db.PlayerGPs.OrderByDescending(x=>x.ID_PlayerGP).ToList();
        }

        public void UpdatePlayerGetPrize(string code, PlayerGP p,int dotquay,int lanquay,string giaiquay)
        {
            var player = db.Players.Where(i => i.Code == code).FirstOrDefault();
            player.Flag = true;
            p.Name = player.Name;
            p.Room = player.Room;
            p.BelongType = player.BelongType;
            p.Flag = Convert.ToString(player.Flag);
            p.Dot = dotquay;
            p.Lan = lanquay;
            p.Giai = giaiquay;
            db.PlayerGPs.Add(p);
            db.SaveChanges();
        }

        public void UpdatePlayerInvalid(string code, PlayerIV p,int dotquay, int lanquay, string giaiquay)
        {
            var player = db.Players.Where(i => i.Code == code).FirstOrDefault();
            player.Flag = true;
            p.Name = player.Name;
            p.Room = player.Room;
            p.BelongType = player.BelongType;
            p.Flag = Convert.ToString(player.Flag);
            p.Dot = dotquay;
            p.Lan = lanquay;
            p.Giai = giaiquay;
            db.PlayerIVs.Add(p);
            db.SaveChanges();
        }

        [HttpGet]
        public JsonResult HinhNen()
        {
            var temp = db.HinhNens.Where(x => x.ID_HinhNen == 1).Single();
            var data = temp.HinhNen1;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangeBackground()
        {
            return View();
        }


        [HttpPost]
        public ActionResult ThayDoiHinhNen(HttpPostedFileBase filehinh)
        {
            string excelPath = Server.MapPath("~/Content/Files/") + Path.GetFileName(filehinh.FileName);
            filehinh.SaveAs(excelPath);
            string path = "../Content/Files/" + Path.GetFileName(filehinh.FileName);
            var data = db.HinhNens.Where(x => x.ID_HinhNen == 1).Single();
            data.HinhNen1 = path;
            db.SaveChanges();
            return RedirectToAction("ChangeBackground");
        }

        [HttpGet]
        public JsonResult SortTen()
        {
            var kq = db.PlayerGPs.OrderByDescending(x => x.Name).ToList();
            ViewBag.SorTen = kq;
            return Json(kq, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SortGiai()
        {
            var kq = db.PlayerGPs.OrderByDescending(x => x.Giai).ToList();
            ViewBag.SorGiai = kq;
            return Json(kq, JsonRequestBehavior.AllowGet);
        }
    }
}