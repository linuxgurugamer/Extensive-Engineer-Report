using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JKorTech.Extensive_Engineer_Report
{
    public static class KSPExtensions
    {
        public static bool HasModule<M>(this Part part)
            where M : PartModule
        {
            return part.FindModuleImplementing<M>() != null;
        }

        public static bool AnyHasModule<M>(this IEnumerable<Part> parts)
            where M : PartModule
        {
            return parts.Any(part => part.HasModule<M>());
        }

        public static bool HasModule(this Part part, string moduleName)
        {
            return part.Modules.Contains(moduleName);
        }

        public static bool AnyHasModule(this IEnumerable<Part> parts, string moduleName)
        {
            return parts.Any(part => part.HasModule(moduleName));
        }

        public static IDictionary<ProtoCrewMember, Part> CrewInSection(IEnumerable<Part> sectionParts)
        {
            if (ShipConstruction.ShipManifest == null || ShipConstruction.ShipManifest.PartManifests == null || sectionParts == null)
                return new Dictionary<ProtoCrewMember, Part>();

            return ShipConstruction.ShipManifest.GetAllCrew(false).Where(crew => crew != null)
               .Select(crew => new KeyValuePair<ProtoCrewMember, Part>(crew, ShipConstruction.FindPartWithCraftID(ShipConstruction.ShipManifest.GetPartForCrew(crew).PartID)))
               .Where(pair => sectionParts.Contains(pair.Value)).ToDictionary(pair => pair.Key, pair => pair.Value);

        }

        public static bool IsProbeControlled(this IEnumerable<Part> sectionParts)
        {
            int antennaCnt = 0;
            foreach (var part in sectionParts)
            {
                var allpartmodules = part.FindModulesImplementing<ModuleDataTransmitter>();
                var datatransmittermodules = allpartmodules.OfType<ModuleDataTransmitter>().Where(p => p.antennaType != AntennaType.INTERNAL);
                antennaCnt += datatransmittermodules.Count();
            }
            
            return antennaCnt > 0 && !CrewInSection(sectionParts).Any(pair => pair.Value.HasModule<ModuleCommand>());
        //    return sectionParts.AnyHasModule<ModuleDeployableAntenna>() && !CrewInSection(sectionParts).Any(pair => pair.Value.HasModule<ModuleCommand>());

        }

        public static IEnumerable<T> GetScenarioModules<T>()
            where T : ScenarioModule
        {
            return ScenarioRunner.GetLoadedModules().OfType<T>();
        }
    }
}
