using System.Collections;

namespace SharpUnit
{
    public interface ITestCase
    {
        
        /**
         * Perform any setup before the test is run.
         */
        void SetUp();

        /**
         * Perform any clean up after the test has run.
         */
        void TearDown();

        /**
         * Set the name of the test method to run.
         *
         * @param method, the test method to run.
         */
        void SetTestMethod(string method);

        /**
         * Run the test, catching all exceptions.
         * 
         * @param result, the result of the test.
         * 
         * @return TestResult, the result of the test.
         */
        IEnumerator Run(TestResult result);


        /**
         * The TestResult
         */
        TestResult GetTestResult();
    } 
}


