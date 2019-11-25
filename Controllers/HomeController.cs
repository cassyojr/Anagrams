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

            var words = System.IO.File.ReadAllLines(@"Infrastructure\words.txt");
            var anagramsGlobalList = new List<Anagrams>();

            await Task.Run(() =>
             {
                 var anagrams = new Anagrams();
                 var currentWords = words;
                 var currentSearchWord = search;

                 foreach (var word in words)
                 {
                     if (string.IsNullOrEmpty(currentSearchWord)) break;

                     var currentWordLetters = word.ToCharArray();
                     var currentSearchContainsWord = currentWordLetters.All(letter => currentSearchWord.Contains(letter));

                     if (currentSearchContainsWord)
                     {
                         foreach (var letter in currentWordLetters)
                         {
                             var regex = new Regex(Regex.Escape(letter.ToString()));
                             currentSearchWord = regex.Replace(currentSearchWord, "", 1);
                         }

                         anagrams.AnagramsList.Add(word);
                     }
                 }
             });

            return Ok();
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
