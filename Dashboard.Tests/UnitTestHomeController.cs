using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Dashboard.Models;
using Xunit;
using Dashboard.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Dashboard.Controllers;
using Microsoft.Extensions.Logging;
using Dashboard.Models;

namespace Dashboard.Tests;
public class UnitTestHomeController
{
    private readonly Mock<ILogger<HomeController>> _mockLogger;
    private readonly HomeController _controller;

    public UnitTestHomeController()
    {
        _mockLogger = new Mock<ILogger<HomeController>>();
        _controller = new HomeController(_mockLogger.Object);
    }

    [Fact]
    public void Index_ReturnsViewResult()
    {
        // Arrange

        // Act
        var result = _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.ViewName);  // Par défaut, le nom de la vue est null, ce qui signifie "index".
    }

    [Fact]
    public void Privacy_ReturnsViewResult()
    {
        // Arrange

        // Act
        var result = _controller.Privacy();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.ViewName);  // Par défaut, le nom de la vue est null, ce qui signifie "privacy".
    }
}
