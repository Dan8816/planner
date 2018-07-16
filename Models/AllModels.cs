using System;
using System.Collections.Generic;//allows creating of list objects
using System.ComponentModel.DataAnnotations;//dependency for validations compared to models
using System.ComponentModel.DataAnnotations.Schema;//dependency for validations compared to db schema


namespace Planner.Models
{
    public class User//these names must match the schema names
    {
        [Key]
        public int Id { get; set; }
        public string first { get; set; }
        public string last { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public List<Wedding> weddings { get; set; }//this instantiates a list of Wedding objects from the weddings table connected to a User object for M2M
        public List<RSVP> rsvps { get; set; }//this instantiates a list of RSVP objects from the rsvps table connected to a User object for M2M
        public User()
        {
            created_at = DateTime.Now;
            updated_at = DateTime.Now;
            weddings = new List<Wedding>();
            rsvps = new List<RSVP>();
        }
    }
    public class Wedding
    {
        [Key]
        public int Id { get; set; }
        public string WedderOne { get; set; }
        public string WedderTwo { get; set; }
        public DateTime WeddingDate { get; set; }
        public string WeddingStrAdd { get; set; }
        public DateTime created_at { get; set;}
        public DateTime updated_at { get; set;}
        public int CreatorId { get; set; }
        public User Creator { get; set; }
        public List<RSVP> rsvps { get; set; }
        public Wedding()
        {
            created_at = DateTime.Now;
            updated_at = DateTime.Now;
            rsvps = new List<RSVP>();
        }
    }
    public class RSVP
    {
        [Key]
        public int Id { get; set; }
        public int WeddingId { get; set; }
        public Wedding weddings { get; set; }
        public int RSVPguestId { get; set; }
        public User RSVPguest { get; set; }
    }
}