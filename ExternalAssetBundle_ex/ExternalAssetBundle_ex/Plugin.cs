using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Utility;

using System.Reflection;
using UnityEngine;

namespace ExternalAssetBundle_ex
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Plugin : BaseUnityPlugin
    {
        private static Assembly ModAssembly { get; } = Assembly.GetExecutingAssembly();
        public new static ManualLogSource Logger { get; private set; }
        public static AssetBundle AssetBundle { get; private set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
        private void Awake()
        {
            Logger = base.Logger;
            AssetBundle = AssetBundleLoadingUtils.LoadFromAssetsFolder(ModAssembly, "myassetbundle");
            Harmony.CreateAndPatchAll(ModAssembly, $"{PluginInfo.PLUGIN_GUID}");
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            Register();
        }

        public static PrefabInfo MyPrefabInfo { get; } = PrefabInfo
            .WithTechType("example_classid", "example (display name)", "example (description)")
            .WithIcon(SpriteManager.Get(TechType.HeatBlade));

        public static void Register()
        {
            CustomPrefab prefab = new(MyPrefabInfo);
            prefab.SetGameObject(GetAssetBundlePrefab());
            prefab.SetPdaGroupCategory(TechGroup.BasePieces, TechCategory.BasePiece);
            prefab.SetRecipe(new RecipeData(new CraftData.Ingredient(TechType.Titanium, 1)));

            prefab.Register();
        }

        private static GameObject GetAssetBundlePrefab()
        {
            GameObject obj = AssetBundle.LoadAsset<GameObject>("ExamplePrefab");
            PrefabUtils.AddBasicComponents(obj, MyPrefabInfo.ClassID, MyPrefabInfo.TechType, LargeWorldEntity.CellLevel.Medium);
            ConstructableFlags constructableFlags = ConstructableFlags.Outside | ConstructableFlags.AllowedOnConstructable | ConstructableFlags.Base | ConstructableFlags.Ceiling | ConstructableFlags.Default | ConstructableFlags.Outside | ConstructableFlags.Rotatable | ConstructableFlags.Wall;
            MaterialUtils.ApplySNShaders(obj);
            GameObject model = obj.transform.Find("Cube").gameObject;

            PrefabUtils.AddConstructable(
                obj,
                MyPrefabInfo.TechType,
                constructableFlags,
                model);

            return obj;
        }
    }
}