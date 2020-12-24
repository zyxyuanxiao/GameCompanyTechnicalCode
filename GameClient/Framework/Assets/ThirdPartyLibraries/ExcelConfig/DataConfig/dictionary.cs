//----auto generate----

 using System;
using  System.Collections.Generic;

namespace dataconfig{
public partial struct dictionary 
{
    public static dictionary DummyObj = new dictionary();
    public static Func<IntPtr, dictionary> Ctor = Creator;
    private IntPtr m_dataPtr;
    public dictionary(IntPtr dataPtr)
    {
        m_dataPtr = dataPtr;

    }
    public static dictionary Creator(IntPtr dataPtr)
    {
        return new dictionary(dataPtr);
    }

    public bool Invalid()
    {
        return m_dataPtr == IntPtr.Zero;
    }

//unique key
    public string id
    {
        get
        {
            return GData.ReadString(m_dataPtr, 2);
        }
    }
    public string text
    {
        get
        {
            return GData.ReadString(m_dataPtr, 6);
        }
    }
    public string value
    {
        get
        {
            return GData.ReadString(m_dataPtr, 10);
        }
    }
}

public class dictionaryArray 
{
    public dictionaryArray(List<dictionary> data)
    {
        items = data;
    }

    public readonly List<dictionary> items;
}
}