using System;
using Xunit;
using WebApi.Controllers;
using System.Linq;

namespace Tests
{
    public class SampleDataControllerTest
    {
        [Fact]
        public void Get_WeatherForecasts()
        {
            // Arrange
            var controller = new SampleDataController();
            
            // Act
            var result = controller.WeatherForecasts().ToList();

            // Assert
            //Assert.Equal(5, result.Count);
            Assert.Equal(5, result.Count);
        }  
    }
}
