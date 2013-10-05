using System;
using System.Text;

namespace UavObjectGenerator
{


    public class Hasher
    {
        //
        //void UAVObjectParser::calculateID(ObjectInfo* info)
        //{
        //    // Hash object name
        //    quint32 hash = updateHash(info->name, 0);
        //    // Hash object attributes
        //    hash = updateHash(info->isSettings, hash);
        //    hash = updateHash(info->isSingleInst, hash);
        //    // Hash field information
        //    for (int n = 0; n < info->fields.length(); ++n) {
        //        hash = updateHash(info->fields[n]->name, hash);
        //        hash = updateHash(info->fields[n]->numElements, hash);
        //        hash = updateHash(info->fields[n]->type, hash);
        //        if(info->fields[n]->type == FIELDTYPE_ENUM) {
        //            QStringList options = info->fields[n]->options;
        //            for (int m = 0; m < options.length(); m++)
        //                hash = updateHash(options[m], hash);
        //        }
        //    }
        //    // Done
        //    info->id = hash & 0xFFFFFFFE;
        //}

        public static UInt32 CalculateId(ObjectData obj)
        {
            UInt32 hash = UpdateHash(obj.Name, 0);
            hash = UpdateHash((UInt32)obj.IsSettingsInt, hash);
            hash = UpdateHash((UInt32)obj.IsSingleInstInt, hash);

            foreach (FieldData f in obj.Fields)
            {
                hash = UpdateHash(f.Name, hash);
                hash = UpdateHash((UInt32)f.NumElements, hash);
                hash = UpdateHash((UInt32)f.Type, hash);

                if (f.TypeString == "enum")
                {
                    foreach (string op in f.Options)
                    {
                        hash = UpdateHash(op, hash);
                    }
                }
            }

            return hash & 0xFFFFFFFE;
        }

        //
        ///**
        // * Shift-Add-XOR hash implementation. LSB is set to zero, it is reserved
        // * for the ID of the metaobject.
        // *
        // * http://eternallyconfuzzled.com/tuts/algorithms/jsw_tut_hashing.aspx
        // */
        //    quint32 UAVObjectParser::updateHash(quint32 value, quint32 hash)
        //    {
        //        return (hash ^ ((hash<<5) + (hash>>2) + value));
        //    }

        private static UInt32 UpdateHash(UInt32 val, UInt32 hash)
        {
            return (hash ^ ((hash << 5) + (hash >> 2) + val));
        }

        //
        //    /**
        // * Update the hash given a string
        // */
        //    quint32 UAVObjectParser::updateHash(QString& value, quint32 hash)
        //    {
        //        QByteArray bytes = value.toLatin1();
        //        quint32 hashout = hash;
        //        for (int n = 0; n < bytes.length(); ++n)
        //            hashout = updateHash(bytes[n], hashout);
        //
        //        return hashout;
        //    }
        //   

        private static UInt32 UpdateHash(string val, UInt32 hash)
        {
            // 28591 = ISO-8859-1 = latin1
            byte[] bytes = Encoding.GetEncoding(28591).GetBytes(val);
            UInt32 hashout = hash;
            for (int n = 0; n < bytes.Length; ++n)
            {
                hashout = UpdateHash(bytes[n], hashout);
            }

            return hashout;
        }
    }
}

