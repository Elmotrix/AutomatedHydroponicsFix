using Assets.Scripts;
using Assets.Scripts.Objects.Electrical;
using Assets.Scripts.Objects.Items;
using Assets.Scripts.Objects.Motherboards;
using HarmonyLib;

namespace AutomatedHydroponicsFix
{

	[HarmonyPatch(typeof(HydroponicsAutomated), nameof(HydroponicsAutomated.OnHarvested))]
	public class HydroponicsAutomatedOnHarvested
	{
		[HarmonyPostfix]
		public static void Postfix(HydroponicsAutomated __instance)
		{
			Plant plant = __instance.Plant;
			if (GameManager.RunSimulation && (bool)plant)
			{
				if (plant.IsMature || plant.IsSeeding)
				{
					plant.Harvest(__instance, __instance.ExportSlot, plant.IsSeeding && plant.SeedQuantity > 0);
				}
				else if (GameManager.RunSimulation)
				{
					OnServer.Destroy(plant);
				}
			}
		}

	}

	[HarmonyPatch(typeof(HydroponicsAutomated), nameof(HydroponicsAutomated.CanLogicRead))]
	class HydroponicsAutomatedCanLogicRead
	{
		[HarmonyPostfix]
		public static bool Prefix(LogicSlotType logicSlotType, int slotId, HydroponicsAutomated __instance, ref bool __result)
		{
			if (logicSlotType == LogicSlotType.Seeding)
            {
				__result = slotId == __instance.PlantedSlot.SlotIndex;
				return false;
			}
			return true;
        }

	}
	[HarmonyPatch(typeof(HydroponicsAutomated), nameof(HydroponicsAutomated.GetLogicValue))]
	class HydroponicsAutomatedGetLogicValue
	{
		[HarmonyPostfix]
		public static bool Prefix(LogicSlotType logicSlotType, int slotId, HydroponicsAutomated __instance, ref double __result)
		{
			if (logicSlotType == LogicSlotType.Seeding)
			{
				if (!__instance.Plant || !__instance.Plant.IsSeeding)
				{
					__result = - 1.0;
				}
				else if (__instance.Plant.IsSeeding && __instance.Plant.SeedQuantity == 0)
				{
					__result = 0.0;
				}
                else
                {
					__result = 1.0;
                }
				return false;
			}
			return true;
		}

	}
}