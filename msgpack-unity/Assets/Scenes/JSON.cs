using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MiniJSON;

namespace chomechomo
{
    public class JSON : API 
    {
        UTF8Encoding utf8enc = new UTF8Encoding();

        override public string Format {
            get {return "json";}
        }

        override public byte[] Serialize(IDictionary<string, object> param) {
            string str = Json.Serialize( param );
            return utf8enc.GetBytes(str);
            
        }

        override public IDictionary<string, object> Deserialize(byte[] data) {
            return Json.Deserialize( utf8enc.GetString(data) ) as Dictionary<string, object>;
        }

    }
}
