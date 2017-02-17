using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace aNewBlog.Models
{
    public class Contact
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Message { get; set; }
        public string Phone { get; set; }
        [Required]
        public DateTime Created{ get; set; }
    }
}