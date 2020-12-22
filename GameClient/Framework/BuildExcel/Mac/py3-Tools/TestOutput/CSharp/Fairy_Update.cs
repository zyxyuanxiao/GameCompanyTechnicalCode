using System;

public class Fairy_Update 
{
    private int m_baseOffset = 0;
    private IntPtr m_dataPtr;
    private IntPtr m_configPtr;

    public Fairy_Update(IntPtr dataPtr, IntPtr configPtr)
    {
        m_configPtr = configPtr;
        m_dataPtr = dataPtr;

    }
    public UInt32 FairyClass
    {
        get
        {
            return GData.ReadUInt32(m_dataPtr, m_baseOffset + 0);
        }
    }
    public UInt32 Star
    {
        get
        {
            return GData.ReadUInt32(m_dataPtr, m_baseOffset + 4);
        }
    }
    public UInt32 CostItemID
    {
        get
        {
            return GData.ReadUInt32(m_dataPtr, m_baseOffset + 24);
        }
    }
}
