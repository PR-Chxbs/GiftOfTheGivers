using GiftOfTheGivers.Controllers;
using GiftOfTheGivers.Data;
using GiftOfTheGivers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GiftOfTheGivers.Tests
{
    public class AdminControllerUserTests
    {
        private ApplicationDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public void Index_ReturnsView()
        {
            var context = GetDbContext("AdminIndexDb");
            var controller = new AdminController(context);

            var result = controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        // ======== Users Tests ========
        [Fact]
        public async Task Users_ReturnsAllUsers()
        {
            var context = GetDbContext("AdminUsersDb");
            context.Users.Add(new ApplicationUser { Id = "1", Email = "a@test.com", Name = "User A", Role = "Admin" });
            context.Users.Add(new ApplicationUser { Id = "2", Email = "b@test.com", Name = "User B", Role = "Admin" });
            await context.SaveChangesAsync();

            var controller = new AdminController(context);
            var result = await controller.Users();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<ApplicationUser>>(viewResult.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public void CreateUser_GET_ReturnsViewWithRoles()
        {
            var context = GetDbContext("CreateUserGETDb");
            var controller = new AdminController(context);

            var result = controller.CreateUser();

            var viewResult = Assert.IsType<ViewResult>(result);
            var roles = Assert.IsAssignableFrom<List<string>>(viewResult.ViewData["Roles"]);
            Assert.Contains("Admin", roles);
            Assert.Contains("User", roles);
        }

        [Fact]
        public async Task CreateUser_POST_ValidModel_AddsUserAndRedirects()
        {
            var context = GetDbContext("CreateUserPOSTValidDb");
            var controller = new AdminController(context);
            var user = new ApplicationUser { Id = "1", Name = "New User", Email = "new@test.com", Role = "User" };

            var result = await controller.CreateUser(user);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Users", redirect.ActionName);
            Assert.Single(context.Users);
        }

        [Fact]
        public async Task CreateUser_POST_InvalidModel_ReturnsView()
        {
            var context = GetDbContext("CreateUserPOSTInvalidDb");
            var controller = new AdminController(context);
            controller.ModelState.AddModelError("Name", "Required");
            var user = new ApplicationUser { Id = "1", Email = "fail@test.com" };

            var result = await controller.CreateUser(user);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ApplicationUser>(viewResult.Model);
            Assert.Empty(context.Users);
        }

        [Fact]
        public async Task EditUser_GET_ReturnsViewWithUser()
        {
            var context = GetDbContext("EditUserGETDb");
            context.Users.Add(new ApplicationUser { Id = "1", Name = "User One", Email = "one@test.com", Role = "User" });
            await context.SaveChangesAsync();

            var controller = new AdminController(context);
            var result = await controller.EditUser("1");

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ApplicationUser>(viewResult.Model);
            Assert.Equal("User One", model.Name);

            var roles = Assert.IsAssignableFrom<List<string>>(viewResult.ViewData["Roles"]);
            Assert.Contains("Admin", roles);
            Assert.Contains("User", roles);
        }

        [Fact]
        public async Task EditUser_GET_InvalidId_ReturnsNotFound()
        {
            var context = GetDbContext("EditUserGETInvalidDb");
            var controller = new AdminController(context);

            var result = await controller.EditUser("999");

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task EditUser_POST_ValidModel_UpdatesUserAndRedirects()
        {
            var context = GetDbContext("EditUserPOSTValidDb");
            var user = new ApplicationUser { Id = "1", Name = "Old Name", Email = "edit@test.com", Role = "User" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var controller = new AdminController(context);
            user.Name = "Updated Name";

            var result = await controller.EditUser("1", user);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Users", redirect.ActionName);
            Assert.Equal("Updated Name", context.Users.First().Name);
        }

        [Fact]
        public async Task EditUser_POST_IdMismatch_ReturnsNotFound()
        {
            var context = GetDbContext("EditUserPOSTIdMismatchDb");
            var controller = new AdminController(context);
            var user = new ApplicationUser { Id = "1", Name = "Mismatch" };

            var result = await controller.EditUser("999", user);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteUser_ValidId_RemovesUserAndRedirects()
        {
            var context = GetDbContext("DeleteUserValidDb");
            var user = new ApplicationUser { Id = "1", Name = "ToDelete", Email = "del@test.com", Role = "User" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var controller = new AdminController(context);
            var result = await controller.DeleteUser("1");

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Users", redirect.ActionName);
            Assert.Empty(context.Users);
        }

        [Fact]
        public async Task DeleteUser_InvalidId_ReturnsNotFound()
        {
            var context = GetDbContext("DeleteUserInvalidDb");
            var controller = new AdminController(context);

            var result = await controller.DeleteUser("999");

            Assert.IsType<NotFoundResult>(result);
        }

        // -------- Events Tests --------
        [Fact] 
        public async Task Events_ReturnsAllEvents() { 
            var context = GetDbContext("EventsListDb");
            context.Events.Add(new Event { Name = "Event 1", Description = "Description 1", Location = "Location 1" });
            context.Events.Add(new Event { Name = "Event 2", Description = "Description 2", Location = "Location 2" });

            await context.SaveChangesAsync(); 
            var controller = new AdminController(context); 
            var result = await controller.Events(); 
            var viewResult = Assert.IsType<ViewResult>(result); 
            var model = Assert.IsAssignableFrom<List<Event>>(viewResult.Model);

            Assert.Equal(2, model.Count); 
        }

        [Fact] 
        public void CreateEvent_GET_ReturnsView() { 
            var context = GetDbContext("CreateEventGETDb"); 
            var controller = new AdminController(context); 
            var result = controller.CreateEvent(); 
            Assert.IsType<ViewResult>(result); 
        }

        [Fact] 
        public async Task CreateEvent_POST_ValidModel_AddsEvent() { 
            var context = GetDbContext("CreateEventPOSTDb"); 
            var controller = new AdminController(context);

            var ev = new Event { Name = "Event 1", Description = "Description 1", Location = "Location 1" };

            var result = await controller.CreateEvent(ev); 
            
            var redirect = Assert.IsType<RedirectToActionResult>(result); 
            Assert.Equal("Events", redirect.ActionName); 
            Assert.Single(context.Events); 
        }
        
        [Fact] 
        public async Task CreateEvent_POST_InvalidModel_ReturnsView() { 
            var context = GetDbContext("CreateEventPOSTInvalidDb"); 
            var controller = new AdminController(context); 
            controller.ModelState.AddModelError("Name", "Required"); 
            var ev = new Event(); 
            
            var result = await controller.CreateEvent(ev); 
            
            var viewResult = Assert.IsType<ViewResult>(result); 
            var model = Assert.IsType<Event>(viewResult.Model); 
            Assert.Empty(context.Events); 
        }

        [Fact] 
        public async Task EditEvent_GET_ReturnsEventView() { 
            var context = GetDbContext("EditEventGETDb");
            var ev = new Event { Name = "Edit Event", Description = "Description 1", Location = "Location 1" };
            context.Events.Add(ev);

            await context.SaveChangesAsync(); 

            var controller = new AdminController(context); 
            
            var result = await controller.EditEvent(1); 
            
            var viewResult = Assert.IsType<ViewResult>(result); 
            var model = Assert.IsType<Event>(viewResult.Model); 
            Assert.Equal("Edit Event", model.Name); 
        }
        
        [Fact] 
        public async Task EditEvent_GET_InvalidId_ReturnsNotFound() { 
            var context = GetDbContext("EditEventGETInvalidDb"); 
            var controller = new AdminController(context); 
            
            var result = await controller.EditEvent(999); 
            Assert.IsType<NotFoundResult>(result); 
        }
        
        [Fact] 
        public async Task EditEvent_POST_ValidModel_UpdatesEvent() { 
            var context = GetDbContext("EditEventPOSTDb");
            var ev = new Event { EventID = 1, Name = "Old Name", Description = "Description 1", Location = "Location 1" };
            context.Events.Add(ev); 
            
            await context.SaveChangesAsync(); 
            
            var controller = new AdminController(context); 
            ev.Name = "Updated Name"; 
            
            var result = await controller.EditEvent(1, ev); 
            
            var redirect = Assert.IsType<RedirectToActionResult>(result); 
            Assert.Equal("Events", redirect.ActionName); 
            Assert.Equal("Updated Name", context.Events.First().Name); 
        }
        
        [Fact] 
        public async Task EditEvent_POST_IdMismatch_ReturnsNotFound() { 
            var context = GetDbContext("EditEventPOSTMismatchDb"); 
            var controller = new AdminController(context);
            var ev = new Event { EventID = 1, Name = "Mismatch", Description = "Description 1", Location = "Location 1" };

            var result = await controller.EditEvent(999, ev); 
            
            Assert.IsType<NotFoundResult>(result); 
        }
        
        [Fact] 
        public async Task DeleteEvent_ValidId_RemovesEvent() { 
            var context = GetDbContext("DeleteEventValidDb");
            var ev = new Event { EventID = 1, Name = "To Delete", Description = "Description 1", Location = "Location 1" };
            context.Events.Add(ev); 
            
            await context.SaveChangesAsync(); 
            
            var controller = new AdminController(context); 
            
            var result = await controller.DeleteEvent(1); 
        
            var redirect = Assert.IsType<RedirectToActionResult>(result); 
            Assert.Equal("Events", redirect.ActionName); 
            Assert.Empty(context.Events); 
        }
        
        [Fact] 
        public async Task DeleteEvent_InvalidId_ReturnsNotFound() { 
            var context = GetDbContext("DeleteEventInvalidDb"); 
            var controller = new AdminController(context); 
            
            var result = await controller.DeleteEvent(999); 
            
            Assert.IsType<NotFoundResult>(result); 
        
        } 
        
        // -------- Donations Tests --------
        [Fact] 
        public async Task Donations_ReturnsAllDonationsWithEvents() { 
            var context = GetDbContext("DonationsListDb");
            var ev = new Event { EventID = 1, Name = "Event 1", Description = "Description 1", Location = "Location 1" };
            context.Events.Add(ev); 
            context.Donations.Add(new Donation { DonationID = 1, DonorName = "Alice", EventID = 1, ItemDescription = "New donation 1" }); 
            context.Donations.Add(new Donation { DonationID = 2, DonorName = "Bob", EventID = 1, ItemDescription = "New donation 1" }); 
            
            await context.SaveChangesAsync(); 
            
            var controller = new AdminController(context); 
            
            var result = await controller.Donations(); 
            var viewResult = Assert.IsType<ViewResult>(result); 
            var model = Assert.IsAssignableFrom<List<Donation>>(viewResult.Model); 
            Assert.Equal(2, model.Count); 
        } 
        
        [Fact] 
        public void CreateDonation_GET_ReturnsViewWithEvents() { 
            var context = GetDbContext("CreateDonationGETDb");
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.Events.Add(new Event { Name = "Event 1", Description = "Description 1", Location = "Location 1" }); 
            context.SaveChanges(); 
            
            var controller = new AdminController(context); 
            var result = controller.CreateDonation(); 
            var viewResult = Assert.IsType<ViewResult>(result); 
            var events = Assert.IsAssignableFrom<List<Event>>(viewResult.ViewData["Events"]); 
            
            Assert.Single(events); 
        } 
        
        [Fact] 
        public async Task CreateDonation_POST_ValidModel_AddsDonation() { 
            var context = GetDbContext("CreateDonationPOSTDb"); 
            var ev = new Event { Name = "Event Y", Description = "Description 1", Location = "Location 1" }; 
            context.Events.Add(ev); 
            
            await context.SaveChangesAsync(); 
            
            var donation = new Donation { DonationID = 1, DonorName = "Donor A", EventID = 1, ItemDescription = "Test Donation" }; 
            var controller = new AdminController(context); 
            
            var result = await controller.CreateDonation(donation); 
            
            var redirect = Assert.IsType<RedirectToActionResult>(result); 
            Assert.Equal("Donations", redirect.ActionName); 
            Assert.Single(context.Donations); 
        } 
        
        [Fact] 
        public async Task CreateDonation_POST_InvalidModel_ReturnsView() { 
            var context = GetDbContext("CreateDonationPOSTInvalidDb"); 
            var controller = new AdminController(context); 
            controller.ModelState.AddModelError("DonorName", "Required"); 
            var donation = new Donation(); 
            
            var result = await controller.CreateDonation(donation); 
            
            var viewResult = Assert.IsType<ViewResult>(result); 
            var model = Assert.IsType<Donation>(viewResult.Model); 
            Assert.Empty(context.Donations); 
        }

        // -------- Resources Tests --------
        [Fact]
        public async Task Resources_ReturnsAllResourcesWithEvents()
        {
            var context = GetDbContext("ResourcesListDb");
            var ev = new Event { EventID = 1, Name = "Event A", Description = "Description 1", Location = "Location 1" };
            context.Events.Add(ev);
            context.Resources.Add(new Resource { ResourceID = 1, Name = "Water", EventID = 1, Unit = "" });
            context.Resources.Add(new Resource { ResourceID = 2, Name = "Food", EventID = 1, Unit = "" });
            await context.SaveChangesAsync();

            var controller = new AdminController(context);
            
            var result = await controller.Resources();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Resource>>(viewResult.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public void CreateResource_GET_ReturnsViewWithEvents()
        {
            var context = GetDbContext("CreateResourceGETDb");
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.Events.Add(new Event { Name = "Event X", Description = "Description 1", Location = "Location 1" });
            context.SaveChanges();

            var controller = new AdminController(context);
            var result = controller.CreateResource();

            var viewResult = Assert.IsType<ViewResult>(result);
            var events = Assert.IsAssignableFrom<List<Event>>(viewResult.ViewData["Events"]);
            Assert.Single(events);
        }

        [Fact]
        public async Task CreateResource_POST_ValidModel_AddsResource()
        {
            var context = GetDbContext("CreateResourcePOSTDb");
            context.Events.Add(new Event { EventID = 1, Name = "Event Y", Description = "Description 1", Location = "Location 1" });
            await context.SaveChangesAsync();

            var resource = new Resource { ResourceID = 1, Name = "Blankets", EventID = 1, Unit = "" };
            var controller = new AdminController(context);

            var result = await controller.CreateResource(resource);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Resources", redirect.ActionName);
            Assert.Single(context.Resources);
        }

        [Fact]
        public async Task CreateResource_POST_InvalidModel_ReturnsView()
        {
            var context = GetDbContext("CreateResourcePOSTInvalidDb");
            var controller = new AdminController(context);
            controller.ModelState.AddModelError("Name", "Required");
            var resource = new Resource();

            var result = await controller.CreateResource(resource);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Resource>(viewResult.Model);
            Assert.Empty(context.Resources);
        }

        [Fact]
        public async Task EditResource_GET_ReturnsResourceView()
        {
            var context = GetDbContext("EditResourceGETDb");
            var res = new Resource { ResourceID = 1, Name = "Water", Unit = "" };
            context.Resources.Add(res);
            await context.SaveChangesAsync();

            var controller = new AdminController(context);
            var result = await controller.EditResource(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Resource>(viewResult.Model);
            Assert.Equal("Water", model.Name);
        }

        [Fact]
        public async Task EditResource_GET_InvalidId_ReturnsNotFound()
        {
            var context = GetDbContext("EditResourceGETInvalidDb");
            var controller = new AdminController(context);

            var result = await controller.EditResource(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task EditResource_POST_ValidModel_UpdatesResource()
        {
            var context = GetDbContext("EditResourcePOSTDb");
            var res = new Resource { ResourceID = 1, Name = "Old Name", Unit = "" };
            context.Resources.Add(res);
            await context.SaveChangesAsync();

            var controller = new AdminController(context);
            res.Name = "Updated Name";

            var result = await controller.EditResource(res);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Resources", redirect.ActionName);
            Assert.Equal("Updated Name", context.Resources.First().Name);
        }

        [Fact]
        public async Task DeleteResource_ValidId_RemovesResource()
        {
            var context = GetDbContext("DeleteResourceValidDb");
            var res = new Resource { ResourceID = 1, Name = "ToDelete", Unit = "" };
            context.Resources.Add(res);
            await context.SaveChangesAsync();

            var controller = new AdminController(context);
            var result = await controller.DeleteResource(1);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Resources", redirect.ActionName);
            Assert.Empty(context.Resources);
        }

        [Fact]
        public async Task DeleteResource_InvalidId_ReturnsNotFound()
        {
            var context = GetDbContext("DeleteResourceInvalidDb");
            var controller = new AdminController(context);

            var result = await controller.DeleteResource(999);

            Assert.IsType<NotFoundResult>(result);
        }

        // -------- Assignments Tests --------

        [Fact]
        public async Task Assignments_ReturnsAllAssignmentsWithUsersAndEvents()
        {
            var context = GetDbContext("AssignmentsListDb");
            var ev = new Event { EventID = 1, Name = "Event A", Description = "Description 1", Location = "Location 1" };
            var user = new ApplicationUser { Id = "1", UserName = "Alice", Name = "Alice", Role = "User" };
            context.Events.Add(ev);
            context.Users.Add(user);
            context.Assignments.Add(new Assignment { AssignmentID = 1, EventID = 1, UserID = "1", RoleInProject = "Leader" });
            await context.SaveChangesAsync();

            var controller = new AdminController(context);
            var result = await controller.Assignments();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Assignment>>(viewResult.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task CreateAssignment_GET_ReturnsViewWithUsersAndEvents()
        {
            var context = GetDbContext("CreateAssignmentGETDb");
            context.Events.Add(new Event { EventID = 1, Name = "Event X", Description = "Description 1", Location = "Location 1" });
            context.Users.Add(new ApplicationUser { Id = "1", UserName = "Bob", Name = "Bob", Role = "User" });
            await context.SaveChangesAsync();

            var controller = new AdminController(context);
            var result = await controller.CreateAssignment();

            var viewResult = Assert.IsType<ViewResult>(result);
            var users = Assert.IsAssignableFrom<List<ApplicationUser>>(viewResult.ViewData["Users"]);
            var events = Assert.IsAssignableFrom<List<Event>>(viewResult.ViewData["Events"]);
            Assert.Single(users);
            Assert.Single(events);
        }

        [Fact]
        public async Task CreateAssignment_POST_ValidModel_AddsAssignment()
        {
            var context = GetDbContext("CreateAssignmentPOSTDb");
            context.Events.Add(new Event { EventID = 1, Name = "Event Y", Description = "Description 1", Location = "Location 1" });
            context.Users.Add(new ApplicationUser { Id = "1", UserName = "Bob", Name = "Bob", Role = "User" });
            await context.SaveChangesAsync();

            var assignment = new Assignment { AssignmentID = 1, UserID = "1", EventID = 1, RoleInProject = "Volunteer" };
            var controller = new AdminController(context);

            var result = await controller.CreateAssignment(assignment);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Assignments", redirect.ActionName);
            Assert.Single(context.Assignments);
        }

        [Fact]
        public async Task EditAssignment_GET_ReturnsAssignmentView()
        {
            var context = GetDbContext("EditAssignmentGETDb");
            var assignment = new Assignment { AssignmentID = 1, UserID = "1", EventID = 1, RoleInProject = "Leader" };
            context.Assignments.Add(assignment);
            await context.SaveChangesAsync();

            var controller = new AdminController(context);
            var result = await controller.EditAssignment(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Assignment>(viewResult.Model);
            Assert.Equal("Leader", model.RoleInProject);
        }

        [Fact]
        public async Task DeleteAssignment_ValidId_RemovesAssignment()
        {
            var context = GetDbContext("DeleteAssignmentValidDb");
            var assignment = new Assignment { AssignmentID = 1, UserID = "1", EventID = 1, RoleInProject = "Leader" };
            context.Assignments.Add(assignment);
            await context.SaveChangesAsync();

            var controller = new AdminController(context);
            var result = await controller.DeleteAssignment(1);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Assignments", redirect.ActionName);
            Assert.Empty(context.Assignments);
        }
    }
}
