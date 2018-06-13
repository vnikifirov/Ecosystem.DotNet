using AutoMapper;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SwiftCode.Core.Interfaces.Models.Common;
using SwiftCode.Core.Interfaces.Services;
using SwiftCode.Core.Mapping;
using SwiftCode.Core.Services;
using TechTalk.SpecFlow;

namespace SwiftCode.Tests.Services.Definitions
{
    [Binding]
    public class ReaderTestsSteps
    {
        [Given(@"I have IReader")]
        public void GivenIHaveIReader()
        {
            // Arrange
            var reader = new Reader(null);

            // Act
            var result = reader as IReader;

            // Assert
            Assert.IsNotNull(result);
        }
        
        [Given(@"It takes IMapper as a parameter")]
        public void GivenItTakesIMapperAsAParameter()
        {
            // Arrange
            var mapper = A.Fake<IMapper>();
            
            // Act
            var result = new Reader(mapper);            

            // Assert
            Assert.IsNotNull(result);
        }
        
        [When(@"I try to read a file")]
        public void WhenITryToReadAFile()
        {
            // Arrange
            var mapper = A.Fake<IMapper>();
            var filePath = string.Empty;

            //var reader 

            //var reader = A.Fake<IReader>(
            //    x => x.WithArgumentsForConstructor(
            //        () => new Reader(mapper)));

            // Act
            A.CallTo(() => reader.ReadAsync(filePath)).Returns(lollipop);


            // Assert

            // Specifying arguments for constructor using IEnumerable<object>.
            var foo = A.Fake<FooClass>(x => x.WithArgumentsForConstructor(new object[] { "foo", "bar" }));

            // Specifying additional interfaces to be implemented. Among other uses,
            // this can help when a fake skips members because they have been
            // explicitly implemented on the class being faked.
            var foo = A.Fake<FooClass>(x => x.Implements(typeof(IFoo)));
            // or
            var foo = A.Fake<FooClass>(x => x.Implements<IFoo>());

            // Assigning custom attributes to the faked type.
            // foo's type should have "FooAttribute"
            var foo = A.Fake<IFoo>(x => x.WithAttributes(() => new FooAttribute()));

            // Create wrapper - unconfigured calls will be forwarded to wrapped
            var wrapped = new FooClass("foo", "bar");
            var foo = A.Fake<IFoo>(x => x.Wrapping(wrapped));

            //// Arrange
            //var mapper = A.Fake<IMapper>();
            //var model = A.Fake<BaseModel>();

            //// Act
            //var result = new Reader(mapper);

            //// Assert
            //result.ReadAsync<BaseModel>(string.Empty);
        }
        
        [Then(@"the result should be Nth numbers of records from a given file")]
        public void ThenTheResultShouldBeNthNumbersOfRecordsFromAGivenFile()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
