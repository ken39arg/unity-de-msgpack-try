﻿/**
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

namespace SharpUnit
{
    public class TestCase : ITestCase
    {
        // Members
        private string m_testMethod = null;     // Name of the test method to run.
        private Exception m_caughtEx = null;    // Exception thrown by unit test method.
        
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

        private TestResult _TestResult;
        public TestResult GetTestResult() {
            return _TestResult;
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
                throw new Exception("Invalid test method encountered, be sure to call TestCase::SetTestMethod()");
            }

            // If the test method does not exist
            Type type = GetType();
            MethodInfo method = type.GetMethod(m_testMethod);
            if (null == method)
            {
                // Error
                throw new Exception("Test method: " + m_testMethod + " does not exist in class: " + type.ToString());
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

            try
            {
                try
                {
                    // Try to call the test method    
                    method.Invoke(this, null);
                }
                catch (TargetInvocationException e)
                {
                    // If we are expecting an exception
                    m_caughtEx = e;
                    if (null != Assert.Exception)
                    {
                        // Compare the exceptions
                        Assert.Equal(Assert.Exception, m_caughtEx.InnerException);
                    }
                    else
                    {
                        // If this is a test exception
                        if (null != e.InnerException &&
                            typeof(TestException) == e.InnerException.GetType())
                        {
                            // Set the description
                            TestException te = e.InnerException as TestException;
                            te.Description  = "Failed: " + GetType() + "." + m_testMethod + "()";
                            if (null != te.StackFrame)
                            {
                                // Add stack frame info
                                te.Description += " in File: " + System.IO.Path.GetFileName( te.StackFrame.GetFileName() );
                                te.Description += " on Line: " + te.StackFrame.GetFileLineNumber();
                            }
                        }

                        // Re-throw the exception to be tracked
                        throw m_caughtEx.InnerException;
                    }
                }

                // If we are expecting an exception but did not catch one
                if (null != Assert.Exception &&
                    null == m_caughtEx)
                {
                    // Error
                    Assert.NotNull(m_caughtEx, "Did not catch expected exception: " + Assert.Exception);
                }

                UnityEngine.Debug.Log("[SharpUnit] " + type.Name + "." + method.Name + " runs ok");
            }
            catch (Exception ex)
            {
                // Track test failure
                result.TestFailed( ex );
                UnityEngine.Debug.LogWarning("[SharpUnit] " + type.Name + "." + method.Name + " failed");
            }



            // Clear expected exception
            Assert.Exception = null;

            // Post-test cleanup
            TearDown();

            _TestResult = result;
            yield break;
        }
    } 
}
