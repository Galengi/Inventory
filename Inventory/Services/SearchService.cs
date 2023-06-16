using Inventory.Models.DB;
using Inventory.Models.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using HtmlAgilityPack;
using PuppeteerSharp;
using System.Globalization;
using System.Text.RegularExpressions;
using Inventory.Models.OwnModels;
using PuppeteerSharp.Input;

namespace Inventory.Services
{
    public class SearchService: ISearchService
    {
        private readonly InventoryContext _context;
        private IProductService _productService;
        private readonly HttpClient _httpClient;
        private int maxElementCount = 20;

        public SearchService(InventoryContext context, 
            IProductService productService,
            HttpClient httpClient)
        {
            this._context = context;
            _productService = productService;
            this._httpClient = httpClient;
        }

        public async Task<ActionResult<SearchRequest>> Get(string name)
        {
            try
            {
                SearchRequest oSearch = new SearchRequest();
                oSearch.Name = name;

                /*
                var lstProducts = await _context.Products.Where((d) => d.Name == name).ToListAsync();
                foreach (Product product in lstProducts)
                {
                    var prod = await _productService.Get(product.Id);
                    oSearch.Products.Add(prod.Value);
                }
                */


                oSearch.Products = await _context.Products.Where((d) => d.Name.Contains(name)).ToListAsync();
                oSearch.Companies = await _context.Companies.Where((d) => d.Name.Contains(name)).ToListAsync();
                oSearch.Tags = await _context.Tags.Where((d) => d.Name.Contains(name)).ToListAsync();

                if (oSearch.Products.Count==0 && oSearch.Companies.Count == 0 && oSearch.Tags.Count == 0)
                {
                    throw new Exception("No se ha encontrado nada con el nombre" + name);
                }

                return oSearch;
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un error en la obtención");
            }
        }

        public async Task<ActionResult<SearchRequest>> GetFromApi(string name)
        {
            try
            {
                SearchRequest oSearch = new SearchRequest();
                oSearch.Name = name;

                /*
                var lstProducts = await _context.Products.Where((d) => d.Name == name).ToListAsync();
                foreach (Product product in lstProducts)
                {
                    var prod = await _productService.Get(product.Id);
                    oSearch.Products.Add(prod.Value);
                }
                */


                oSearch.Products = await _context.Products.Where((d) => d.Name.Contains(name)).ToListAsync();
                oSearch.Companies = await _context.Companies.Where((d) => d.Name.Contains(name)).ToListAsync();
                oSearch.Tags = await _context.Tags.Where((d) => d.Name.Contains(name)).ToListAsync();

                if (oSearch.Products.Count == 0 && oSearch.Companies.Count == 0 && oSearch.Tags.Count == 0)
                {
                    throw new Exception("No se ha encontrado nada con el nombre" + name);
                }

                return oSearch;
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un error en la obtención");
            }
        }
        public async Task<IEnumerable<BasicItem>> GetFromCarrefour(string name)
        {
            try
            {
                var url = $"https://www.carrefour.es/?q={name}&page=1";

                await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
                var browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true
                });

                using (var page = await browser.NewPageAsync())
                {
                    page.DefaultNavigationTimeout = 1200000;
                    await page.GoToAsync(url);
                    await page.WaitForNetworkIdleAsync(new WaitForNetworkIdleOptions { Timeout = 120000 });
                    //await page.WaitForXPathAsync(".//article[@class='ebx-result ebx-grid-item']");
                    await Task.Delay(3000);

                    var html = await page.GetContentAsync();
                    var document = new HtmlDocument();
                    document.LoadHtml(html);

                    var productNodes = document.DocumentNode.Descendants("article")
                        .Where(node => node.GetAttributeValue("class", "").Contains("ebx-result ebx-grid-item"));

                    var count = maxElementCount;
                    var items = new List<BasicItem>();
                    foreach (var node in productNodes)
                    {
                        var itemName = node.Descendants("h1")
                            .Where(n => n.GetAttributeValue("class", "").Contains("ebx-result-title ebx-result__title"))
                            .FirstOrDefault()?.InnerText.Trim();

                        var priceString = node.Descendants("strong")
                            .Where(n => n.GetAttributeValue("class", "").Contains("ebx-result-price__value"))
                            .FirstOrDefault()?.InnerText.Trim();

                        var priceUnitString = node.Descendants("div")
                            .Where(n => n.GetAttributeValue("class", "").Contains("ebx-result__quantity ebx-result-quantity"))
                            .FirstOrDefault()?.InnerText.Trim();

                        var imageUrl = node.Descendants("img")
                            .FirstOrDefault()?.GetAttributeValue("src", "").Trim();

                        if (itemName is not null && imageUrl is not null)
                        {
                            count--;
                            if (count < 0)
                            {
                                break;
                            }
                            (decimal price, string currency) = GetPrice(priceString);
                            (decimal priceUnit, string Unit) = GetType(priceUnitString, "carrefour");
                            //string[] itemSplitedName = GetOrderedName(itemName);
                            string[] itemSplitedName = new string[] { };
                            //byte[] imageBytes = await GetImage(imageUrl, "carrefour");

                            var item = new BasicItem()
                            {
                                Name = itemName,
                                Price = price,
                                PriceUnit = priceUnit,
                                Currency = currency,
                                Unit = Unit,
                                //Image = imageBytes,
                                Company = "Carrefour",
                                ImageUrl = imageUrl,
                            };
                            items.Add(item);
                        }

                    }
                    await browser.CloseAsync();

                    return items;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception is"+ ex);
                throw;
            }
            
        }
        public async Task<IEnumerable<BasicItem>> GetFromConsum(string name)
        {
            var items = new List<BasicItem>();
            try
            {
                var url = $"https://tienda.consum.es/es/s/{name}?orderById=13&page=1&limit=25";

                await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
                var browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true
                });

                using (var page = await browser.NewPageAsync())
                {
                    page.DefaultNavigationTimeout = 1200000;
                    await page.GoToAsync(url);
                    await page.WaitForNetworkIdleAsync(new WaitForNetworkIdleOptions { Timeout = 120000 });
                    //await page.WaitForNavigationAsync(new NavigationOptions {Timeout = 120000 });
                    //await page.WaitForXPathAsync(".//cmp-widget-product[@class='grid__widget--prod']");

                    var html = await page.GetContentAsync();
                    var document = new HtmlDocument();
                    document.LoadHtml(html);

                    var productNodes = document.DocumentNode.Descendants("cmp-widget-product")
                        .Where(node => node.GetAttributeValue("class", "").Contains("grid__widget--prod"));

                    var count = maxElementCount;
                    foreach (var node in productNodes)
                    {
                        var itemName = node.Descendants("h3")
                            .Where(n => n.GetAttributeValue("class", "").Contains("widget-prod__info-description"))
                            .FirstOrDefault()?.InnerText.Trim();

                        var priceString = node.Descendants("span")
                            .Where(n => n.GetAttributeValue("class", "").Contains("widget-prod__price--bold"))
                            .FirstOrDefault()?.InnerText.Trim();

                        var priceUnitString = node.Descendants("div")
                            .Where(n => n.GetAttributeValue("class", "").Contains("widget-prod__info-unitprice"))
                            .FirstOrDefault()?.InnerText.Trim();

                        var imageUrl = node.Descendants("img")
                            .FirstOrDefault()?.GetAttributeValue("src", "").Trim();

                        if (itemName is not null)
                        {
                            count--;
                            if (count < 0)
                            {
                                break;
                            }
                            //string[] itemSplitedName = GetOrderedName(itemName);
                            string[] itemSplitedName = new string[] { };
                            (decimal price, string currency) = GetPrice(priceString);
                            (decimal priceUnit, string Unit) = GetType(priceUnitString, "consum");
                            byte[] imageBytes = await GetImage(imageUrl, "consum");

                            var item = new BasicItem()
                            {
                                Name = itemName,
                                OrderedName = itemSplitedName,
                                Price = price,
                                PriceUnit = priceUnit,
                                Currency = currency,
                                Unit = Unit,
                                //Image = imageBytes,
                                Company = "Consum",
                                ImageUrl = imageUrl,
                            };
                            items.Add(item);
                        }

                    }
                    await browser.CloseAsync();

                    return items;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception is" + ex);
                return items;
            }

        }

        public async Task<IEnumerable<BasicItem>> GetFromMercadona(string name)
        {
            try
            {
                var bas = "https://tienda.mercadona.es/";
                var url = $"https://tienda.mercadona.es/search-results?query={name}";

                var options = new LaunchOptions
                {
                    Headless = true, // Para ver la acción que se está realizando
                    //DefaultViewport = null // Para tener un viewport personalizado
                };

                await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);

                using (var browser = await Puppeteer.LaunchAsync(options))
                using (var page = await browser.NewPageAsync())
                {
                    page.DefaultNavigationTimeout = 1200000;
                    await page.GoToAsync(bas);
                    await page.WaitForNetworkIdleAsync(new WaitForNetworkIdleOptions { Timeout = 120000 });

                    await page.TypeAsync("input[name='postalCode'][class='ym-hide-content']", "46670", new TypeOptions { Delay = 100 });
                    await page.ClickAsync("button[type = button].ui-button.ui-button--small.ui-button--primary.ui-button--positive");
                    await page.ClickAsync("button[type = button][aria-label = Continuar].button.button-primary.button-big");
                    await page.WaitForNavigationAsync(new NavigationOptions { Timeout = 50000 });

                    await page.TypeAsync("input[name='search'][class='body1-r search__input']", name, new TypeOptions { Delay = 100 });
                    await Task.Delay(5000);


                    var html = await page.GetContentAsync();
                    var document = new HtmlDocument();
                    document.LoadHtml(html);

                    var productNodes = document.DocumentNode.Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "").Equals("product-cell"));


                    var items = new List<BasicItem>();
                    var count = maxElementCount;
                    foreach (var node in productNodes)
                    {
                        var itemName = node.Descendants("h4")
                            .Where(n => n.GetAttributeValue("class", "").Contains("product-cell__description-name"))
                            .FirstOrDefault()?.InnerText.Trim();

                        var priceString = node.Descendants("p")
                            .Where(n => n.GetAttributeValue("class", "").Contains("product-price__unit-price"))
                            .FirstOrDefault()?.InnerText.Trim();

                        var priceUnitString = node.Descendants("p")
                            .Where(n => n.GetAttributeValue("class", "").Contains("product-price__extra-price"))
                            .FirstOrDefault()?.InnerText.Trim();

                        var imageUrl = node.Descendants("img")
                            .FirstOrDefault()?.GetAttributeValue("src","").Trim();
                        /*
                        var imageUrl2 = node.Descendants("div")
                            .Where(n => n.GetAttributeValue("class", "").Contains("product-cell__image-wrapper"))
                            .FirstOrDefault()?.InnerText.Trim();
                        */


                        if (itemName is not null)
                        {
                            count--;
                            if (count <0 )
                            {
                                break;
                            }
                            //string[] itemSplitedName = GetOrderedName(itemName);
                            string[] itemSplitedName = new string[] { };
                            (decimal price, string currency) = GetPrice(priceString);
                            (decimal priceUnit, string Unit) = GetType("", "consum");
                            //byte[] imageBytes = await GetImage(imageUrl, "mercadona");

                            //byte[] imageBytes = await _httpClient.GetByteArrayAsync(imageUrl);
                            //await System.IO.File.WriteAllBytesAsync(savePath, imageBytes);

                            var item = new BasicItem()
                            {
                                Name = itemName,
                                OrderedName = itemSplitedName,
                                Price = price,
                                PriceUnit = priceUnit,
                                Currency = currency,
                                Unit = Unit,
                                //Image = imageBytes,
                                Company = "Mercadona",
                                ImageUrl = imageUrl,
                            };
                            items.Add(item);
                        }
                    }
                    await browser.CloseAsync();

                    return items;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception is" + ex);
                throw;
            }
        }

        public async Task<IEnumerable<BasicItem>> GetFromDia(string name)
        {
            try
            {
                var url = $"https://www.dia.es";
                //var url = $"https://www.dia.es/compra-online/search?q={name}%3Arelevance&page=0&disp=0";

                await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
                var browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true
                });

                using (var page = await browser.NewPageAsync())
                {
                    page.DefaultNavigationTimeout = 1200000;
                    await page.GoToAsync(url);
                    await page.WaitForNetworkIdleAsync(new WaitForNetworkIdleOptions { Timeout = 120000 });
                    //await page.WaitForNavigationAsync(new NavigationOptions {Timeout = 120000 });
                    //await page.WaitForXPathAsync(".//cmp-widget-product[@class='grid__widget--prod']");

                    var html = await page.GetContentAsync();
                    var document = new HtmlDocument();
                    document.LoadHtml(html);

                    var productNodes = document.DocumentNode.Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "").Contains("prod_grid"));


                    var items = new List<BasicItem>();
                    foreach (var node in productNodes)
                    {
                        var itemName = node.Descendants("span")
                            .Where(n => n.GetAttributeValue("class", "").Contains("details"))
                            .FirstOrDefault()?.InnerText.Trim();

                        var priceString = node.Descendants("p")
                            .Where(n => n.GetAttributeValue("class", "").Contains("price"))
                            .FirstOrDefault()?.InnerText.Trim();

                        var priceUnitString = node.Descendants("p")
                            .Where(n => n.GetAttributeValue("class", "").Contains("pricePerKilogram"))
                            .FirstOrDefault()?.InnerText.Trim();

                        if (itemName is not null)
                        {
                            //string[] itemSplitedName = GetOrderedName(itemName);
                            string[] itemSplitedName = new string[] { };
                            (decimal price, string currency) = GetPrice(priceString);
                            (decimal priceUnit, string Unit) = GetType(priceUnitString, "consum");

                            var item = new BasicItem()
                            {
                                Name = itemName,
                                OrderedName = itemSplitedName,
                                Price = price,
                                PriceUnit = priceUnit,
                                Currency = currency,
                                Unit = Unit,
                                Company = "Dia"
                            };
                            items.Add(item);
                        }

                    }
                    await browser.CloseAsync();

                    return items;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception is" + ex);
                throw;
            }

        }
        public string[] GetOrderedName(string input)
        {
            // Palabras a remover
            string[] stopWords = new string[] {"a","acá","ahí","al","algo","algún","alguna","algunas","alguno","algunos","allá","allí","ambos","ante","antes","aquel","aquella","aquellas","aquello","aquellos","aquí","arriba","así","atrás","aún","bajo","bastante","bien","cada","casi","cierta","ciertas","cierto","ciertos","como","cómo","con","conmigo","conseguimos","conseguir","consigo","consigue","consiguen","contigo","contra","cual","cuales","cualquier","cualquiera","cuán","cuando","cuanta","cuantas","cuánto","cuántos","cuenta","da","dado","dan","dar","de","debajo","del","delante","demás","demasiado","dentro","deprisa","desde","después","detras","día","días","donde","dos","durante","e","é","el","ella","ellas","ellos","empleáis","emplean","emplear","empleas","empleo","en","encima","encuentra","enfrente","enseguida","entre","era","eramos","eran","eras","eres","es","esa","esas","ese","eso","esos","esta","está","estaba","estábamos","estaban","estáis","estamos","están","estar","este","esto","estos","estoy","etc","fin","fue","fueron","fui","fuimos","gente","ha","habéis","haber","había","habíais","habíamos","habían","hacer","hacia","hago","han","hasta","hay","haya","he","hecho","hemos","hicieron","hizo","hoy","hubo","igual","incluso","indicó","informo","informó","intenta","intentais","intentamos","intentan","intentar","intentas","intento","ir","jamás","junto","juntos","la","lado","las","le","les","lo","los","luego","lugar","más","mal","manera","manifestó","me","mediante","medio","mejor","acerca","además","adonde","ahora","alrededor","apenas","aproximadamente","aun","aunque","cabe","cerca","ciertamente","cinco","cosas","creo","cualquieras","cuanto","cuatro","despacio","despues","detrás","dia","dice","dicen","dicho","dieron","diferente","diferentes","dijeron","dijo","dio","ejemplo","ello","embargo","entonces","estado","estados","fuera","gran","grandes","habrá","hace","hacen","haciendo","horas","alli","cuan","cuantos","dejar","él","general","hora","mayoría","mencionó","menos","mi","mientras","mio","modo","momento","mucha","muchas","muchísima","muchísimas","muchísimo","muchísimos","mucho","muchos","muy","nada","ni","ningún","ninguna","ningunas","ninguno","ningunos","no","nos","nosotras","nosotros","nuestra","nuestras","nuestro","nuestros","nueva","nuevo","nunca","o","ocho","os","otra","otras","otro","otros","para","por","según","sin","sobre","tras","un","una","unas","uno","unos","y","ajena","ajenas","ajeno","ajenos","aquél","cuál","cuáles","cuálquier","cuálquiera","cuálquieras","cuánta","cuántas","demasiada","demasiadas","demasiados","dias","empleais","estará","estarán","estarás","estaré","estaréis","estaremos","estaría","estaríais","estaríamos","estarían","estarías","estas","esté","estéis","estemos","estén","estés"};

            // Separar la frase en palabras
            string[] orderedName = input.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // Remover conectores
            orderedName = orderedName.Where(p => !stopWords.Contains(p)).ToArray();

            // Ordenar alfabéticamente las palabras
            Array.Sort(orderedName);

            return orderedName;
        }
        public static string[] RemoveDuplicates(string[] arr)
        {
            HashSet<string> set = new HashSet<string>(arr);
            string[] result = new string[set.Count];
            set.CopyTo(result);
            return result;
        }
        public (decimal, string) GetPrice(string input)
        {
            if (input is not null)
            {

                input = input.Replace("&nbsp;", "");
                input = input.Replace(",", ".");
                decimal price = decimal.Parse(input.Trim(' ', '\u20AC', '$'), CultureInfo.InvariantCulture);
                string currency = input.Contains('\u20AC') ? "EUR" : "USD";
                return (price, currency);
            }
            return (-1, "error");
        }
        public async Task<byte[]> GetImage(string input, string company)
        {
            if (input is not null)
            {
                if (company == "mercadona" || company == "consum")
                {
                    Regex rx = new Regex(@"([^?]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    MatchCollection matches = rx.Matches(input);
                    return await _httpClient.GetByteArrayAsync(matches[0].ToString());
                }
                else if (company == "carrefour")
                {
                    return await _httpClient.GetByteArrayAsync(input);
                }
            }
            return new byte[0];
        }
        public (decimal value, string type) GetType(string input, string source)
        {
            if (input is not null && input != "")
            {
                var numbers = new Char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                if (source.Equals("carrefour"))
                {
                    input = input.Replace("&nbsp;", "");
                    var regex = new Regex(@"([\d.,]+)\s+([^\d.,]+)");
                    var match = regex.Match(input);
                    if (match.Success)
                    {
                        var valueStr = match.Groups[1].Value.Replace(",", ".");
                        var value = decimal.Parse(valueStr, CultureInfo.InvariantCulture);
                        var type = match.Groups[2].Value.Trim();
                        type = type.Replace("€/", "").Replace("$/", "");
                        return (value, type);
                    }
                    else
                    {
                        throw new ArgumentException($"Input string '{input}' is not in the correct format.");
                    }
                }
                else if (source.Equals("consum"))
                {
                    input = input.Replace("&nbsp;", "").Replace("€", "").Replace("$", "").Trim();
                    var splitInput = input.Split("/");

                    var valueStr = splitInput[0].Replace(",", ".");
                    var value = decimal.Parse(valueStr, CultureInfo.InvariantCulture);
                    var type = splitInput[1].Trim(numbers).Trim();
                    //type = type.Replace("€/", "").Replace("$/", "");
                    return (value, type);
                }
            }
            return (0, "none");
        }

    }
}
