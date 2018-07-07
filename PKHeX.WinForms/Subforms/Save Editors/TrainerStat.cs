﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Subforms.Save_Editors
{
    public partial class TrainerStat : UserControl
    {
        public TrainerStat() => InitializeComponent();
        private readonly ToolTip Tip = new ToolTip();
        private bool Editing;
        private ITrainerStatRecord SAV;
        private Dictionary<int, string> RecordList; // index, description
        public Func<int, string> GetToolTipText { private get; set; }

        public void LoadRecords(ITrainerStatRecord sav, Dictionary<int, string> records)
        {
            SAV = sav;
            RecordList = records;
            CB_Stats.Items.Clear();
            for (int i = 0; i < 200; i++)
            {
                if (!RecordList.TryGetValue(i, out string name))
                    name = $"{i:D3}";

                CB_Stats.Items.Add(name);
            }
            CB_Stats.SelectedIndex = RecordList.First().Key;
        }

        private void ChangeStat(object sender, EventArgs e)
        {
            Editing = true;
            int index = CB_Stats.SelectedIndex;
            NUD_Stat.Maximum = SAV.GetRecordMax(index);
            NUD_Stat.Value = SAV.GetRecord(index);

            int offset = SAV.GetRecordOffset(index);
            L_Offset.Text = $"Offset: 0x{offset:X3}";
            UpdateTip(index, true);
            Editing = false;
        }

        private void ChangeStatVal(object sender, EventArgs e)
        {
            if (Editing)
                return;
            int index = CB_Stats.SelectedIndex;
            SAV.SetRecord(index, (int)NUD_Stat.Value);
            UpdateTip(index, false);
        }

        private void UpdateTip(int index, bool updateStats)
        {
            if (GetToolTipText != null)
                UpdateToolTipSpecial(index, updateStats);
            else
                UpdateToolTipDefault(index, updateStats);
        }

        private void UpdateToolTipSpecial(int index, bool updateStats)
        {
            var str = GetToolTipText(index);
            if (str != null)
            {
                Tip.SetToolTip(NUD_Stat, str);
                return;
            }
            UpdateToolTipDefault(index, updateStats); // fallback
        }

        private void UpdateToolTipDefault(int index, bool updateStats)
        {
            if (!updateStats || !RecordList.TryGetValue(index, out string tip))
            {
                Tip.RemoveAll();
                return;
            }
            Tip.SetToolTip(CB_Stats, tip);
        }
    }
}