namespace ExtractionService.Test
{
    using System;
    using System.IO;
    using System.Linq;
    using ExtractionService.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Summary tests for ResentFilesExtractionService
    /// </summary>
    [TestClass]
    public class DocumentExtractionTests
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        private DocumentExtraction _extractionService = null;

        private readonly string _source = Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())) + @"\Docs\source_directory";
        private readonly string _target = Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())) + @"\Docs\target_directory";

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestInitialize]
        public void DocumentExtraction_Initialize()
        {
            try
            {
                _extractionService = new DocumentExtraction();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }

        [TestMethod]
        public void DocumentExtractionHasIDocumentExtraction()
        {
            Assert.IsTrue(typeof(IDocumentExtraction)
                  .IsAssignableFrom(typeof(DocumentExtraction)));
        }

        [TestMethod, ExpectedException(typeof(System.ArgumentNullException))]
        public void SourceIsNullThanThrownArgumentNullException()
        {
            _extractionService.CopyFiles(string.Empty, _source);
        }

        [TestMethod, ExpectedException(typeof(System.ArgumentNullException))]
        public void TargetIsNullThanThrownArgumentNullException()
        {
            _extractionService.CopyFiles(_source, string.Empty);
        }

        [TestMethod, ExpectedException(typeof(System.IO.DirectoryNotFoundException))]
        public void SourceIsUnderfindThanThrownDirectoryNotFoundException()
        {
            string mock = @"C:\Foo\Bar";

            _extractionService.CopyFiles(mock, _target);
        }

        [TestMethod, ExpectedException(typeof(System.IO.DirectoryNotFoundException))]
        public void TargetIsUnderfindThanThrownDirectoryNotFoundException()
        {
            string mock = @"C:\Foo\Bar";

            _extractionService.CopyFiles(_source, mock);
        }

        [TestMethod]
        public void DocumentExtractionTestForTryExtractionFiles()
        {
            try
            {
                // Remove all files before test from target directory.
                Array.ForEach(Directory.GetFiles(_target, "*", SearchOption.AllDirectories), File.Delete);

                int source = Directory.GetFiles(_source, "*", SearchOption.AllDirectories).Count();

                if (source <= 0) Assert.Fail("The directory with test files is empty!");

                _extractionService.CopyFiles(_source, _target);

                int target = Directory.GetFiles(_target).Count();

                Assert.AreEqual(source, target);
                // Remove all files after test from target directory.
                Array.ForEach(Directory.GetFiles(_target, "*", SearchOption.AllDirectories), File.Delete);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }         
        }
    }
}
