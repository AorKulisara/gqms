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
public class DALException : Exception
{

    public DALException()
        : base()
    {
    }

    public DALException(string errorMessage)
        : base(errorMessage)
    {
    }

    public DALException(Exception ex)
        : base(ex.Message, ex)
    {
    }

    public DALException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
    }
}

