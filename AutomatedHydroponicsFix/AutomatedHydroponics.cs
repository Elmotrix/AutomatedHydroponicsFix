using Assets.Scripts;
using Assets.Scripts.Objects.Electrical;
using Assets.Scripts.Objects.Items;
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
}