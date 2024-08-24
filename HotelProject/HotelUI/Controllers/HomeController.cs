using HotelUI.Exceptions;
using HotelUI.Filters;
using HotelUI.Models.Branch;
using HotelUI.Models.Reservation;
using HotelUI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace HotelUI.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class HomeController : Controller
    {
        private HttpClient _client;
        private readonly ICrudService _crudService;

        public HomeController(ICrudService crudService)
        {
            _client = new HttpClient();
            _crudService = crudService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("api/branches/incomes")]
        public async Task<IActionResult> BranchIncomes()
        {
            try
            {
                var incomes = await _crudService.GetAsyncBranchesIncome<List<BranchIncomeResponse>>("branches/incomes");

                return Ok(incomes);
            }
            catch (HttpException e)
            {
                if (e.Status == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("login", "auth");
                }
                else
                {
                    return RedirectToAction("error", "home");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("error", "home");
            }
        }



        [HttpGet("api/current-year-monthly-income")]
        public async Task<IActionResult> GetCurrentYearMonthlyIncome()
        
        {
            try
            {
                var monthlyIncome = await _crudService.GetOrdersPricePerYearAsync();
                return Ok(monthlyIncome);
            }
            catch (HttpException ex)
            {
                if (ex.Status == System.Net.HttpStatusCode.Unauthorized)
                {
                    return Unauthorized("You are not authorized to access this resource.");
                }
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }



        [HttpGet("api/get-total-reservations")]
        public async Task<IActionResult> GetTotalReservations()
        {
            try
            {
                int count = await _crudService.GetTotalReservationsCountAsync("reservations/count");
                return Ok(new { TotalReservations = count });
            }
            catch (HttpException e)
            {
                if (e.Status == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("login", "auth");
                }
                else
                {
                    return RedirectToAction("error", "home");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("error", "home");
            }
        }


        [HttpGet("api/get-totalPrice-reservations")]
        public async Task<IActionResult> GetTotalPriceReservation()
        {
            try
            {
                double count = await _crudService.GetTotalPriceReservationsAsync("reservations/total-price");
                return Ok(new { TotalPrice = count });
            }
            catch (HttpException e)
            {
                if (e.Status == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("login", "auth");
                }
                else
                {
                    return RedirectToAction("error", "home");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("error", "home");
            }
        }


        [HttpGet("api/get-total-registered")]
        public async Task<IActionResult> GetMemberRegisterCount()
        {
            try
            {
                int count = await _crudService.GetMemberRegisteredCountAsync("auth/registered-users-count");
                return Ok(new { Count = count });
            }
            catch (HttpException e)
            {
                if (e.Status == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("login", "auth");
                }
                else
                {
                    return RedirectToAction("error", "home");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("error", "home");
            }
        }



        [HttpGet("api/get-ourStaff-count")]
        public async Task<IActionResult> OurStaffCount()
        {
            try
            {
                int count = await _crudService.GetOurStaffCount("ourStaff/count");
                return Ok(new { Count = count });
            }
            catch (HttpException e)
            {
                if (e.Status == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("login", "auth");
                }
                else
                {
                    return RedirectToAction("error", "home");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("error", "home");
            }
        }



        public IActionResult Error()
        {
            return View();
        }
    }
}
