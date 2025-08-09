//*********************************************************
//  Copyright (c), Thaiintersol co,ltd. All rights reserved.
//**********************************************************




using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Web.Services.Protocols;

[Serializable()]
public class SharedException:Exception
{
    public void Exception()
    {
        Exception();
    }

    public void Exception(string errormessage)
    {
        Exception(errormessage);
    }

    public void Exception(SerializationInfo info, StreamingContext context)
    {
        Exception(info, context);
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context) {
        GetObjectData(info, context);
    }

}
public class ApplicationError : SoapHeader {
    public string Message;
}
