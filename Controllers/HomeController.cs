using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Anagram.Models;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Anagrams.Interfaces;

namespace Anagram.Controllers
{
    public class HomeController : Controller
    {
        private IAnagramService _anagramService;
        private Stopwatch _watch;

        [BindProperty]
        public AnagramViewModel AnagramViewModel { get; set; }

        public HomeController(IAnagramService anagramService)
        {
            _anagramService = anagramService;
            _watch = new Stopwatch();
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Search(string search)
        {
            if (string.IsNullOrEmpty(search)) return RedirectToAction("Error", new { errorTitle = "Invalid input", errorMessage = "Cannot find anagrams with empty strings" });
            if (CheckForInvalidInputs(search)) return RedirectToAction("Error", new { errorTitle = "Invalid input", errorMessage = "Input only accepts letters without accents and white spaces" });

            search = search.ToUpper();

            _watch.Start();

            var anagramsList = await _anagramService.SearchForAnagrams(search);

            _watch.Stop();

            var viewModel = new AnagramViewModel
            {
                Anagrams = anagramsList,
                Search = search,
                RequestTime = _watch.Elapsed.ToString(@"mm\:ss\:fff")
            };

            return View("Index", viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string errorTitle, string errorMessage)
        {
            return View(new ErrorViewModel
            {
                ErrorTitle = errorTitle,
                ErrorMessage = errorMessage
            });
        }

        private bool CheckForInvalidInputs(string input)
        {
            return Regex.Match(input, "[^a-zA-Z ]+").Success;
        }
    }
}