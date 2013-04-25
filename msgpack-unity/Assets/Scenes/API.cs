using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace chomechomo
{
    public enum Types {
        POST,
        BODY
    }
    abstract public class API : MonoBehaviour
    {
        public string Url {
            get {
                string type = (Type == Types.BODY) ? "body" : "post";
                return "http://" + Config.HOST + ":" + Config.PORT + 
                       "?format=" + Format + "&type=" + type;
            }
        }

        public Types Type = Types.BODY;

        public abstract string Format { get; }

        public bool IsError {
            get {
                return 0 < ErrorMessage.Length;
            }
        }

        public string ErrorMessage {
            get; private set;
        }

        public IDictionary<string, object> LastResult {
            get; private set;
        }

        public string LastBody {
            get; private set;
        }

        public int LastLength {
            get; private set;
        }

        public IEnumerator Request ( IDictionary<string, object> param ) {
            byte[] body = (Type == Types.BODY) ?
                               Serialize(param) : BuildPostBody(param);

            LastResult = new Dictionary<string, object>();
            LastLength = 0;
            LastBody   = "";
            ErrorMessage = "";

            WWW www = new WWW(Url, body);
            
            yield return www;

            LastLength = www.bytes.Length;

            if (www.error != null) {
                ErrorMessage = www.error;
                yield break;
            }

            LastBody = www.text;
            LastResult = Deserialize( www.bytes );
        }

        abstract public byte[] Serialize(IDictionary<string, object> param);

        abstract public IDictionary<string, object> Deserialize(byte[] data);

        UTF8Encoding utf8enc = new UTF8Encoding();

        public byte[] BuildPostBody(IDictionary<string, object> param) {

            StringBuilder builder = new StringBuilder ();
            foreach (KeyValuePair<string, object> kv in param) {
                string key = kv.Key;
                if (kv.Value is IDictionary) {
                    builder.AppendFormat("&{0}={1}", kv.Key, Serialize(kv.Value as IDictionary<string, object>));
                } else if (kv.Value is IList) {
                    foreach (object v in kv.Value as List<object>) {
                        builder.AppendFormat("&{0}={1}", kv.Key, v);
                    }
                } else {
                    builder.AppendFormat("&{0}={1}", kv.Key, kv.Value.ToString());
                }
            }
            string str = builder.ToString();
            return utf8enc.GetBytes(str);
        }
    }
}

