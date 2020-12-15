using System.IO;
using UnityEngine;

public class PersistentStorage : MonoBehaviour
{
    string savePath;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "saveFile");
    }

    public void Save(PersistableObject c, int version)
    {
        using(
            var writer = new BinaryWriter(File.Open(savePath,FileMode.Create))
        )
        {
            writer.Write(-version);
            c.Save(new GameDataWriter(writer));
        }
    }

    public void Load(PersistableObject c)
    {
        using (
            var reader = new BinaryReader(File.Open(savePath, FileMode.Open))
        )
        {
            c.Load(new GameDataReader(reader, -reader.ReadInt32()));
        }
    }
}
