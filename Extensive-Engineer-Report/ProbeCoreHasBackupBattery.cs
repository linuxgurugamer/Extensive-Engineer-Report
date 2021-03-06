﻿using PreFlightTests;
using System.Collections.Generic;
using System.Linq;

namespace JKorTech.Extensive_Engineer_Report
{
    public class ProbeCoreHasBackupBattery : SectionDesignConcernBase
    {
        public override string GetConcernDescription()
        {
            return "This probe lacks a backup battery with ElectricCharge flow disabled.  Without careful attention, it might run out of power.";
        }

        public override string GetConcernTitle()
        {
            return "Probe Lacks Backup Battery";
        }

        public override DesignConcernSeverity GetSeverity()
        {
            return DesignConcernSeverity.WARNING;
        }

        protected internal override bool IsApplicable(IEnumerable<Part> sectionParts)
        {
            return sectionParts.IsProbeControlled();
        }

        public override bool TestCondition(IEnumerable<Part> sectionParts)
        {
            var backupBattery = sectionParts.Any(part =>
            {
                var electricCharge = part.Resources["ElectricCharge"];
                if (electricCharge == null) return false;
                var isBattery = electricCharge.amount > 0;
                var isNotCommandModule = !part.HasModule<ModuleCommand>();
                var flowDisabled = !electricCharge.flowState;
                return isBattery && isNotCommandModule && flowDisabled;
            });
            return backupBattery;
        }
    }
}
