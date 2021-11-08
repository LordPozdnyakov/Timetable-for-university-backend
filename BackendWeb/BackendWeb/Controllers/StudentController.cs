using BackendWeb.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BackendWeb.Controllers
{
    public class StudentController : Controller
    {
        public List<Student> GetAllStudent()
        {
            List<Student> student = new List<Student>();
            XDocument doc = XDocument.Load("data.xml");
            foreach(XElement element in doc.Descendants("university").Descendants("user"))
            {
                Student student1 = new Student();
                student1.Id_student =int.Parse(element.Element("id").Value);
                student1.Surname = element.Element("surname").Value;
                student1.Name = element.Element("name").Value;
                student1.Patronymic = element.Element("patronymic").Value;
                student1.Date_birth = DateTime.Parse(element.Element("date_birth").Value);
                student1.Place_residence = element.Element("place_residence").Value;
                student1.Telephone = element.Element("surname").Value;
                student1.Email = element.Element("email").Value;
                student1.FullName_father = element.Element("fullname_father").Value;
                student1.Father_telephone = element.Element("father_tel").Value;
                student1.FullName_mother = element.Element("fullname_mother").Value;
                student1.Mother_telephone = element.Element("mother_tel").Value;
                student.Add(student1);
            }
            return student;

        }
        private DatasContext _context;
        public StudentController(DatasContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("/students")]
        public ActionResult<List<Student>> GetAction([FromServices] DatasContext context)
        {
            return GetAllStudent();
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
