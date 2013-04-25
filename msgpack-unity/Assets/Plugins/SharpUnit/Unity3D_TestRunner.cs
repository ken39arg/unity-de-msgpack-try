/**
 * @file TestRunner.cs
 * 
 * Unity3D unit test runner.
 * Sets up the unit testing suite and executes all unit tests.
 * Drag this onto an empty GameObject to run tests.
 */

using System;
using System.Collections;
using System.Reflection;
using SharpUnit;
using UnityEngine;

public class Unity3D_TestRunner : MonoBehaviour 
{

    void Start() {
        StartCoroutine(StartTest());
    }

    /**
     * Initialize class resources.
     */
    public IEnumerator StartTest() 
    {
        // Create test suite
        Unity3D_TestSuite suite = gameObject.AddComponent<Unity3D_TestSuite>();

		AddCompornents(suite);

        // Run the tests
        yield return StartCoroutine(suite.Run(null));
        TestResult res = suite.TestResult;

        // Report results
        Unity3D_TestReporter reporter = new Unity3D_TestReporter();
        reporter.LogResults(res);
	}

	protected virtual void AddCompornents(Unity3D_TestSuite suite) {
        // For each assembly in this app domain
        foreach (Assembly assem in AppDomain.CurrentDomain.GetAssemblies())
        {
            // For each type in the assembly
            foreach (Type type in assem.GetTypes())
            {
                // If this is a valid test case
                // i.e. derived from TestCase and instantiable
                if (typeof(UnityTestCase).IsAssignableFrom(type) &&
                    type != typeof(UnityTestCase) && !type.IsAbstract)
                {
                    // Add tests to suite
                    UnityTestCase test = gameObject.AddComponent(type.Name) as UnityTestCase;
                    suite.AddAll(test);
                } else if (typeof(TestCase).IsAssignableFrom(type) &&
                    type != typeof(TestCase) &&
                    !type.IsAbstract)
                {
                    suite.AddAll(type.GetConstructor(new Type[0]).Invoke(new object[0]) as TestCase);
                }
            }
        }
	}
}
