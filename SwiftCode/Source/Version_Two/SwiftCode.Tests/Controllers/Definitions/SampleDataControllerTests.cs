using Microsoft.AspNetCore.Mvc;
using SwiftCode.Web.Controllers;
using System.Linq;
using TechTalk.SpecFlow;
using Xunit;
using static SwiftCode.Web.Controllers.SampleDataController;

namespace SwiftCode.Tests.Controllers.Definitions
{
    [Binding]
    public sealed class SampleDataControllerTests
    {
        // For additional details on SpecFlow step definitions see http://go.specflow.org/doc-stepdef

        [Given(@"SampleDataController type of Contoller")]
        public void GivenSampleDataControllerType()
        {
            // Arrange
            var controller = new SampleDataController();

            // Act
            var result = controller as Controller;

            // Assert
            Assert.NotNull(result);
        }

        [When(@"it returns WeatherForecasts")]
        public void WhenItReturnsWeatherForecasts()
        {
            // Arrange
            var controller = new SampleDataController();

            // Act
            var result = controller.WeatherForecasts();

            // Assert
            Assert.All(result, item => Assert.IsType<WeatherForecast>(item));
        }

        [Then(@"The WeatherForecasts should be (.*)")]
        public void ThenTheWeatherForecastsShouldBe(int p0)
        {
            // Arrange
            const int expected = 5;
            var controller = new SampleDataController();

            // Act
            var result = controller.WeatherForecasts().ToList().Count;

            // Assert
            Assert.Equal(result, expected);
        }
    }
}
