using System.Reflection;
using UnityEngine;
using ZSerializer;

public partial class Settings : GlobalObject
{
    private static Settings _instance;
    public static Settings Instance => _instance ??= Get<Settings>();
    
    public static void Save() => ZSerialize.SaveGlobal(Instance);
    public static void Load() => ZSerialize.LoadGlobal(Instance);
}
