using System.Reflection;
using UnityEngine;
using ZSerializer;

public partial class ProgressManager : GlobalObject
{
    private static ProgressManager _instance;
    public static ProgressManager Instance => _instance ??= Get<ProgressManager>();
    
    public static void Save() => ZSerialize.SaveGlobal(Instance);
    public static void Load() => ZSerialize.LoadGlobal(Instance);
}
