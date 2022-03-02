using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectCoffee.Dal;
using ProjectCoffee.Models;
using ProjectCoffee.ViewModel;

namespace ProjectCoffee.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(Client Me)
        {
            UserDal dal = new UserDal();
            if (ModelState.IsValid)
            {
                var temp = dal.Clients.Where(x => x.Name.Equals(Me.Name) && x.PhoneNum.Equals(Me.PhoneNum)).SingleOrDefault();
                if (temp == null)
                {
                    dal.Clients.Add(Me);
                    dal.SaveChanges();
                }
                else
                    return View("userExists");
                
            }
            else
                return View("submit", Me);
            var check = dal.Clients.Where(x => x.Name.Equals(Me.Name) && x.PhoneNum.Equals(Me.PhoneNum)).SingleOrDefault();
            if (check != null)
                Session["Client"] = check;
            return View("ClientHomePage");
        }

        public ActionResult Login()
        {
            if (Session["Temp"] != null)
                return View();
            return View("HomePage");
        }

        public ActionResult ClientHomePage()
        {
            if (Session["Client"] != null)
                return View();
            return View("HomePage");
        }

        [HttpPost]
        public ActionResult Validate()
        {
            string tempName = Request.Form["name"];
            string tempPhone = Request.Form["phoneNum"];
            UserDal dal = new UserDal();
            var check_s = dal.Staffs.Where(x => x.Name.Equals(tempName) && x.PhoneNum.Equals(tempPhone)).SingleOrDefault();
            if (check_s != null)
            {
                Session["Temp"] = check_s;
                return View("login");
            }
            else
            {
                var check_c = dal.Clients.Where(x => x.Name.Equals(tempName) && x.PhoneNum.Equals(tempPhone)).SingleOrDefault();
                if (check_c != null)
                {
                    Session["Client"] = check_c;
                    return View("ClientHomePage");
                }
            }
            return View("noUser");
        }

        public ActionResult Submit()
        {
            if (Session.Count == 0)
                return View();
            return View("HomePage");
        }

        [HttpPost]
        public ActionResult ValStaff()
        {
            string tempPass = Request.Form["Password"];
            string tempName = ((Staff)Session["Temp"]).Name;
            string tempPhone = ((Staff)Session["Temp"]).PhoneNum;
            UserDal dal = new UserDal();
            var check_s = dal.Staffs.Where(x => x.Name.Equals(tempName) 
                                            && x.PhoneNum.Equals(tempPhone) 
                                            && x.Password.Equals(tempPass)).SingleOrDefault();
            if (check_s != null)
            {
                Session["Temp"] = null;
                Session["Staff"] = check_s;
                return View("StaffHomePage");
            }
            Session["Temp"] = null;
            return View("noUser");
        }

        public ActionResult SubmitStaff()
        {
            if (Session["Staff"] != null && ((Staff)Session["Staff"]).Permission)
                return View();
            return View("HomePage");
        }

        [HttpPost]
        public ActionResult AddStaff(Staff Other)
        {
            UserDal dal = new UserDal();
            if (ModelState.IsValid)
            {
                var temp = dal.Staffs.Where(x => x.Name.Equals(Other.Name) && x.PhoneNum.Equals(Other.PhoneNum)).SingleOrDefault();
                if (temp == null)
                {
                    dal.Staffs.Add(Other);
                    dal.SaveChanges();
                }
                else
                    return View("userExists");

            }
            else
                return View("SubmitStaff", Other);
            return View("StaffHomePage");
        }

        public ActionResult ShowMenu()
        {
            UserViewModel ivm = new UserViewModel();
            UserDal dal = new UserDal();
            ivm.Items = dal.Items.ToList<Item>();
            return View("ShowMenu",ivm);
        }
        public ActionResult AddItem()
        {
            if (Session["Staff"] != null && ((Staff)Session["Staff"]).Permission)
                return View();
            return View("HomePage");
        }
        [HttpPost]
        public ActionResult SubmitItem(HttpPostedFileBase postedItem)
        {
            if (postedItem == null || Request.Form["Info"] == "" || Request.Form["Price"] == "")
                ModelState.AddModelError("oneOf", "Info/Price/Picture are not set properly");
            if (ModelState.IsValid)
            {
                UserViewModel ivm = new UserViewModel();
                Item newItem = new Item();
                newItem.Info = Request.Form["Info"];
                newItem.Price = Int32.Parse(Request.Form["Price"]);
                newItem.Availability = true;
                if (postedItem != null)
                {
                    newItem.file = new Byte[postedItem.ContentLength];
                    postedItem.InputStream.Read(newItem.file, 0, postedItem.ContentLength);
                    UserDal dal = new UserDal();
                    dal.Items.Add(newItem);
                    dal.SaveChanges();
                    ivm.Items = dal.Items.ToList<Item>();
                    return View("EditMenu", ivm);
                }
                return View("AddItem", newItem);
            }
            else {
                return View("AddItem");
            }
        }

        public ActionResult StaffHomePage()
        {
            if (Session["Staff"] != null)
                return View();
            return View("HomePage");
        }
        public ActionResult EditMenu()
        {
            if (Session["Staff"] != null && ((Staff)Session["Staff"]).Permission)
            {
                UserViewModel ivm = new UserViewModel();
                UserDal dal = new UserDal();
                ivm.Items = dal.Items.ToList<Item>();
                return View("EditMenu", ivm);
            }
            return View("HomePage");
        }
        public ActionResult DelFromMenu(string idItem)
        {
            int temp1 = Int32.Parse(idItem);
            UserDal dal = new UserDal();
            var temp = dal.Items.Find(temp1);
            if (temp != null)
            {
                dal.Items.Remove(temp);
                dal.SaveChanges();
            }
            UserViewModel ivm = new UserViewModel();
            ivm.Items = dal.Items.ToList<Item>();
            return View("editMenu", ivm);
        }
        public ActionResult changeAvail(string idItem)
        {
            int temp1 = Int32.Parse(idItem);
            UserDal dal = new UserDal();
            var temp = dal.Items.Find(temp1);
            if (temp != null)
            {
                dal.Items.Find(temp1).Availability = !dal.Items.Find(temp1).Availability;
                dal.SaveChanges();
            }
            UserViewModel ivm = new UserViewModel();
            ivm.Items = dal.Items.ToList<Item>();
            return View("editMenu", ivm);
        }
        public ActionResult changePrice(string idItem)
        {
            if (Session["Staff"] != null && ((Staff)Session["Staff"]).Permission)
            {
                int temp1 = Int32.Parse(idItem);
                UserDal dal = new UserDal();
                var temp = dal.Items.Find(temp1);
                if (temp != null)
                {
                    return View("ChangePrice", temp);
                }
                return View();
            }
            return View("HomePage");
        }
        [HttpPost]
        public ActionResult submitChanges(Item item)
        {
            UserDal dal = new UserDal();
            dal.Items.Find(item.idItem).Info = item.Info;
            dal.Items.Find(item.idItem).Price = item.Price;
            dal.SaveChanges();

            UserViewModel ivm = new UserViewModel();
            ivm.Items = dal.Items.ToList<Item>();
            return View("editmenu",ivm);
        }

        public ActionResult CartMenu()
        {
            if (Session["Client"] != null)
            {
                UserViewModel ivm = new UserViewModel();
                UserDal dal = new UserDal();
                ivm.Items = dal.Items.ToList<Item>();
                return View("CartMenu", ivm);
            }
            return View("HomePage");
        }

        public ActionResult AddToCart(string idItem)
        {
            UserDal dal = new UserDal();
            string temp = "";
            if (Session["Client"] != null && Session["orderingClient"] == null)
            {
                temp = ((Client)Session["Client"]).PhoneNum;
            }
            else
            {
                if (Session["Client"] == null && Session["orderingClient"] != null)
                {
                    temp = ((Client)Session["orderingClient"]).PhoneNum;
                }
            }
            var check = dal.Orders.Where(x => x.Client.Equals(temp) && x.Status == false).SingleOrDefault();
            if (check != null)
            {
                Order_items newItemToOrder = new Order_items();
                newItemToOrder.OrderId = dal.Orders.Find(check.OrderId).OrderId;
                newItemToOrder.ProductId = Int32.Parse(idItem);
                dal.orderedItems.Add(newItemToOrder);
                dal.SaveChanges();
            }
            else
            {
                Order newOrder = new Order();
                newOrder.Client = temp;
                newOrder.Status = false;
                dal.Orders.Add(newOrder);
                dal.SaveChanges();
                check = dal.Orders.Where(x => x.Client.Equals(temp) && x.Status == false).SingleOrDefault();
                Order_items newItemToOrder = new Order_items();
                newItemToOrder.OrderId = dal.Orders.Find(check.OrderId).OrderId;
                newItemToOrder.ProductId = Int32.Parse(idItem);
                dal.orderedItems.Add(newItemToOrder);
                dal.SaveChanges();
            }
            dal.Clients.Where(x => x.PhoneNum.Equals(temp)).SingleOrDefault().CCounter++;
            dal.SaveChanges();
            if (Session["Client"] == null && Session["orderingClient"] != null)
            {

                Session["orderingClient"] = dal.Clients.Where(x => x.PhoneNum.Equals(temp)).SingleOrDefault();
                UserViewModel ivm2 = new UserViewModel();
                ivm2.Items = dal.Items.ToList<Item>();
                return View("PickItems", ivm2);
            }
            Session["Client"] = dal.Clients.Where(x => x.PhoneNum.Equals(temp)).SingleOrDefault();
            UserViewModel ivm = new UserViewModel();
            ivm.Items = dal.Items.ToList<Item>();
            return View("CartMenu", ivm);
        }

        public ActionResult Cart()
        {
            string temp = "";
            if (Session["Client"] != null && Session["orderingClient"] == null)
            {
                temp = ((Client)Session["Client"]).PhoneNum;
            }
            else
            {
                if (Session["Client"] == null && Session["orderingClient"] != null)
                {
                    temp = ((Client)Session["orderingClient"]).PhoneNum;
                }
                else
                {
                    return View("HomePage");
                }
            }
            UserDal dal = new UserDal();
            UserViewModel ivm = new UserViewModel();
            ivm.Items = new List<Item>();
            var order = dal.Orders.Where(x => x.Client.Equals(temp) && x.Status == false).SingleOrDefault();
            if (order != null)
            {
                var orderedItems = dal.orderedItems.Where(x => x.OrderId == order.OrderId).ToList();
                foreach (Order_items i in orderedItems)
                    ivm.Items.Add(dal.Items.Find(i.ProductId));
            }
            if (Session["Client"] == null && Session["orderingClient"] != null)
            {
                return View("ClientsCart", ivm);
            }
            return View("cart",ivm);
            
        }

        public ActionResult RemoveFromCart(string idItem)
        {
            int tempItemId = Int32.Parse(idItem);
            UserDal dal = new UserDal();
            string client = "";
            if (Session["Client"] != null && Session["orderingClient"] == null)
            {
                client = ((Client)Session["Client"]).PhoneNum;
            }
            else
            {
                if (Session["Client"] == null && Session["orderingClient"] != null)
                {
                    client = ((Client)Session["orderingClient"]).PhoneNum;
                }
            }
            int orderNum = dal.Orders.Where(x => x.Client.Equals(client) && x.Status == false).SingleOrDefault().OrderId;
            var item = dal.orderedItems.Where(x => x.ProductId == tempItemId && x.OrderId == orderNum).FirstOrDefault();
            dal.orderedItems.Remove(item);
            dal.SaveChanges();
            UserViewModel ivm = new UserViewModel();
            ivm.Items = new List<Item>();
            var order = dal.Orders.Where(x => x.Client.Equals(client) && x.Status == false).SingleOrDefault();
            if (order != null)
            {
                var orderedItems = dal.orderedItems.Where(x => x.OrderId == order.OrderId).ToList();
                foreach (Order_items i in orderedItems)
                    ivm.Items.Add(dal.Items.Find(i.ProductId));
            }
            if (dal.Clients.Where(x => x.PhoneNum.Equals(client)).SingleOrDefault().CCounter != 0)
                dal.Clients.Where(x => x.PhoneNum.Equals(client)).SingleOrDefault().CCounter--;
            else
                dal.Clients.Where(x => x.PhoneNum.Equals(client)).SingleOrDefault().CCounter = 9;
            dal.SaveChanges();
            if (Session["Client"] == null && Session["orderingClient"] != null)
            {

                Session["orderingClient"] = dal.Clients.Where(x => x.PhoneNum.Equals(client)).SingleOrDefault();
                return View("ClientsCart", ivm);
            }
            Session["Client"] = dal.Clients.Where(x => x.PhoneNum.Equals(client)).SingleOrDefault();
            return View("cart",ivm);
        }

        public ActionResult PaymentPage(string sum)
        {
            UserDal dal = new UserDal();
            string client = "";
            if (Session["Client"] != null && Session["orderingClient"] == null)
            {
                client = ((Client)Session["Client"]).PhoneNum;
            }
            else
            {
                if (Session["Client"] == null && Session["orderingClient"] != null)
                {
                    client = ((Client)Session["orderingClient"]).PhoneNum;
                }
                else
                {
                    return View("HomePage");
                }
            }
            Client tempClient = dal.Clients.Where(x => x.PhoneNum.Equals(client)).SingleOrDefault();
            int temp = Int32.Parse(sum);
            TempData["sum"] = temp;
            if (tempClient.VIP && tempClient.CCounter >= 10)   
            {   
                int orderNum = dal.Orders.Where(x => x.Client.Equals(client) && x.Status == false).SingleOrDefault().OrderId;
                int itemId = dal.orderedItems.Where(x => x.OrderId == orderNum).FirstOrDefault().ProductId;
                temp -= dal.Items.Find(itemId).Price;
                TempData["sum"] = temp;
            }
            if (tempClient.CCounter >= 10)
            {
                dal.Clients.Where(x => x.PhoneNum.Equals(client)).SingleOrDefault().CCounter = dal.Clients.Where(x => x.PhoneNum.Equals(client)).SingleOrDefault().CCounter % 10;
                dal.SaveChanges();
            }
            if (Session["Client"] == null && Session["orderingClient"] != null)
            {

                Session["orderingClient"] = dal.Clients.Where(x => x.PhoneNum.Equals(client)).SingleOrDefault();
                return View();
            }
            Session["Client"] = dal.Clients.Where(x => x.PhoneNum.Equals(client)).SingleOrDefault();
            return View();
        }

        public ActionResult PayByClient()
        {
            UserDal dal = new UserDal();
            string client = "";
            if (Session["Client"] != null && Session["orderingClient"] == null)
            {
                client = ((Client)Session["Client"]).PhoneNum;
            }
            else
            {
                if (Session["Client"] == null && Session["orderingClient"] != null)
                {
                    client = ((Client)Session["orderingClient"]).PhoneNum;
                }
            }
            Order tempOrder = dal.Orders.Where(x => x.Client.Equals(client) && x.Status == false).SingleOrDefault();
            int tempNum = tempOrder.OrderId;
            dal.Orders.Find(tempNum).Status = true;
            dal.SaveChanges();
            Client tempClient = dal.Clients.Where(x => x.PhoneNum.Equals(client)).SingleOrDefault();
            if (tempClient.CCounter >= 10)
            {
                dal.Clients.Where(x => x.PhoneNum.Equals(client)).SingleOrDefault().CCounter = dal.Clients.Where(x => x.PhoneNum.Equals(client)).SingleOrDefault().CCounter % 10;
                dal.SaveChanges();
            }
            Table tempTable = dal.Tables.Where(x => x.Client.Equals(client) && x.Occupied == true).SingleOrDefault();
            dal.Tables.Find(tempTable.TableId).Client = null;
            dal.Tables.Find(tempTable.TableId).Occupied = false;
            dal.SaveChanges();
            if (Session["Client"] == null && Session["orderingClient"] != null)
            {
                Session["orderingClient"] = null;
                return View("StaffHomePage");
            }
            return View("ClientHomePage");
        }

        public ActionResult ChangeVIP(Client lookUp)
        {
            if (Session["Staff"] != null)
            {
                if (lookUp.PhoneNum != null)
                {
                    UserDal dal = new UserDal();
                    Client temp = dal.Clients.Where(x => x.PhoneNum.Equals(lookUp.PhoneNum)).SingleOrDefault();
                    if (temp == null)
                    {
                        return View("noUser");
                    }
                    TempData["tempName"] = temp.Name;
                    TempData["tempStatus"] = temp.VIP;
                    Session["tempClient"] = temp;
                }
                return View();
            }
            return View("HomePage");
        }

        public ActionResult submitVIPChange()
        {
            string tempClient = ((Client)Session["tempClient"]).PhoneNum;
            Session["tempClient"] = null;
            UserDal dal = new UserDal();
            dal.Clients.Where(x => x.PhoneNum.Equals(tempClient)).SingleOrDefault().VIP = !dal.Clients.Where(x => x.PhoneNum.Equals(tempClient)).SingleOrDefault().VIP;
            dal.SaveChanges();
            return View("ChangeVIP");
        }

        public ActionResult TakeOrder(Client lookUp)
        {
            if (Session["Staff"] != null && !((Staff)Session["Staff"]).Permission)
            {
                if (lookUp.PhoneNum != null)
                {
                    UserDal dal = new UserDal();
                    Client temp = dal.Clients.Where(x => x.PhoneNum.Equals(lookUp.PhoneNum)).SingleOrDefault();
                    if (temp == null)
                    {
                        return View("noUser");
                    }
                    Session["orderingClient"] = temp;
                    return RedirectToAction("PickItems");
                }
                return View();
            }
            return View("HomePage");
        }

        public ActionResult PickItems()
        {
            if (Session["Staff"] != null && !((Staff)Session["Staff"]).Permission)
            {
                UserViewModel ivm = new UserViewModel();
                UserDal dal = new UserDal();
                ivm.Items = dal.Items.ToList<Item>();
                return View("PickItems", ivm);
            }
            return View("HomePage");
        }

        public ActionResult setTable()
        {
            if (Session["Staff"] != null && ((Staff)Session["Staff"]).Permission)
                return View();
            return View("HomePage");
        }

        public ActionResult addTable()
        {
            UserDal dal = new UserDal();
            Table newTable = new Table();
            newTable.Chairs = Int32.Parse(Request.Form["Chairs"]);
            newTable.Occupied = false;
            dal.Tables.Add(newTable);
            dal.SaveChanges();
            ViewBag.msg = "Table Add successfully";
            return View("setTable");
        }

        public ActionResult changeTable()
        {
            if(Session["Staff"] != null && ((Staff)Session["Staff"]).Permission){
                UserDal dal = new UserDal();
                UserViewModel tvm = new UserViewModel();
                tvm.Tables = dal.Tables.ToList<Table>();
                return View("changeTable", tvm);
            }
            return View("HomePage");
        }

        public ActionResult DelTable(string TableId)
        {
            int temp1 = Int32.Parse(TableId);
            UserDal dal = new UserDal();
            var temp = dal.Tables.Find(temp1);
            if (temp != null)
            {
                dal.Tables.Remove(temp);
                dal.SaveChanges();
            }
            UserViewModel ivm = new UserViewModel();
            ivm.Tables = dal.Tables.ToList<Table>();
            return View("changeTable", ivm);
        }

        public ActionResult changeSits(string TableId)
        {
            if (Session["Staff"] != null && ((Staff)Session["Staff"]).Permission)
            {
                int temp1 = Int32.Parse(TableId);
                UserDal dal = new UserDal();
                var temp = dal.Tables.Find(temp1);
                if (temp != null)
                {
                    return View("changeSits", temp);
                }
                return View();
            }
            return View("HomePage");
        }

        [HttpPost]
        public ActionResult SubmitSitChanges()
        {
            int tempTable = Int32.Parse(Request.Form["TableId"]);
            UserDal dal = new UserDal();
            dal.Tables.Find(tempTable).Chairs = Int32.Parse(Request.Form["Chairs"]);
            dal.SaveChanges();
            UserViewModel tvm = new UserViewModel();
            tvm.Tables = dal.Tables.ToList<Table>();
            return View("changeTable", tvm);
        }

        public ActionResult pickTable()
        {
            if ((Session["Staff"] != null && !((Staff)Session["Staff"]).Permission) || Session["Client"] != null) 
            {
                UserDal dal = new UserDal();
                UserViewModel tvm = new UserViewModel();
                var temp = dal.Tables.Where(x => x.Occupied == false);
                tvm.Tables = temp.ToList<Table>();
                return View(tvm);
            }
            return View("HomePage");
        }

        public ActionResult Sit(string TableId)
        {
            string client = "";
            if (Session["Client"] != null && Session["orderingClient"] == null)
            {
                client = ((Client)Session["Client"]).PhoneNum;
            }
            else
            {
                if (Session["Client"] == null && Session["orderingClient"] != null)
                {
                    client = ((Client)Session["orderingClient"]).PhoneNum;
                }
            }
            UserDal dal = new UserDal();
            var check = dal.Tables.Where(x => x.Client.Equals(client) && x.Occupied == true).SingleOrDefault();
            if (check == null)
            {
                dal.Tables.Find(Int32.Parse(TableId)).Client = client;
                dal.Tables.Find(Int32.Parse(TableId)).Occupied = true;
                dal.SaveChanges();
            }
            else
            {
                dal.Tables.Find(check.TableId).Client = null;
                dal.Tables.Find(check.TableId).Occupied = false;
                dal.Tables.Find(Int32.Parse(TableId)).Client = client;
                dal.Tables.Find(Int32.Parse(TableId)).Occupied = true;
                dal.SaveChanges();
            }
            if (Session["Client"] == null && Session["orderingClient"] != null)
            {
                UserViewModel ivm2 = new UserViewModel();
                ivm2.Items = dal.Items.ToList<Item>();
                return View("PickItems", ivm2);
            }
            UserViewModel ivm = new UserViewModel();
            ivm.Items = dal.Items.ToList<Item>();
            return View("CartMenu", ivm);
        }

        public ActionResult signOut()
        {
            Session.RemoveAll();
            return View("index");
        }

    }

}