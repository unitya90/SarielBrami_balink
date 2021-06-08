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
    public class MembershipController : Controller
    {
        // Create/Retrieve a Membership

        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "9HrhlyZKl2JrVfP8rF0hGufjL89EYscFjycKTCnD",
            BasePath = "https://backendexercise-c5925-default-rtdb.firebaseio.com/"
        };

        IFirebaseClient client;

        // Create a Membership
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Membership membership)
        {
            if (CheckRelationship(membership))
                ModelState.AddModelError(string.Empty, "The animal with the ID : " + membership.animal_id + " already have a relationship!");
            
            else
            {
                try
                {
                    AddMembershipToFirebase(membership);
                    ModelState.AddModelError(string.Empty, "The Membership " + membership.membership_id + " was added Successfully!");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View();
        }
        
        bool CheckRelationship(Membership membership)
        {
            client = new FireSharp.FirebaseClient(config);

            FirebaseResponse response = client.Get("Membership");

            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            var list = new List<Membership>();

            if (data != null)
            {
                foreach (var m in data)
                {
                    list.Add(JsonConvert.DeserializeObject<Membership>(((JProperty)m).Value.ToString()));

                    if (JsonConvert.DeserializeObject<Membership>(((JProperty)m).Value.ToString()).animal_id == membership.animal_id)
                        return true;
                }
            }
            
            return false;
        }

        private void AddMembershipToFirebase(Membership membership)
        {
            client = new FireSharp.FirebaseClient(config);

            var data = membership;

            PushResponse response = client.Push("Membership/", data);

            data.membership_id = response.Result.name;

            SetResponse setResponse = client.Set("Membership/" + data.membership_id, data);
        }

        // Retrive a Membership
        [HttpGet]
        public ActionResult Retrive(string id)
        {
            client = new FireSharp.FirebaseClient(config);

            FirebaseResponse response = client.Get("Membership/" + id);

            Membership data = JsonConvert.DeserializeObject<Membership>(response.Body);

            return View(data);
        }
        
        // Retrive all the Memberships of a specific person
        [HttpGet]
        public ActionResult MembershipsOfPerson(string id)
        {
            client = new FireSharp.FirebaseClient(config);

            FirebaseResponse response = client.Get("Membership");

            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            var list = new List<Membership>();
            var newList = new List<Membership>();

            if (data != null)
                foreach (var m in data)
                {
                    list.Add(JsonConvert.DeserializeObject<Membership>(((JProperty)m).Value.ToString()));

                    if (JsonConvert.DeserializeObject<Membership>(((JProperty)m).Value.ToString()).person_id == id)
                        newList.Add(JsonConvert.DeserializeObject<Membership>(((JProperty)m).Value.ToString()));
                }
            
            return View(newList);
        }

        // Retrive all the Memberships of all persons
        public ActionResult Index()
        {
            client = new FireSharp.FirebaseClient(config);

            FirebaseResponse response = client.Get("Membership");

            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            var list = new List<Membership>();

            if (data != null)
                foreach (var m in data)
                {
                    list.Add(JsonConvert.DeserializeObject<Membership>(((JProperty)m).Value.ToString()));
                }

            return View(list);
        }
    }
}