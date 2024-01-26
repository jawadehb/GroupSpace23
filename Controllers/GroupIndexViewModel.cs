using System.Text.RegularExpressions;

namespace GroupSpace23.Controllers
{
    public class GroupIndexViewModel
    {
        public IEnumerable<Group> Groups { get; set; }
        public DateTime? StartDate { get; set; }
    }
}