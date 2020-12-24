//----auto generate----

 using System;
using  System.Collections.Generic;

namespace dataconfig{
public partial struct language 
{
    public static language DummyObj = new language();
    public static Func<IntPtr, language> Ctor = Creator;
    private IntPtr m_dataPtr;
    public language(IntPtr dataPtr)
    {
        m_dataPtr = dataPtr;

    }
    public static language Creator(IntPtr dataPtr)
    {
        return new language(dataPtr);
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
}

public class languageArray 
{
    public languageArray(List<language> data)
    {
        items = data;
    }

    public readonly List<language> items;
}
}