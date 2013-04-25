using System;
using System.Collections;
using System.Reflection;
using SharpUnit;
using UnityEngine;

namespace chomechomo {
    public class TestRunner : Unity3D_TestRunner {
        protected override void AddCompornents(Unity3D_TestSuite suite) {
            suite.AddAll(gameObject.AddComponent("MsgpackTestCase") as UnityTestCase);
        }
    }
}

