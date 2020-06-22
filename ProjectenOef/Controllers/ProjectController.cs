using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.Language;
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
                ProjectTags = createProject.SelectedTags.Select(tag => new ProjectTag() { TagId = tag }).ToList()

            };

            if (createProject.Photo !=null)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(createProject.Photo.FileName);
                var pathName = Path.Combine(_hostingEnvironment.WebRootPath, "pics");
                var fileNameWithPath = Path.Combine(pathName, uniqueFileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    createProject.Photo.CopyTo(stream);
                }

                newProject.PhotoUrl = "/pics/" + uniqueFileName;
            }

            _projectDbContext.Projects.Add(newProject);
            await _projectDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detail(int id)
        {
            Project projectFromDb = await _projectDbContext.Projects
                 .Include(project => project.ProjectStatus)
                 .Include(project => project.ProjectTags)
                 .ThenInclude(projectTag => projectTag.Tag)
                 .FirstOrDefaultAsync(p => p.Id == id);

            ProjectDetailViewModel detailView = new ProjectDetailViewModel()
            {
                Id = id,
                Title = projectFromDb.Title,
                Description = projectFromDb.Description,
                ProjectStatus = projectFromDb.ProjectStatus.Status,
                ProjectTags = projectFromDb.ProjectTags.Select(projectTag => projectTag.Tag.Name).ToList(),
                PhotoUrl = projectFromDb.PhotoUrl
            };

            return View(detailView);
        }

        public async Task<IActionResult> Edit(int id)
        {
            Project projectFromDb = await _projectDbContext.Projects
                .Include(project => project.ProjectTags)
                .FirstOrDefaultAsync(p => p.Id == id);

            ProjectEditViewModel editView = new ProjectEditViewModel()
            {
                Title = projectFromDb.Title,
                Description = projectFromDb.Description,
                SelectedTags = projectFromDb.ProjectTags.Select(pt => pt.TagId).ToArray()
            };

            var projectStatuses = await _projectDbContext.ProjectStatuses.ToListAsync();
            editView.ProjectStatuses = projectStatuses.Select(projectStatus => new SelectListItem() { Value = projectStatus.Id.ToString(), Text = projectStatus.Status }).ToList();

            var tags = await _projectDbContext.Tags.ToListAsync();
            editView.Tags = tags.Select(tag => new SelectListItem() { Value = tag.Id.ToString(), Text = tag.Name }).ToList();

            return View(editView);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProjectEditViewModel editView, int id)
        {
            Project projectFromDb = await _projectDbContext.Projects
               .Include(project => project.ProjectStatus)
               .Include(project => project.ProjectTags)
               .FirstOrDefaultAsync(p => p.Id == id);

            var projectStatuses = await _projectDbContext.ProjectStatuses.ToListAsync();

            _projectDbContext.ProjectTags.RemoveRange(projectFromDb.ProjectTags);

            projectFromDb.Title = editView.Title;
            projectFromDb.Description = editView.Description;
            projectFromDb.ProjectStatusId = editView.SelectedProjectStatus;
            projectFromDb.ProjectTags = editView.SelectedTags.Select(tagId => new ProjectTag() { TagId = tagId }).ToList();

            if (editView.Photo != null)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(editView.Photo.FileName);
                var pathName = Path.Combine(_hostingEnvironment.WebRootPath, "pics");
                var fileNameWithPath = Path.Combine(pathName, uniqueFileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    editView.Photo.CopyTo(stream);
                }

                projectFromDb.PhotoUrl = "/pics/" + uniqueFileName;
            }

            _projectDbContext.Update(projectFromDb);

            await _projectDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            Project projectFromDb = await _projectDbContext.Projects.FirstOrDefaultAsync(p => p.Id == id);
            ProjectDeleteViewModel deleteView = new ProjectDeleteViewModel()
            {
                Id = projectFromDb.Id,
                Title = projectFromDb.Title
            };

            return View(deleteView);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            Project projectFromDb = await _projectDbContext.Projects.FirstOrDefaultAsync(p => p.Id == id);

            _projectDbContext.Projects.Remove(projectFromDb);
            await _projectDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
