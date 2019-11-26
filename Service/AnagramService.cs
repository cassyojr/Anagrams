using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anagrams.Interfaces;

namespace Anagrams.Service
{
    public class AnagramService : IAnagramService
    {
        const string FileUrl = @"Infrastructure\words.txt";

        public async Task<List<string>> SearchForAnagrams(string search)
        {
            var words = System.IO.File.ReadAllLines(FileUrl);
            var anagramsList = new List<string>();

            await Task.Run(() =>
            {
                var filteredWordsList = FindWordsThatContainsSearchLetters(search, words);
                anagramsList = FindAnagrams(search, filteredWordsList);
            });

            return anagramsList;
        }

        private List<string> FindWordsThatContainsSearchLetters(string term, IEnumerable<string> words)
        {
            var wordsList = new List<string>();

            foreach (var word in words)
            {
                var currentTermContainsWord = IsCurrentSearchContainsWord(term, word);
                if (currentTermContainsWord && term != word) wordsList.Add(word);
            }

            return wordsList;
        }

        private List<string> FindAnagrams(string term, List<string> words)
        {
            var anagramsList = new List<string>();

            foreach (var word in words)
                RecursiveCheckForAnagrams(term, word, words, anagramsList);

            return anagramsList;
        }

        private void RecursiveCheckForAnagrams(string term, string word, List<string> words, List<string> anagramsList)
        {
            var sortedTerm = SortStringCharactersAscending(term);
            var termLength = term.Length;

            foreach (var wordToConcat in words)
            {
                if (word == wordToConcat) continue;

                var currentSearchContainsWord = IsCurrentSearchContainsWord(term, wordToConcat);
                var newAnagram = (word + " " + wordToConcat);
                var newAnagramLengthMatches = newAnagram.Replace(" ", string.Empty).Length == termLength;
                var newAnagramLengthInRange = newAnagram.Replace(" ", string.Empty).Length < termLength;

                if (newAnagramLengthInRange) RecursiveCheckForAnagrams(term, newAnagram, words, anagramsList);
                else if (newAnagramLengthMatches)
                {
                    var sortedNewTerm = SortStringCharactersAscending(newAnagram);

                    if (sortedNewTerm != sortedTerm) continue;

                    var existentTerm = IsExistingAnagram(anagramsList, newAnagram);

                    if (!existentTerm) anagramsList.Add(newAnagram);
                }
            }
        }

        private bool IsCurrentSearchContainsWord(string term, string word)
        {
            var currentWordLetters = word.Trim().ToCharArray();
            return currentWordLetters.All(letter => term.Contains(letter));
        }

        private bool IsExistingAnagram(List<string> anagramsList, string newAnagram)
        {
            return anagramsList.Any(anagram =>
            {
                var splitedExistingAnagram = anagram.Split(" ");
                Array.Sort(splitedExistingAnagram);

                var splitedNewAnagram = newAnagram.Split(" ");
                Array.Sort(splitedNewAnagram);

                return splitedExistingAnagram.SequenceEqual(splitedNewAnagram);
            });
        }

        private string SortStringCharactersAscending(string arg)
        {
            var convertedTerm = arg.Replace(" ", string.Empty).ToCharArray();
            Array.Sort(convertedTerm);
            return new string(convertedTerm).ToUpper();
        }
    }
}