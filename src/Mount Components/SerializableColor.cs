using UnityEngine;

[System.Serializable]
public class SerializableColor
{
    public float r, g, b, a;

    public SerializableColor()
    {
    }

    public SerializableColor(float r, float g, float b, float a)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    public static implicit operator Color(SerializableColor sc)
    {
        return new Color(sc.r, sc.g, sc.b, sc.a);
    }

    public static implicit operator SerializableColor(Color c)
    {
        return new SerializableColor(c.r, c.g, c.b, c.a);
    }
}