using System.Collections.Generic;
using System.Threading.Tasks;

namespace Anagrams.Interfaces
{
    public interface IAnagramService
    {
        Task<List<string>> SearchForAnagrams(string search);
    }
}