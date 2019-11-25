using System.Collections.Generic;

namespace Anagram.Models
{
    public class AnagramViewModel
    {
        public string Search { get; set; }

        public List<string> Anagrams { get; set; }

        public string RequestTime { get; set; }
    }
}
