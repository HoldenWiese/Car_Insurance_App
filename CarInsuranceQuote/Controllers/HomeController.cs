using CarInsuranceQuote.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CarInsuranceQuote.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetQuote(string firstName,
                                   string lastName,
                                   string emailAddress,
                                   System.DateTime dateOfBirth,
                                   int carYear,
                                   string carMake,
                                   string carModel,
                                   string dUIHistory,
                                   int totalTickets,
                                   string fullCoverage)
        {
            int dUI = dUIHistory == "no" ? 0 : 1;
            int coverage = fullCoverage == "no" ? 0 : 1;
            myMethods instance = new myMethods();


            using (QuotesEntities db = new QuotesEntities())
            {
                var customer = new Customer();
                customer.FirstName = firstName;
                customer.LastName = lastName;
                customer.EmailAddress = emailAddress;
                customer.DateOfBirth = dateOfBirth;
                customer.CarYear = carYear;
                customer.CarMake = carMake;
                customer.CarModel = carModel;
                customer.DUIHistory = dUI;
                customer.TotalTickets = totalTickets;
                customer.FullCoverage = coverage;
                customer.Quote = instance.getQuote(dateOfBirth, carYear, carMake, carModel, totalTickets, dUIHistory, fullCoverage);

                db.Customers.Add(customer);
                db.SaveChanges();
            }

            return Redirect("Admin");
        }

        public ActionResult Admin()
        {
            QuotesEntities db = new QuotesEntities();
            var customers = db.Customers;
            return View(customers);
        }
    }

    public class myMethods
    {
        public int getQuote(DateTime dateOfBirthForm, 
                            int carYearForm, 
                            string carMakeForm, 
                            string carModelForm, 
                            int totalTicketsForm,
                            string dUIHistoryForm,
                            string fullCoverageForm)
        {
            decimal totalEst = 50m;

            DateTime zeroTime = new DateTime(1, 1, 1);
            DateTime a = dateOfBirthForm;
            DateTime b = DateTime.Today;
            TimeSpan span = b - a;
            int age = (zeroTime + span).Year - 1;

            //If the user is under 25, add $25 to the monthly total.
            if (age < 25) totalEst += 25m;
            //Not certain if less than 18 should add addition 100 to the 25 for being under 25 or just 100 total?
            if (age < 18) totalEst += 100m;
            //If the user is over 100, add $25 to the monthly total.
            if (age > 100) totalEst += 25m;
            //If the car's year is before 2000, add $25 to the monthly total.
            int carYear = carYearForm;
            if (carYear < 2000) totalEst += 25m;
            //If the car's year is after 2015, add $25 to the monthly total.
            if (carYear > 2015) totalEst += 25m;
            //If the car's Make is a Porsche, add $25 to the price.
            string carMake = carMakeForm.ToLower();
            if (carMake == "porsche") totalEst += 25m;
            //If the car's Make is a Porsche and its model is a 911 Carrera, add an additional $25 to the price.
            string carModel = carModelForm.ToLower();
            if (carModel == "911 carrera") totalEst += 25m;
            //Add $10 to the monthly total for every speeding ticket the user has.
            decimal ticketsFee = totalTicketsForm * 10m;
            totalEst += ticketsFee;
            //If the user has ever had a DUI, add 25% to the total.
            if (dUIHistoryForm == "yes") totalEst = decimal.Multiply(totalEst, 1.25m);
            //If it's full coverage, add 50% to the total.
            if (fullCoverageForm == "yes") totalEst = decimal.Multiply(totalEst, 1.5m);

            return Convert.ToInt32(totalEst);
        }
    }
}