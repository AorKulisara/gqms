//*********************************************************
//  Copyright (c), Thaiintersol co,ltd. All rights reserved.
//**********************************************************




using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


public static class SerializeHelper
{

    public static string SerializeObject(object pObject)
    {
        MemoryStream MS = new MemoryStream();
        XmlSerializer XS = new XmlSerializer(pObject.GetType());
        XmlTextWriter XW = new XmlTextWriter(MS, Encoding.UTF8);
        System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
        string XmlString = null;

        XS.Serialize(XW, pObject);
        MS = (MemoryStream)XW.BaseStream;
        XmlString = UTF8.GetString(MS.ToArray());

        return XmlString;
    }

    public static void DeserializeObject(ref object pObject, string XmlString)
    {
        System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
        MemoryStream MS = new MemoryStream(UTF8.GetBytes(XmlString));
        XmlSerializer XS = new XmlSerializer(pObject.GetType());
        XmlTextWriter XW = new XmlTextWriter(MS, Encoding.UTF8);

        XS.Serialize(XW, pObject);
        MS = (MemoryStream)XW.BaseStream;
        XmlString = UTF8.GetString(MS.ToArray());
        pObject = XS.Deserialize(MS);
    }

    public static void SerializeFile(object pObject, string Filename)
    {
        XmlSerializer XS = new XmlSerializer(pObject.GetType());
        StreamWriter SW = null;
        string MapFilename = null;

        try
        {
            MapFilename = System.Web.HttpContext.Current.Server.MapPath(Filename);
            SW = new StreamWriter(MapFilename);
            XS.Serialize(SW, pObject);
        }
        catch (Exception ex)
        {
        }
        finally
        {
            if ((SW != null)) SW.Close();
            SW = null;
        }
    }

    public static void DeserializeFile( ref object pObject, string Filename)
    {
        XmlSerializer XS = new XmlSerializer(pObject.GetType());
        StreamReader SR = null;
        string MapFilename = "";

        try
        {
            MapFilename = HttpContext.Current.Server.MapPath(Filename);
            if (File.Exists(MapFilename))
            {
                SR = new StreamReader(MapFilename);
                pObject = XS.Deserialize(SR);
            }
        }
        catch (Exception ex) { }
        finally
        {
            if (SR != null) { SR.Close(); }
            SR = null;
        }
    }
}
