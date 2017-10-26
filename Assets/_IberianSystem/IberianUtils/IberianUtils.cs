using UnityEngine;
using System.Collections;
using System.IO;
using System;

public static class IberianUtils
{

    public static long CalculateEpochMillis(DateTime dateTime)
    {
        DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        return (long)(dateTime - epochStart).TotalMilliseconds;
    }

    public static byte[] SerializeObject(System.Object serializableObject, Type type)
    {
        //T obj = serializableObject;

        using (MemoryStream stream = new MemoryStream())
        {
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(type);

            x.Serialize(stream, serializableObject);

            return stream.ToArray();
        }
    }

    public static System.Object DeserializeObject(byte[] serilizedBytes, Type type)
    {
        Debug.Log("DeserializeObject" + type);
        System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(type);

        using (MemoryStream stream = new MemoryStream(serilizedBytes))
        {
            return x.Deserialize(stream);
        }
    }

    public static Sprite CreateSpriteFromTexture(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    public static IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    
}
