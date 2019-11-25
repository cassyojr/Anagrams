using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Anagram.Models;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Anagram.Controllers
{
    public class HomeController : Controller
    {
        [BindProperty]
        public AnagramViewModel AnagramViewModel { get; set; }

        public HomeController()
        {
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Search(string search)
        {
            search = search.ToUpper();

            if (string.IsNullOrEmpty(search)) return RedirectToAction("Error", new { errorTitle = "Invalid input", errorMessage = "Cannot find anagrams with empty strings" });
            if (CheckForInvalidInputs(search)) return RedirectToAction("Error", new { errorTitle = "Invalid input", errorMessage = "Input only accepts letters without accents and white spaces" });

            var watch = new Stopwatch();
            watch.Start();

            var words = System.IO.File.ReadAllLines(@"Infrastructure\words.txt");
            var anagramsList = new List<string>();

            await Task.Run(() =>
            {
                var filteredWordsList = FilterWords(search, words);
                anagramsList = FindAnagrams(search, filteredWordsList);
            });

            watch.Stop();

            var viewModel = new AnagramViewModel
            {
                Anagrams = anagramsList,
                Search = search,
                RequestTime = watch.Elapsed.ToString(@"mm\:ss\:fff")
            };

            return View("Index", viewModel);
        }

        private List<string> FindAnagrams(string term, List<string> words)
        {
            var anagramsList = new List<string>();

            for (var i = 0; i <= words.Count - 1; i++)
                Recursive(term, words[i], words, anagramsList);

            return anagramsList;
        }

        private void Recursive(string term, string word, List<string> words, List<string> anagramsList)
        {
            var convertedTerm = term.ToCharArray();
            Array.Sort(convertedTerm);
            var sortedTerm = new string(convertedTerm);
            var termLength = term.Length;

            for (var i = 0; i <= words.Count - 1; i++)
            {
                if (word == words[i]) continue;
                var currentWordLetters = words[i].Trim().ToCharArray();
                var currentSearchContainsWord = currentWordLetters.All(letter => term.Contains(letter));
                var newTerm = (word + " " + words[i]);
                var currentLengthMatches = newTerm.Replace(" ", string.Empty).Length == termLength;
                var currentLengthInRange = newTerm.Replace(" ", string.Empty).Length < termLength;

                if (currentLengthInRange) Recursive(term, newTerm, words, anagramsList);
                else if (currentLengthMatches)
                {
                    var convertedNewTerm = newTerm.Replace(" ", string.Empty).ToCharArray();
                    Array.Sort(convertedNewTerm);
                    var sortedNewTerm = new string(convertedNewTerm);

                    if (sortedNewTerm != sortedTerm) continue;

                    var existentTerm = anagramsList.Any(existingTerm =>
                     {
                         var splitedExistingTerm = existingTerm.Split(" ");
                         Array.Sort(splitedExistingTerm);

                         var splitedNewTerm = newTerm.Split(" ");
                         Array.Sort(splitedNewTerm);

                         return splitedExistingTerm.SequenceEqual(splitedNewTerm);
                     });

                    if (!existentTerm)
                        anagramsList.Add(newTerm);
                }
            }
        }

        private List<string> FilterWords(string term, IEnumerable<string> words)
        {
            var wordsList = new List<string>();

            foreach (var word in words)
            {
                var currentWordLetters = word.ToCharArray();
                var currentTermContainsWord = currentWordLetters.All(letter => term.Contains(letter));
                if (currentTermContainsWord && term != word) wordsList.Add(word);
            }

            return wordsList;
        }

        private bool CheckForInvalidInputs(string input)
        {
            return Regex.Match(input, "[^a-zA-Z ]+").Success;
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
    }
}