/**
 * @file TestSuite.cs
 * 
 * Test suite class, used for running a collection of tests.
 */

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SharpUnit;

public class Unity3D_TestSuite : MonoBehaviour
{
    // Member values
    private List<ITestCase> m_tests = null;  // List of test cases to run.
    private TestResult _TestResult;
    public TestResult TestResult {
        get { return _TestResult; }
    }
    
    /**
     * Constructor
     */
    public Unity3D_TestSuite()
    {
        // Create test list
        m_tests = new List<ITestCase>();
    }

    /**
     * Destructor
     */
    ~Unity3D_TestSuite()
    {
        // Clear list
        m_tests = null;
    }

    /**
     * Add all test cases to the test suite.
     * 
     * @param test, the test case containing the tests we will add.
     */
    public void AddAll(UnityTestCase test)
    {
        // If test invalid
        if (null == test)
            {
                // Error
                throw new Exception("Invalid test case encountered.");
            }

        // For each method in the test case
        Type type = test.GetType();
        foreach (MethodInfo method in type.GetMethods())
            {
                // For each unit test attribute
                foreach (object obj in method.GetCustomAttributes(typeof(UnitTest), false))
                    {
                        // If attribute is valid
                        Attribute testAtt = obj as Attribute;
                        if (null != testAtt)
                            {
                                // create GameObject and attach
                                GameObject gameobj = new GameObject("Test" + type.Name + "_" + method.Name);
                                UnityTestCase tmp = gameobj.AddComponent(type.Name) as UnityTestCase;
                                tmp.SetTestMethod(method.Name);
                                m_tests.Add(tmp);
                            }
                    }
            }
    }

    public void AddAll(TestCase test)
    {
        // If test invalid
        if (null == test)
            {
                // Error
                throw new Exception("Invalid test case encountered.");
            }

        // For each method in the test case
        Type type = test.GetType();
        foreach (MethodInfo method in type.GetMethods())
            {
                // For each unit test attribute
                foreach (object obj in method.GetCustomAttributes(typeof(UnitTest), false))
                    {
                        // If attribute is valid
                        Attribute testAtt = obj as Attribute;
                        if (null != testAtt)
                            {
                                // If type has constructors
                                ConstructorInfo[] ci= type.GetConstructors();
                                if (0 < ci.Length)
                                    {
                                        // Add the test
                                        TestCase tmp = ci[0].Invoke(null) as TestCase;
                                        tmp.SetTestMethod(method.Name);
                                        m_tests.Add(tmp);
                                    }
                            }
                    }
            }
    }

    /**
     * Run all of the tests in the test suite.
     * 
     * @param result, result of the test run.
     */
    public IEnumerator Run(TestResult result)
    {
        // For each test
        foreach (ITestCase test in m_tests)
            {
                // Run test
                yield return StartCoroutine(test.Run(result));
                result = test.GetTestResult();
            }
        
        _TestResult = result;
    }
    
}



