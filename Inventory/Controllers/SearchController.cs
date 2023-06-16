using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Inventory.Models.Request;
using Inventory.Models.Response;
using Inventory.Services;
using PuppeteerSharp;
using System.Globalization;
using System.Text.RegularExpressions;
using Inventory.Models.OwnModels;

namespace Inventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private ISearchService _search;
        private readonly HttpClient _httpClient;

        public SearchController(ISearchService search,
            HttpClient httpClient)
        {
            this._search = search;
            this._httpClient = httpClient;
        }

        
        // GET: api/Search/llet
        [HttpGet("{name}")]
        public async Task<ActionResult<SearchRequest>> GetTag(string name)
        {
            Inventory.Models.Response.Response oResponse = new Inventory.Models.Response.Response();

            try
            {
                // Create a list of asynchronous tasks
                List<Task<IEnumerable<BasicItem>>> tasks = new List<Task<IEnumerable<BasicItem>>>
                {
                    _search.GetFromCarrefour(name),
                    _search.GetFromConsum(name),
                    _search.GetFromMercadona(name)
                };

                // Wait for all tasks to complete
                IEnumerable<BasicItem>[] results = await Task.WhenAll(tasks);

                // Process the results
                var totalItems = new List<BasicItem>();
                var totalI2 = new List<BasicItem>();
                foreach (IEnumerable<BasicItem> result in results)
                {
                    totalItems.AddRange(result);
                }
                totalI2 = totalItems.OrderBy(x => x.Price).ToList();

                // Dispose resources
                foreach (Task<IEnumerable<BasicItem>> task in tasks)
                {
                    task.Dispose();
                }
                /*
                var carrefourItems = new List<BasicItem>();
                carrefourItems = await _search.GetFromCarrefour(name) as List<BasicItem>;
                var consumItems = new List<BasicItem>();
                consumItems = await _search.GetFromConsum(name) as List<BasicItem>;
                var mercadonaItems = new List<BasicItem>();
                mercadonaItems = await _search.GetFromMercadona(name) as List<BasicItem>;
                totalItems.AddRange(carrefourItems);
                totalItems.AddRange(consumItems);
                totalItems.AddRange(mercadonaItems);
                //var search = await _search.Get(name);
                */
                oResponse.Data = totalI2;
                oResponse.Success = 1;
            }
            catch (Exception ex)
            {

                oResponse.Message = ex.Message;
            }

            return Ok(oResponse);


        }
        
        // GET: api/Search/llet

        /*
        [HttpGet("{name}")]
        public async Task<ActionResult<SearchRequest>> GetTag(string name)
        {
            Response oResponse = new Response();

            try
            {
                var search = await _search.Get(name);


                oResponse.Data = search;
                oResponse.Success = 1;
            }
            catch (Exception ex)
            {

                oResponse.Message = ex.Message;
            }

            return Ok(oResponse);


        }
        */

    }
}
