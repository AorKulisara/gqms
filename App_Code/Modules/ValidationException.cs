//*********************************************************
//  Copyright (c), Thaiintersol co,ltd. All rights reserved.
//**********************************************************




using System;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

[Serializable()] 
public class ValidationException:Exception
{
    public void Exception()
    {
        //base.Exception();
    }

    public void Exception(string errorMessage)
    {
        //new Exception(errorMessage);
    }

    public void Exception(Exception ex)
    {
        //new Exception(ex.Message, ex);
    }

    public void Exception(SerializationInfo info, StreamingContext context)
    {
        //New(info, context);
    }

    public override void GetObjectData(SerializationInfo info ,StreamingContext context){
        GetObjectData(info, context);
    }
}
