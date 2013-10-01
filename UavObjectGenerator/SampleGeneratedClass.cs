using System;
using System.IO;
using UavTalk;

namespace UavObjectGenerator
{
    /*

    <xml>
        <object name="Accels" singleinstance="true" settings="false">
            <description>The accelerometer sensor data, rotated into body frame.</description>
            <field name="x" units="m/s^2" type="float" elements="1"/>
            <field name="y" units="m/s^2" type="float" elements="1"/>
            <field name="z" units="m/s^2" type="float" elements="1"/>
            <field name="temperature" units="deg C" type="float" elements="1"/>
            <access gcs="readwrite" flight="readwrite"/>
            <telemetrygcs acked="false" updatemode="manual" period="0"/>
            <telemetryflight acked="false" updatemode="periodic" period="1000"/>
            <logging updatemode="manual" period="0"/>
        </object>
    </xml>


    */


    /*
     * This is the sample class that we pretend to generate for the XML above.
     */


    public class Accels: UavDataObject
    {

        public float X {
            get { return mX; }
            set    { mX = value; NotifyUpdated(); }
        }

        public float Y {
            get { return mY; }
            set    { mY = value; NotifyUpdated(); }
        }

        public float Z {
            get { return mZ; }
            set    { mZ = value; NotifyUpdated(); }
        }

        public float Temperature {
            get { return mTemperature; }
            set    { mTemperature = value; NotifyUpdated(); }
        }


        public Accels()
        {
        }


        public override UavDataObject Deserialize(BinaryReader stream)
        {
            Accels result = new Accels();
            result.mX = stream.ReadSingle();
            result.mY = stream.ReadSingle();
            result.mZ = stream.ReadSingle();
            result.mTemperature = stream.ReadSingle();

            return result;
        }

        public override void Serialize(BinaryWriter stream)
        {
            stream.Write(mX);
            stream.Write(mY);
            stream.Write(mZ);
            stream.Write(mTemperature);
        }


        private float mX;
        private float mY;
        private float mZ;
        private float mTemperature;
    }
}

