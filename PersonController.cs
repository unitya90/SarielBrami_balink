using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FireSharp.Interfaces;
using FireSharp.Config;
using FireSharp.Response;
using BackendExercise.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BackendExercise.Controllers
{
    public class PersonController : Controller
    {
        // Create/Delete/Retrieve a Person

        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "9HrhlyZKl2JrVfP8rF0hGufjL89EYscFjycKTCnD",
            BasePath = "https://backendexercise-c5925-default-rtdb.firebaseio.com/"
        };

        IFirebaseClient client;

        // Create a Person
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Person person)
        {
            try
            {
                AddPersonToFirebase(person);
                ModelState.AddModelError(string.Empty, "The Person " + person.firstName + " " +  person.lastName + " was added Successfully!");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View();
        }

        private void AddPersonToFirebase(Person person)
        {
            client = new FireSharp.FirebaseClient(config);

            var data = person;

            PushResponse response = client.Push("Persons/", data);

            data.person_id = response.Result.name;

            SetResponse setResponse = client.Set("Persons/" + data.person_id, data);
        }

        // Delete a Person
        [HttpGet]
        public ActionResult Delete(string id)
        {
            client = new FireSharp.FirebaseClient(config);

            FirebaseResponse response = client.Delete("Persons/" + id);

            return RedirectToAction("Index");
        }

        // Retrive a Person
        [HttpGet]
        public ActionResult Retrive(string id)
        {
            client = new FireSharp.FirebaseClient(config);

            FirebaseResponse response = client.Get("Persons/"+id);

            Person data = JsonConvert.DeserializeObject<Person>(response.Body);
            
            return View(data);
        }

        // Retrive all the Persons
        public ActionResult Index()
        {
            client = new FireSharp.FirebaseClient(config);

            FirebaseResponse response = client.Get("Persons");

            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            var list = new List<Person>();

            if (data != null)
                foreach (var p in data)
                {
                    list.Add(JsonConvert.DeserializeObject<Person>(((JProperty)p).Value.ToString()));
                }

            return View(list);
        }
    }
}