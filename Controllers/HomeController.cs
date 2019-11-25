using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Anagram.Models;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

namespace Anagram.Controllers
{
    public class HomeController : Controller
    {
        private List<string> Anagrams = new List<string>();

        [BindProperty]
        public AnagramViewModel AnagramViewModel { get; set; }

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Search(string search)
        {
            search = search.ToUpper();

            if (string.IsNullOrEmpty(search)) return RedirectToAction("Error", new { errorTitle = "Invalid input", errorMessage = "Cannot find anagrams with empty strings" });
            if (CheckForInvalidInputs(search)) return RedirectToAction("Error", new { errorTitle = "Invalid input", errorMessage = "Input only accepts letters and white spaces" });

            var words = System.IO.File.ReadAllLines(@"Infrastructure\words2.txt");

            var filteredWordsList = FilterWords(search, words);
            FindAnagrams(search, words.ToList());

            foreach (var anagram in Anagrams.Distinct())
                Console.WriteLine(anagram);

            await Task.Run(() => { });

            return Ok();
        }

        private void FindAnagrams(string term, List<string> words)
        {
            // var convertedTerm = term.ToCharArray();
            // Array.Sort(convertedTerm);
            // var sortedTerm = new string(convertedTerm);

            for (var i = 0; i <= words.Count - 1; i++)
            {
                var line = string.Empty;
                Recursive(line, term, words);
            }
        }

        private void Recursive(string line, string term, List<string> words)
        {
            for (var i = 0; i <= words.Count - 1; i++)
            {
                if (Anagrams.Any(x => x == words[i])) continue;

                var currentWordLetters = words[i].Trim().ToCharArray();
                var currentSearchContainsWord = currentWordLetters.All(letter => term.Contains(letter));

                if (currentSearchContainsWord)
                {
                    var newLine = line + " " + words[i];

                    if (Anagrams.Any(x => x == newLine)) continue;

                    line = newLine;

                    foreach (var letter in currentWordLetters)
                    {
                        var regex = new Regex(Regex.Escape(letter.ToString()));
                        term = regex.Replace(term, "", 1);
                    }
                }

                if (string.IsNullOrEmpty(term))
                {
                    Anagrams.Add(line);
                    return;
                }
            }

            // if (!string.IsNullOrEmpty(term))
            // {
            // var newWords = words.Where(x => !Anagrams.Any(y => y == x)).ToList();
            //     Recursive(term, words);
            // };
        }

        private IList<string> FilterWords(string term, IEnumerable<string> words)
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



// using System;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Logging;
// using Anagram.Models;
// using System.IO;
// using System.Text.RegularExpressions;

// namespace Anagram.Controllers
// {
//     public class HomeController : Controller
//     {
//         [BindProperty]
//         public AnagramViewModel AnagramViewModel { get; set; }

//         private readonly ILogger<HomeController> _logger;

//         public HomeController(ILogger<HomeController> logger)
//         {
//             _logger = logger;
//         }

//         public IActionResult Index()
//         {
//             return View();
//         }

//         public async Task<IActionResult> Search(string search)
//         {
//             search = search.ToUpper();

//             if (string.IsNullOrEmpty(search)) return RedirectToAction("Error", new { errorTitle = "Invalid input", errorMessage = "Cannot find anagrams with empty strings" });
//             if (CheckForInvalidInputs(search)) return RedirectToAction("Error", new { errorTitle = "Invalid input", errorMessage = "Input only accepts letters and white spaces" });

//             var words = System.IO.File.ReadAllLines(@"Infrastructure\words.txt");

//             await Task.Run(() =>
//              {
//                  Parallel.ForEach(words, (word, state) =>
//                  {
//                      if (search == word) return;

//                      var currentWord = word;
//                      //  var isAnagram = true;
//                      var letters = search.ToCharArray();

//                      char[] char1 = currentWord.ToLower().ToCharArray();
//                      char[] char2 = search.ToLower().ToCharArray();

//                      Array.Sort(char1);
//                      Array.Sort(char2);

//                      string NewWord1 = new string(char1);
//                      string NewWord2 = new string(char2);

//                      //  foreach (var letter in letters)
//                      //  {

//                      //  var regex = new Regex(Regex.Escape(letter.ToString()));
//                      //  currentWord = regex.Replace(currentWord, "", 1);

//                      //  if(string.IsNullOrEmpty(currentWord)) isAnagram = true;
//                      //  }

//                      if (NewWord1 == NewWord2)
//                          Console.WriteLine("Yes! Words \"{0}\" and \"{1}\" are Anagrams", currentWord, search);

//                      //  if (isAnagram) Console.WriteLine(word);
//                  });
//              });

//             return Ok();
//         }

//         private bool CheckForInvalidInputs(string input)
//         {
//             return Regex.Match(input, "[^a-zA-Z ]+").Success;
//         }

//         [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
//         public IActionResult Error(string errorTitle, string errorMessage)
//         {
//             return View(new ErrorViewModel
//             {
//                 ErrorTitle = errorTitle,
//                 ErrorMessage = errorMessage
//             });
//         }
//     }
// }
