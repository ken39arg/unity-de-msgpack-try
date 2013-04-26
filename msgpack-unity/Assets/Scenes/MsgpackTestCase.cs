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
            Application.targetFrameRate = 1000;
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

        const int FUNC_RUNTIME = 3000;
        const int HTTP_RUNTIME = 200;
        const int DATA_LIST_COUNT = 100;

        public float persec( int runtime, long millisec ) {
            return (float)(((double)runtime / (double)millisec) * 1000d);
        }

        [UnitTest]
        public IEnumerator JsonSerializeBenchmark()
        {
            JSON json = gameObject.AddComponent("JSON") as JSON;
            Dictionary<string, object> data = GetBigData(DATA_LIST_COUNT);
            byte[] bytes = json.Serialize( data );
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            for (int i=0; i < FUNC_RUNTIME; i++) {
                bytes = json.Serialize( data );
            }
            stopWatch.Stop();

            Debug.Log("Json Serialize: n=" + FUNC_RUNTIME
                    + "\r\n   Time:   " + (stopWatch.ElapsedMilliseconds / 1000f).ToString()
                    + "\r\n   Length: " + bytes.Length
                    + "\r\n   Rate:   " + persec( FUNC_RUNTIME, stopWatch.ElapsedMilliseconds) + "/s");

            Assert.True(true);

            DoneTesting();
            yield break;
        }

        [UnitTest]
        public IEnumerator MsgpackSerializeBenchmark()
        {
            Msgpack msgpack = gameObject.AddComponent("Msgpack") as Msgpack;
            Dictionary<string, object> data = GetBigData(DATA_LIST_COUNT);
            byte[] bytes = msgpack.Serialize( data );;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            for (int i=0; i < FUNC_RUNTIME; i++) {
                bytes = msgpack.Serialize( data );
            }
            stopWatch.Stop();

            Debug.Log("Msgpack Serialize: n=" + FUNC_RUNTIME
                    + "\r\n   Time:   " + (stopWatch.ElapsedMilliseconds / 1000f).ToString()
                    + "\r\n   Length: " + bytes.Length
                    + "\r\n   Rate:   " + persec( FUNC_RUNTIME, stopWatch.ElapsedMilliseconds) + "/s");

            Assert.True(true);

            DoneTesting();
            yield break;
        }

        [UnitTest]
        public IEnumerator JsonDeserializeBenchmark()
        {
            JSON json = gameObject.AddComponent("JSON") as JSON;
            Dictionary<string, object> data = GetBigData(DATA_LIST_COUNT);
            byte[] bytes = json.Serialize( data );
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            for (int i=0; i < FUNC_RUNTIME; i++) {
                IDictionary<string, object> ret = json.Deserialize( bytes );
            }
            stopWatch.Stop();

            Debug.Log("JSON Deserialize: n=" + FUNC_RUNTIME
                    + "\r\n   Time:   " + (stopWatch.ElapsedMilliseconds / 1000f).ToString()
                    + "\r\n   Length: " + bytes.Length
                    + "\r\n   Rate:   " + persec( FUNC_RUNTIME, stopWatch.ElapsedMilliseconds) + "/s");

            Assert.True(true);

            DoneTesting();
            yield break;
        }

        [UnitTest]
        public IEnumerator MsgpackDeserializeBenchmark()
        {
            Msgpack msgpack = gameObject.AddComponent("Msgpack") as Msgpack;
            Dictionary<string, object> data = GetBigData(DATA_LIST_COUNT);
            byte[] bytes = msgpack.Serialize( data );;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            for (int i=0; i < FUNC_RUNTIME; i++) {
                IDictionary<string, object> ret = msgpack.Deserialize( bytes );
            }
            stopWatch.Stop();

            Debug.Log("Msgpack Deserialize: n=" + FUNC_RUNTIME
                    + "\r\n   Time:   " + (stopWatch.ElapsedMilliseconds / 1000f).ToString()
                    + "\r\n   Length: " + bytes.Length
                    + "\r\n   Rate:   " + persec( FUNC_RUNTIME, stopWatch.ElapsedMilliseconds) + "/s");

            Assert.True(true);

            DoneTesting();
            yield break;
        }

        [UnitTest]
        public IEnumerator JsonBenchmark()
        {
            JSON json = gameObject.AddComponent("JSON") as JSON;
            json.Type = Types.BODY;
            bool success = true;

            Debug.Log("Start JSON Bench");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            for (int i=0; i < HTTP_RUNTIME; i++) {
                yield return StartCoroutine(json.Request( GetBigData( i % 100 ) ));

                if ( json.IsError ) {
                    Assert.True(false, "Fail");
                    success = false;
                    break;
                }
            }
            stopWatch.Stop();

            Debug.Log("JSON HTTP: n=" + HTTP_RUNTIME 
                    + "\r\n   Time:   " + (stopWatch.ElapsedMilliseconds / 1000f).ToString()
                    + "\r\n   Rate:   " + persec( HTTP_RUNTIME, stopWatch.ElapsedMilliseconds) + "/s");

            Assert.True(success);

            DoneTesting();
        }

        [UnitTest]
        public IEnumerator MsgpackBenchmark()
        {
            Msgpack msgpack = gameObject.AddComponent("Msgpack") as Msgpack;
            msgpack.Type = Types.BODY;

            bool success = true;

            Debug.Log("Start Msgpack Bench");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            for (int i=0; i < HTTP_RUNTIME; i++) {
                yield return StartCoroutine(msgpack.Request( GetBigData(i % 100) ));

                if ( msgpack.IsError ) {
                    Assert.True(false, "Fail");
                    success = false;
                    break;
                }
            }
            stopWatch.Stop();

            Debug.Log("Msgpack HTTP: n=" + HTTP_RUNTIME 
                    + "\r\n   Time:   " + (stopWatch.ElapsedMilliseconds / 1000f).ToString()
                    + "\r\n   Rate:   " + persec( HTTP_RUNTIME, stopWatch.ElapsedMilliseconds) + "/s");


            Assert.True(success);

            DoneTesting();
        }

        [UnitTest]
        public IEnumerator BigData() {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            for (int i=0; i < 100; i++) {
                IDictionary<string, object> ret = GetBigData(i);
            }
            stopWatch.Stop();

            Debug.Log("CreateData: n=" +100 
                    + "\r\n   Time:   " + (stopWatch.ElapsedMilliseconds / 1000f).ToString()
                    + "\r\n   Rate:   " + persec( FUNC_RUNTIME, stopWatch.ElapsedMilliseconds) + "/s");

            DoneTesting();
            yield break;

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

        public Dictionary<string, object> GetBigData ( int count ) {
            Dictionary<string, object> param = GetParam("test", 1);

            List<int> intArr = new List<int> ();
            for (int i = 0; i < count; i++) intArr.Add(i);
            param.Add("intarr",   intArr);

            List<string> strArr = new List<string> ();
            for (int i = 0; i < count; i++) strArr.Add("text_" + i.ToString());
            param.Add("strarr",   strArr);

            List<bool> boolArr = new List<bool> ();
            for (int i = 0; i < count; i++) boolArr.Add(i % 2 == 1 ? true : false);
            param.Add("boolarr",   boolArr);

            List<Dictionary<string, object>> mapArr = new List<Dictionary<string, object>> ();
            for (int i = 0; i < count; i++) mapArr.Add(GetParam( "map"+i, i + 10000));
            param.Add("maparr", mapArr);

            return param;
        }
    }
}

/*
Json Serialize: n=3000
   Time:   9.993
   Length: 17211
   Rate:   300.2101/s

Msgpack Serialize: n=3000
   Time:   7.701
   Length: 11118
   Rate:   389.5598/s
   
JSON Deserialize: n=3000
   Time:   19.12
   Length: 17211
   Rate:   156.9038/s

Msgpack Deserialize: n=3000
   Time:   7.83
   Length: 11118
   Rate:   383.1418/s
   
JSON HTTP: n=200
   Time:   306.695
   Rate:   0.6521137/s

Msgpack HTTP: n=200
   Time:   287.642
   Rate:   0.6953087/s

CreateData: n=100
   Time:   0.044
   Rate:   68181.82/s
*/
