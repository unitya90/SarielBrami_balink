using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackendExercise.Models
{
    public class Person
    {
        public string person_id { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public string phoneNumber { get; set; }

        public string city { get; set; }

        public string country { get; set; }
    }
}