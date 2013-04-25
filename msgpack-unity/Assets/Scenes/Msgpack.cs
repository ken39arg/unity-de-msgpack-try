using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MsgPack;

namespace chomechomo
{
    public class Msgpack : API 
    {
        BoxingPacker packer = new BoxingPacker();
        //ObjectPacker packer = new ObjectPacker();

        override public string Format {
            get {return "msgpack";}
        }

        override public byte[] Serialize(IDictionary<string, object> param) {
            return packer.Pack( param );
        }

        override public IDictionary<string, object> Deserialize(byte[] data) {
            IDictionary<string, object> dict = packer.Unpack( data ) as IDictionary<string, object>;
            return dict;
            //return dict as IDictionary<string, object>;
            //return packer.Unpack<IDictionary<string, object>>( data ) as IDictionary<string, object>;
        }
    }
}

