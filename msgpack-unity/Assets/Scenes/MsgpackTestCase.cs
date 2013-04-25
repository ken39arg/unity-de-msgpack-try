using SharpUnit;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using Debug = UnityEngine.Debug;

namespace chomechomo
{
    public class MsgpackTestCase : UnityTestCase {
        public override void SetUp() {
        }

        public override void TearDown() {
        }

        [UnitTest]
        public IEnumerator JsonBodyTest()
        {
            JSON json = gameObject.AddComponent("JSON") as JSON;

            json.Type = Types.BODY;

            yield return StartCoroutine(json.Request( GetParam("jtest1", 1) ));

            Assert.False(json.IsError);

            Assert.Equal((long)json.LastResult["int"], 14142);
            Assert.Equal((string)json.LastResult["name"], "jtest1");
            Assert.Equal(json.LastResult["nil"], null);
            Assert.Equal((bool)json.LastResult["bool"], true);

            IList<object> arr = json.LastResult["arr"] as IList<object>;
            Assert.True( arr != null );
            Assert.Equal((long)arr[2], 8);

            IDictionary<string,object> map = json.LastResult["map"] as IDictionary<string, object>;
            Assert.True( map != null );
            Assert.Equal((string)map["foo"], "bar");

            DoneTesting();
        }

        [UnitTest]
        public IEnumerator MsgpackBodyTest()
        {
            Msgpack msgpack = gameObject.AddComponent("Msgpack") as Msgpack;

            msgpack.Type = Types.BODY;

            yield return StartCoroutine(msgpack.Request( GetParam("mtest1", 2) ));

            Assert.False(msgpack.IsError);

            Assert.Equal((uint)msgpack.LastResult["int"], 14142);
            Assert.Equal((string)msgpack.LastResult["name"], "mtest1");
            Assert.Equal(msgpack.LastResult["nil"], null);
            Assert.Equal((bool)msgpack.LastResult["bool"], true);

            IList<object> arr = msgpack.LastResult["arr"] as IList<object>;
            Assert.True( arr != null );
            Assert.Equal((int)arr[2], 8);

            IDictionary<string,object> map = msgpack.LastResult["map"] as IDictionary<string, object>;
            Assert.True( map != null );
            Assert.Equal((string)map["foo"], "bar");

            DoneTesting();
        }

        const int RUNTIME = 5000;

        [UnitTest]
        public IEnumerator JsonBenchmark()
        {
            JSON json = gameObject.AddComponent("JSON") as JSON;
            json.Type = Types.BODY;
            bool success = true;

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            for (int i=0; i < RUNTIME; i++) {
                yield return StartCoroutine(json.Request( GetParam("jtest1", i) ));

                if ( json.IsError ) {
                    Assert.True(false, "Fail");
                    success = false;
                    break;
                }
            }
            stopWatch.Stop();

            Debug.Log("Time: " +  stopWatch.ElapsedMilliseconds.ToString() );

            Assert.True(success);

            DoneTesting();
        }

        [UnitTest]
        public IEnumerator MsgpackBenchmark()
        {
            Msgpack msgpack = gameObject.AddComponent("Msgpack") as Msgpack;
            msgpack.Type = Types.BODY;

            bool success = true;

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            for (int i=0; i < RUNTIME; i++) {
                yield return StartCoroutine(msgpack.Request( GetParam("mtest1", i) ));

                if ( msgpack.IsError ) {
                    Assert.True(false, "Fail");
                    success = false;
                    break;
                }
            }
            stopWatch.Stop();

            Debug.Log("Time: " +  stopWatch.ElapsedMilliseconds.ToString() );

            Assert.True(success);

            DoneTesting();
        }

        public Dictionary<string, object> GetParam (string name, int id) {
            Dictionary<string, object> param = new Dictionary<string, object>();

            param.Add("id", id);
            param.Add("name", name);
            param.Add("int",   14142);
            param.Add("float", 3.141565f);
            param.Add("bool",  true);
            param.Add("nil",   null);
            param.Add("str",   "test");

            List<int> arr = new List<int> {1, 2, 8, 9};
            param.Add("arr",   arr);

            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("foo", "bar");
            map.Add("buzz", 551);
            map.Add("piyo", 2.14);
            param.Add("map",   map);

            return param;
        }
    }
}
