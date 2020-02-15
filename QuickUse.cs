using System;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;
using Harmony12;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.UI.ServiceWindow;
using static QuickUse.Main;

// ReSharper disable InconsistentNaming

namespace QuickUse
{
    static class Main
    {
        private static bool OnToggle(UnityModManager.ModEntry mod_entry, bool value)
        {
            enabled = value;
            return true;
        }

        internal static bool enabled;

        internal static HarmonyInstance harmony = HarmonyInstance.Create("ca.gnivler.kingmaker.QuickUse");

        static void Load(UnityModManager.ModEntry mod_entry)
        {
            mod_entry.OnToggle = OnToggle;
            Patches.Init();
            Log("Startup");
        }

        internal static void Log(object input)
        {
            //FileLog.Log($"[QuickUse] {input}");
        }
    }
}

static class Patches
{
    internal static void Init()
    {
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    [HarmonyPatch(typeof(InventorySlotsController), "HandleSlotClick")]
    public static class InventorySlotsController_HandleSlotClick_Patch
    {
        public static bool Prefix(ItemSlot slot)
        {
            Log("HandleSlotClick");
            var shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            if (shift)
            {
                Log("QuickUse triggered " + slot.Item.Name);
                QuickUse(slot);
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(InventorySlotsController), "HandleSlotDoubleClick")]
    public static class InventorySlotsController_HandleSlotDoubleClick_Patch
    {
        public static bool Prefix(ItemSlot slot)
        {
            Log("HandleSlotDoubleClick");
            var shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            if (shift)
            {
                Log("QuickUse triggered " + slot.Item.Name);
                QuickUse(slot);
                return false;
            }

            return true;
        }
    }

    private static void QuickUse(ItemSlot slot)
    {
        try
        {
            var itemType = ((BlueprintItemEquipmentUsable) slot.Item.Blueprint).Type;
            if (itemType == UsableItemType.Scroll ||
                itemType == UsableItemType.Potion)
            {
                Log("Using item " + slot.Item.Name);
                slot.UseItem();
                slot.UpdateCount();
            }
        }
        catch (Exception ex)
        {
            Log(ex);
        }
    }
}
