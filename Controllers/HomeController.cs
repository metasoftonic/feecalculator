using FeeCalculator.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace FeeCalculator.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.display = false;
            

            return View();
        }
        private FeesArray LoadJson(string path)
        {
            var items = new FeesArray();
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                items = JsonConvert.DeserializeObject<FeesArray>(json);
            }
            return items;
        }

        private (long charge, FeesArray feesArray) ChargeCalculate(decimal amount)
        {
            if (amount < 1)
            {
                return (0, null);
            }
            string path = Server.MapPath("~/Config/Fees.json");
            var feeConfigurations = LoadJson(path);
            var charge = feeConfigurations.Fees.FirstOrDefault(x => amount >= x.MinAmount && amount <= x.MaxAmount);
            return (charge.FeeAmount, feeConfigurations);
        }

        public ActionResult TransferFee(decimal amount)
        {
            var calculatedCharge = ChargeCalculate(amount);
            var charge = (calculatedCharge.charge);
            if (charge < 1)
            {
                return Json(new
                {
                    status = false,
                    message = "Transaction amount must be greater than 0",
                });
            } 
            var maxAmount = calculatedCharge.feesArray.Fees.Max(x => x.MaxAmount);
            if (amount > maxAmount)
            {
                var maxcharge = calculatedCharge.feesArray.Fees.FirstOrDefault(x => x.MaxAmount == maxAmount);
                charge = maxcharge.FeeAmount;
            }

            return Json(new
            {
                status = true,
                message = "Transaction charge calculated",
                charge = charge,
                totalCharge = charge + amount
            });
        }
        public ActionResult SurChargeFee(decimal amount)
        {
            var calculatedCharge = ChargeCalculate(amount);
            var charge = (calculatedCharge.charge);
            if (charge < 1)
            {
                return Json(new
                {
                    status = false,
                    message = "Transaction amount must be greater than 0",
                });
            }
            var transferAmount = amount - charge;
            var debitAmount = transferAmount + charge;
            return Json(new
            {
                status = true,
                message = "Transaction charge calculated",
                charge,
                transferAmount,
                debitAmount,
                amount
            });

        }
            
    }
}