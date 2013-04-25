/**
 * @file TestCase.cs
 *
 * This class defines a "test case."
 * A test case is a class that contains several methods that test and
 * verify the expected functionality of another class using the Assert methods.
 */

using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using UnityEngine;

namespace SharpUnit
{
    public class UnityTestCase : MonoBehaviour, ITestCase
    {
        // Members
        private string m_testMethod = null;     // Name of the test method to run.
        private Exception m_caughtEx = null;    // Exception thrown by unit test method.
        private TestResult _TestResult = null;
        public TestResult GetTestResult() {
            return _TestResult;
        }

        protected bool _Failed = true;
        protected TestException _Exception = null;
        protected void MarkAsFailure(TestException e) {
            _Failed = true;
            _Exception = e;
        }
        protected void DoneTesting() {
            _Failed = false;
        }

        /**
         * Perform any setup before the test is run.
         */
        public virtual void SetUp()
        {
            // Base class has nothing to setup.
        }

        /**
         * Perform any clean up after the test has run.
         */
        public virtual void TearDown()
        {
            // Base class has nothing to tear down.
        }

        /**
         * Set the name of the test method to run.
         *
         * @param method, the test method to run.
         */
        public void SetTestMethod(string method)
        {
            m_testMethod = method;
        }

        /**
         * Run the test, catching all exceptions.
         * 
         * @param result, the result of the test.
         * 
         * @return TestResult, the result of the test.
         */
        public IEnumerator Run(TestResult result)
        {
            // If test method invalid
            if (null == m_testMethod)
            {
                // Error
                throw new Exception("Invalid test method encountered, be sure to call " +
                                    "TestCase::SetTestMethod()");
            }

            // If the test method does not exist
            Type type = GetType();
            MethodInfo method = type.GetMethod(m_testMethod);
            if (null == method)
            {
                // Error
                throw new Exception("Test method: " + m_testMethod + " does not exist in class: " +
                                    type.ToString());
            }

            // If result invalid
            if (null == result)
            {
                // Create result
                result = new TestResult();
            }

            // Pre-test setup
            SetUp();
            result.TestStarted();

            yield return StartCoroutine((IEnumerator)method.Invoke(this, null));

            if (_Exception != null) {
                result.TestFailed(_Exception);
                UnityEngine.Debug.LogWarning("[SharpUnit] " + type.Name + "." + method.Name + " failed");

            } else if (_Failed) {
                result.TestFailed(new TestException("DoneTesting for " + type.Name + "." + method.Name +
                                                    " not called."));
                UnityEngine.Debug.LogWarning("[SharpUnit] " + type.Name + "." + method.Name +
                                             " might be failed");
            } else {
                UnityEngine.Debug.Log("[SharpUnit] " + type.Name + "." + method.Name + " runs ok");
            }

            // Clear expected exception
            Assert.Exception = null;

            // Post-test cleanup
            TearDown();

            _TestResult = result;
        }
    } 
}
