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
    public class UserControllerTests
    {
        private ApplicationDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Index_ReturnsView()
        {
            // Arrange  
            var context = GetDbContext("IndexTestDb");
            var controller = new UserController(context);

            // Act  
            var result = controller.Index();

            // Assert  
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Events_ReturnsListOfEvents()
        {
            // Arrange  
            var context = GetDbContext("EventsTestDb");
            context.Events.Add(new Event { Name = "Event 1", Description = "Description 1", Location = "Location 1" });
            context.Events.Add(new Event { Name = "Event 2", Description = "Description 2", Location = "Location 2" });
            await context.SaveChangesAsync();

            var controller = new UserController(context);

            // Act  
            var result = await controller.Events();

            // Assert  
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Event>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public void CreateDonation_GET_ReturnsViewWithEvents()
        {
            // Arrange  
            var context = GetDbContext("CreateDonationGETDb");
            context.Events.Add(new Event { Name = "Event A", Description = "Description A", Location = "Location A" });
            context.Events.Add(new Event { Name = "Event B", Description = "Description B", Location = "Location B" });
            context.SaveChanges();

            var controller = new UserController(context);

            // Act  
            var result = controller.CreateDonation();

            // Assert  
            var viewResult = Assert.IsType<ViewResult>(result);
            var events = Assert.IsAssignableFrom<List<Event>>(viewResult.ViewData["Events"]);
            Assert.Equal(2, events.Count);
        }

        [Fact]
        public async Task CreateDonation_POST_ValidModel_AddsDonationAndRedirects()
        {
            // Arrange  
            var context = GetDbContext("CreateDonationPOSTValidDb");
            context.Events.Add(new Event { EventID = 1, Name = "Event X", Description = "Description X", Location = "Location X" });
            await context.SaveChangesAsync();

            var controller = new UserController(context);
            var donation = new Donation { DonorName = "John", EventID = 1, Amount = 100, ItemDescription = "Test Donation" };

            // Act  
            var result = await controller.CreateDonation(donation);

            // Assert  
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Events", redirect.ActionName);
            Assert.Single(context.Donations);
        }

        [Fact]
        public async Task CreateDonation_POST_InvalidModel_ReturnsViewWithEvents()
        {
            // Arrange  
            var context = GetDbContext("CreateDonationPOSTInvalidDb");
            context.Events.Add(new Event { EventID = 1, Name = "Event X", Description = "Description X", Location = "Location X" });
            await context.SaveChangesAsync();

            var controller = new UserController(context);
            controller.ModelState.AddModelError("DonorName", "Required");
            var tempEvent = new Event { EventID = 1, Name = "Event X", Description = "Description X", Location = "Location X" };
            var donation = new Donation { EventID = 1, Amount = 50, ItemDescription = "Test Donation", Event = tempEvent };

            // Act  
            var result = await controller.CreateDonation(donation);

            // Assert  
            var viewResult = Assert.IsType<ViewResult>(result);
            var events = Assert.IsAssignableFrom<List<Event>>(viewResult.ViewData["Events"]);
            Assert.Single(events);
            Assert.Empty(context.Donations);
        }

        [Fact]
        public void CreateResource_GET_ReturnsViewWithEvents()
        {
            // Arrange  
            var context = GetDbContext("CreateResourceGETDb");
            context.Events.Add(new Event { Name = "Event Y", Description = "Description Y", Location = "Location Y" });
            context.Events.Add(new Event { Name = "Event Z", Description = "Description Z", Location = "Location Z" });
            context.SaveChanges();

            var controller = new UserController(context);

            // Act  
            var result = controller.CreateResource();

            // Assert  
            var viewResult = Assert.IsType<ViewResult>(result);
            var events = Assert.IsAssignableFrom<List<Event>>(viewResult.ViewData["Events"]);
            Assert.Equal(2, events.Count);
        }

        [Fact]
        public async Task CreateResource_POST_ValidModel_AddsResourceAndRedirects()
        {
            // Arrange  
            var context = GetDbContext("CreateResourcePOSTValidDb");
            context.Events.Add(new Event { EventID = 1, Name = "Event 1", Description = "Description 1", Location = "Location 1" });
            await context.SaveChangesAsync();

            var controller = new UserController(context);
            var resource = new Resource { Name = "Chairs", Quantity = 10, EventID = 1, Unit = "" };

            // Act  
            var result = await controller.CreateResource(resource);

            // Assert  
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Events", redirect.ActionName);
            Assert.Single(context.Resources);
        }

        [Fact]
        public async Task CreateResource_POST_InvalidModel_ReturnsViewWithEvents()
        {
            // Arrange  
            var context = GetDbContext("CreateResourcePOSTInvalidDb");
            context.Events.Add(new Event { EventID = 1, Name = "Event 1", Description = "Description 1", Location = "Location 1" });
            await context.SaveChangesAsync();

            var controller = new UserController(context);
            controller.ModelState.AddModelError("Name", "Required");
            var resource = new Resource { Quantity = 5, EventID = 1 };

            // Act  
            var result = await controller.CreateResource(resource);

            // Assert  
            var viewResult = Assert.IsType<ViewResult>(result);
            var events = Assert.IsAssignableFrom<List<Event>>(viewResult.ViewData["Events"]);
            Assert.Single(events);
            Assert.Empty(context.Resources);
        }
    }  
}
