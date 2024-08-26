using GroupSpace23.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace GroupSpace23.Models
{
    public class Mand
    {
        [Display(Name = "Quantities")]
        public int Quantities { get; set; }

        public int Id { get; set; }

        [Required(ErrorMessage = "Deze veld is verplicht")]
        [Display(Name = "Title")]
        public string Title { get; set; }


        [Required(ErrorMessage = "Deze veld is verplicht")]
        
        [Display(Name = "Boodschap")]
        public string Body { get; set; }

        [Required(ErrorMessage = "Deze veld is verplicht")]
       
        [Display(Name = "Verzonden")]
        public DateTime Sent { get; set; } = DateTime.Now;




        
        [ForeignKey("GroupSpace23User")]
        [Display(Name = "Verzonden door")]
        public string SenderId { get; set; } = Globals.DummyUser.Id;
        public GroupSpace23User? Sender { get; set; } = Globals.DummyUser;

        public DateTime Deleted { get; set; } = DateTime.MaxValue;
       
        [ForeignKey("Evenement")]
        [Display(Name = "Ontvanger")]
        public int RecipientId { get; set; }
       
        [Display(Name = "Ontvanger")]
        public Evenement? Recipient { get; set; }
        
        [NotMapped]
        [Display(Name = "Geselecteerde items")]
        public string SelectedItems { get; set; }
    }

    public class MandIndexViewModel
    {
        public List<Mand> Mands { get; set; }
        public string SelectMode { get; set; } = "R";
        public SelectList Modes { get; set; }
    }

    public class ModeItem
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }
}
