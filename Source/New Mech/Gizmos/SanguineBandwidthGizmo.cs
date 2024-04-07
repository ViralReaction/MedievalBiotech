using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace MedievalBiotech
{
    [StaticConstructorOnStartup]
    public class SanguineBandwidthGizmo : Gizmo
    {
        public override bool Visible
        {
            get
            {
                return Find.Selector.SelectedPawns.Count == 1;
            }
        }

        public SanguineBandwidthGizmo(Pawn_MechanitorTracker tracker)
        {
            this.tracker = tracker;
            this.Order = -90f;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            if (!ModLister.CheckBiotech("Mechanitor bandwidth gizmo"))
            {
                return new GizmoResult(GizmoState.Clear);
            }
            Rect rect = new Rect(topLeft.x, topLeft.y, this.GetWidth(maxWidth), 75f);
            Rect rect2 = rect.ContractedBy(6f);
            Widgets.DrawWindowBackground(rect);
            int totalBandwidth = this.tracker.TotalBandwidth;
            int usedBandwidth = this.tracker.UsedBandwidth;
            string text = usedBandwidth.ToString("F0") + " / " + totalBandwidth.ToString("F0");
            TaggedString taggedString = "Bandwidth".Translate().Colorize(ColoredText.TipSectionTitleColor) + ": " + text + "\n\n" + "BandwidthGizmoTip".Translate();
            int usedBandwidthFromSubjects = this.tracker.UsedBandwidthFromSubjects;
            if (usedBandwidthFromSubjects > 0)
            {
                taggedString += "\n\n" + ("BandwidthUsage".Translate() + ": ") + usedBandwidthFromSubjects;
                IEnumerable<string> entries = (from p in this.tracker.OverseenPawns
                                               where !p.IsGestating()
                                               group p by p.kindDef).Select(delegate (IGrouping<PawnKindDef, Pawn> p)
                                               {
                                                   object[] array = new object[5];
                                                   array[0] = p.Key.LabelCap + " x";
                                                   array[1] = p.Count<Pawn>();
                                                   array[2] = " (+";
                                                   array[3] = p.Sum((Pawn mech) => mech.GetStatValue(StatDefOf.BandwidthCost, true, -1));
                                                   array[4] = ")";
                                                   return string.Concat(array);
                                               });
                taggedString += "\n\n" + entries.ToLineList(" - ", false);
            }
            int usedBandwidthFromGestation = this.tracker.UsedBandwidthFromGestation;
            if (usedBandwidthFromGestation > 0)
            {
                taggedString += "\n\n" + "MechGestationBandwidthUsed".Translate() + ": " + usedBandwidthFromGestation;
                IEnumerable<string> entries2 = (from p in this.tracker.OverseenPawns
                                                where p.IsGestating()
                                                group p by p.kindDef).Select(delegate (IGrouping<PawnKindDef, Pawn> p)
                                                {
                                                    object[] array = new object[5];
                                                    array[0] = p.Key.LabelCap + " x";
                                                    array[1] = p.Count<Pawn>();
                                                    array[2] = " (+";
                                                    array[3] = p.Sum((Pawn mech) => mech.GetStatValue(StatDefOf.BandwidthCost, true, -1));
                                                    array[4] = ")";
                                                    return string.Concat(array);
                                                });
                taggedString += "\n\n" + entries2.ToLineList(" - ", false);
            }
            TooltipHandler.TipRegion(rect, taggedString);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect rect3 = new Rect(rect2.x, rect2.y, rect2.width, 20f);
            Widgets.Label(rect3, "Bandwidth".Translate());
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperRight;
            Widgets.Label(rect3, text);
            Text.Anchor = TextAnchor.UpperLeft;
            int num = Mathf.Max(usedBandwidth, totalBandwidth);
            Rect rect4 = new Rect(rect2.x, rect3.yMax + 6f, rect2.width, rect2.height - rect3.height - 6f);
            int num2 = 2;
            int num3 = Mathf.FloorToInt(rect4.height / (float)num2);
            int num4 = Mathf.FloorToInt(rect4.width / (float)num3);
            int num5 = 0;
            while (num2 * num4 < num)
            {
                num2++;
                num3 = Mathf.FloorToInt(rect4.height / (float)num2);
                num4 = Mathf.FloorToInt(rect4.width / (float)num3);
                num5++;
                if (num5 >= 1000)
                {
                    Log.Error("Failed to fit bandwidth cells into gizmo rect.");
                    return new GizmoResult(GizmoState.Clear);
                }
            }
            int num6 = Mathf.FloorToInt(rect4.width / (float)num3);
            int num7 = num2;
            float num8 = (rect4.width - (float)(num6 * num3)) / 2f;
            int num9 = 0;
            int usedBandwidthFromGestation2 = this.tracker.UsedBandwidthFromGestation;
            int num10 = (num7 <= 2) ? 4 : 2;
            for (int i = 0; i < num7; i++)
            {
                for (int j = 0; j < num6; j++)
                {
                    num9++;
                    Rect rect5 = new Rect(rect4.x + (float)(j * num3) + num8, rect4.y + (float)(i * num3), (float)num3, (float)num3).ContractedBy(2f);
                    if (num9 <= num)
                    {
                        if (num9 <= usedBandwidthFromGestation2)
                        {
                            Widgets.DrawRectFast(rect5, SanguineBandwidthGizmo.EmptyBlockColor, null);
                            Widgets.DrawRectFast(rect5.ContractedBy((float)num10), SanguineBandwidthGizmo.FilledBlockColor, null);
                        }
                        else if (num9 <= usedBandwidth)
                        {
                            Widgets.DrawRectFast(rect5, (num9 <= totalBandwidth) ? SanguineBandwidthGizmo.FilledBlockColor : SanguineBandwidthGizmo.ExcessBlockColor, null);
                        }
                        else
                        {
                            Widgets.DrawRectFast(rect5, SanguineBandwidthGizmo.EmptyBlockColor, null);
                        }
                    }
                }
            }
            return new GizmoResult(GizmoState.Clear);
        }

        public override float GetWidth(float maxWidth)
        {
            return 136f;
        }

        public const int InRectPadding = 6;
        private const int CellPadding = 2;
        private const float Width = 136f;
        private const int StartingBandwidthRows = 2;
        private static readonly Color EmptyBlockColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        private static readonly Color FilledBlockColor = ColorLibrary.Green;
        private static readonly Color ExcessBlockColor = ColorLibrary.Red;
        private const int HeaderHeight = 20;
        private Pawn_MechanitorTracker tracker;
    }
}
