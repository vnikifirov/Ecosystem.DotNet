Feature: ReaderTests
	I'd like to make sure that a Reader read a file and 
	return Enumerable collection of records

@mytag
Scenario: Add two numbers
	Given I have IReader	
	And It takes IMapper as a parameter
	When I try to read a file
	Then the result should be Nth numbers of records from a given file
