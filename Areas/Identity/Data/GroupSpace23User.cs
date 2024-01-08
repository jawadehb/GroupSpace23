using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GroupSpace23.Models;
using Microsoft.AspNetCore.Identity;

namespace GroupSpace23.Areas.Identity.Data;

// Add profile data for application users by adding properties to the GroupSpace23User class
public class GroupSpace23User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

}

