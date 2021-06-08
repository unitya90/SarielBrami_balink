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
    public class AnimalController : Controller
    {
        // Create/Update/Retrieve an Animal

        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "9HrhlyZKl2JrVfP8rF0hGufjL89EYscFjycKTCnD",
            BasePath = "https://backendexercise-c5925-default-rtdb.firebaseio.com/"
        };

        IFirebaseClient client;

        // Create an Animal
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Animal animal)
        {
            try
            {
                AddAnimalToFirebase(animal);
                ModelState.AddModelError(string.Empty, "The animal " + animal.name + " was added Successfully!");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View();
        }

        private void AddAnimalToFirebase(Animal animal)
        {
            client = new FireSharp.FirebaseClient(config);

            var data = animal;

            PushResponse response = client.Push("Animals/", data);

            data.animal_id = response.Result.name;

            SetResponse setResponse = client.Set("Animals/" + data.animal_id, data);
        }

        // Update an Animal
        [HttpGet]
        public ActionResult Update(string id)
        {
            client = new FireSharp.FirebaseClient(config);

            FirebaseResponse response = client.Get("Animals/" + id);

            Animal data = JsonConvert.DeserializeObject<Animal>(response.Body);

            return View(data);
        }

        [HttpPost]
        public ActionResult Update(Animal animal)
        {
            client = new FireSharp.FirebaseClient(config);

            SetResponse response = client.Set("Animals/" + animal.animal_id, animal);

            return RedirectToAction("Index");
        }

        // Retrive an Animal
        [HttpGet]
        public ActionResult Retrive(string id)
        {
            client = new FireSharp.FirebaseClient(config);

            FirebaseResponse response = client.Get("Animals/" + id);

            Animal data = JsonConvert.DeserializeObject<Animal>(response.Body);

            return View(data);
        }

        // Retrive all the Animals
        public ActionResult Index()
        {
            client = new FireSharp.FirebaseClient(config);

            FirebaseResponse response = client.Get("Animals");

            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            var list = new List<Animal>();

            if (data != null)
                foreach (var a in data)
                {
                    list.Add(JsonConvert.DeserializeObject<Animal>(((JProperty)a).Value.ToString()));
                }

            return View(list);
        }
    }
}