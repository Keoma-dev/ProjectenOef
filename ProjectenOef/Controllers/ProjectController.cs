using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectenOef.Database;
using ProjectenOef.Domain;
using ProjectenOef.Models;


namespace ProjectenOef.Controllers
{
    public class ProjectController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ProjectDbContext _projectDbContext;

        public ProjectController(IWebHostEnvironment hostingEnvironment, ProjectDbContext projectDbContext)
        {
            _hostingEnvironment = hostingEnvironment;
            _projectDbContext = projectDbContext;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Project> projectsFromDb = await _projectDbContext.Projects.ToListAsync();

            List<ProjectListViewModel> projects = new List<ProjectListViewModel>();

            foreach (Project project in projectsFromDb)
            {
                projects.Add(new ProjectListViewModel { Id = project.Id, Title = project.Title });
            }

            return View(projects);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ProjectCreateViewModel create = new ProjectCreateViewModel();

            var projectStatuses = await _projectDbContext.ProjectStatuses.ToListAsync();

            foreach (ProjectStatus projectStatus in projectStatuses)
            {
                create.ProjectStatuses.Add(new SelectListItem()
                {
                    Value = projectStatus.Id.ToString(),
                    Text = projectStatus.Status
                });
            }

            var tags = await _projectDbContext.Tags.ToListAsync();
            create.Tags = tags.Select(tag => new SelectListItem() { Value = tag.Id.ToString(), Text = tag.Name }).ToList();

            return View(create);
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProjectCreateViewModel createProject)
        {
            Project newProject = new Project()
            {
                Title = createProject.Title,
                Description = createProject.Description,
                ProjectStatusId = createProject.SelectedProjectStatus,
                ProjectTags = createProject.SelectedTags.Select(tag => new ProjectTag() { TagId = tag}).ToList()

            };

            _projectDbContext.Projects.Add(newProject);
            await _projectDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detail(int id)
        {
           Project projectFromDb = await _projectDbContext.Projects
                .Include(project =>project.ProjectStatus)
                .Include(project => project.ProjectTags)
                .ThenInclude(projectTag => projectTag.Tag)
                .FirstOrDefaultAsync(p => p.Id == id);

            ProjectDetailViewModel detailView = new ProjectDetailViewModel()
            {
                Title = projectFromDb.Title,
                Description = projectFromDb.Description,
                ProjectStatus = projectFromDb.ProjectStatus.Status,
                ProjectTags = projectFromDb.ProjectTags.Select(projectTag => projectTag.Tag.Name).ToList()
            };

            return View(detailView);
        }
    }
}
