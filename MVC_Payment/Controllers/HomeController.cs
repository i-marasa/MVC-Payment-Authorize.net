using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_Payment.Models;

namespace MVC_Payment.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.css = "hidden";
            return View();
        }


        [HttpPost]
        public ActionResult PaymentProcess(PaymentModel payment)
        {
            TransactionResponse result = new PaymentProcesses().ChargeCredit(payment);
            string respMsg = "";
            if (result != null && result.resultCode == AuthorizeNet.Api.Contracts.V1.messageTypeEnum.Ok)
            {
                respMsg += "Successfully created transaction with Transaction ID: " + result.transId + "<br />";
                respMsg += "Response Code: " + result.responseCode + "<br />";
                respMsg += "Message Code: " + result.messageCode + "<br />";
                respMsg += "Description: " + result.description + "<br />";
                respMsg += "Success, Auth Code : " + result.authCode + "<br />";
                ViewBag.css = "alert alert-success";
            }
            else
            {
                respMsg = "Error Code: " + result.errorCode + "<br />" +
                    "Error message: " + result.errorText;
                ViewBag.css = "alert alert-danger";
            }
            ViewBag.Msg = respMsg;

            return View("index");
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
    }
}