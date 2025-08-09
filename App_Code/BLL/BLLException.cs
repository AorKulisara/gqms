//*********************************************************
//  Copyright (c), Thaiintersol co,ltd. All rights reserved.
//**********************************************************


using System;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Linq;
using System.Web;


[Serializable()]
public class BLLException : Exception
{

    public BLLException()
        : base()
    {
    }

    public BLLException(string errorMessage)
        : base(errorMessage)
    {
    }

    public BLLException(Exception ex)
        : base(ex.Message, ex)
    {
    }

    public BLLException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
    }
}

