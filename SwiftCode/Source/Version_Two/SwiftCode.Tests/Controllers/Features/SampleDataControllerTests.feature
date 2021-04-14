Feature: SampleDataControllerTests
	In order to make shure that all things works
	I made this test

@SampleDataControllerTests
Scenario: Get forecast on 5 days
	Given SampleDataController type of Contoller
	When it returns WeatherForecasts
	Then The WeatherForecasts should be 5