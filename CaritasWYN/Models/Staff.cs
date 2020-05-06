﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CaritasWYN.Models
{
    public class Staff
    {
        [Key]
        public int StaffId { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public int? JobDutyId { get; set; }
        [Display(Name = "Job Duty")]
        public JobDuty JobDuty { get; set; }
        public ICollection<DailyActivity> DailyActivities { get; set; }
        public ICollection<Group> Groups { get; set; }
        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return LastName + ", " + FirstName;
            }
        }
    }
}
