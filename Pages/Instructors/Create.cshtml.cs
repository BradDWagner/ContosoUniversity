using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;

namespace ContosoUniversity.Pages.Instructors
{
    public class CreateModel : InstructorCoursesPageModel
    {
        private readonly ContosoUniversity.Data.SchoolContext _context;
        private readonly ILogger<InstructorCoursesPageModel> _logger;

        public CreateModel(ContosoUniversity.Data.SchoolContext context, ILogger<InstructorCoursesPageModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            var instructor = new Instructor();
            instructor.Courses = new List<Course>();

            //Provides an empty collection for the foreach loop
            //foreach (var course in Model.AssignedCourseDataList)
            // in the Create page.
            PopulateAssignedCourseData(_context, instructor);
            return Page();
        }

        [BindProperty]
        public Instructor Instructor { get; set; }
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync(string[] selectedCourses)
        {
          var newInstructor = new Instructor();

          if (selectedCourses.Length > 0)
          {
            newInstructor.Courses = new List<Course>();
            // Load collection with one DB call.
            _context.Courses.Load();
          }

          //Add selected Courses courses to the new instructor.
          foreach (var course in selectedCourses)
          {
            var foundCourse = await _context.Courses.FindAsync(int.Parse(course));
            if (foundCourse != null)
            {
                newInstructor.Courses.Add(foundCourse);
            }
            else
            {
                _logger.LogWarning("course {course} not found", course);
            }
          }

          try 
          {
            if (await TryUpdateModelAsync<Instructor>(
                                newInstructor,
                                "Instructor",
                                i => i.FirstMidName, i => i.LastName,
                                i => i.HireDate, i => i.OfficeAssignment))
            {
                _context.Instructors.Add(newInstructor);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            return RedirectToPage("./Index");
          }
          catch (Exception ex)
          {
            _logger.LogError(ex.Message);
          }

          PopulateAssignedCourseData(_context, newInstructor);
          return Page();
        }
    }
}
