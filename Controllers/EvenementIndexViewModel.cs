using GroupSpace23.Models;
using System.Text.RegularExpressions;

namespace GroupSpace23.Controllers
{
    public class EvenementIndexViewModel
    {
        public IEnumerable<Evenement> Evenements { get; set; }
        public DateTime? StartDate { get; set; }
    }
}