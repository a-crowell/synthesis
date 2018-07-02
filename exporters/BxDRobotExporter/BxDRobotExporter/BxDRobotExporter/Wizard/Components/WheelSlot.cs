﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BxDRobotExporter.Wizard
{
    /// <summary>
    /// <see cref="UserControl"/> added to a <see cref="FlowLayoutPanel"/> in the <see cref="DefineWheelsPage"/>
    /// </summary>
    public partial class WheelSlotPanel : UserControl
    {
        /// <summary>
        /// Field containing the <see cref="WheelSetupPanel"/> in this slot.
        /// </summary>
        private WheelSetupPanel wheelSetupPanel;
        public String name;

        public WheelSlotPanel()
        {
            InitializeComponent();
            IsFilled = false;
        }

        public void FillSlot(RigidNode_Base node, String name, bool isRight, WizardData.WizardWheelType wheelType = WizardData.WizardWheelType.NORMAL)
        {
            this.name = name;
            wheelSetupPanel = new WheelSetupPanel(node, name, wheelType);
            wheelSetupPanel.Dock = DockStyle.Fill;
            wheelSetupPanel.isRightWheel = isRight;
            this.SuspendLayout();
            while (Controls.Count > 0)
            {
                Controls[0].Dispose();
            }
            this.Controls.Add(wheelSetupPanel);
            wheelSetupPanel.Visible = true;
            this.ResumeLayout();

            wheelSetupPanel._WheelTypeChangedInternal += delegate () { OnWheelTypeChanged(); };

            IsFilled = true;
        }

        /// <summary>
        /// Fills the slot with a prexisting <see cref="WheelSetupPanel"/>. TODO: Use this for soft value saving when someone unchecks something.
        /// </summary>
        /// <param name="setupPanel"></param>
        public void FillSlot(WheelSetupPanel setupPanel)
        {
            wheelSetupPanel = setupPanel;
            wheelSetupPanel.Dock = DockStyle.Fill;
            this.Controls.Add(wheelSetupPanel);

            IsFilled = true;
        }

        /// <summary>
        /// Frees up the slot and sets <see cref="IsFilled"/> to false
        /// </summary>
        public void FreeSlot()
        {
            wheelSetupPanel.Dispose();
            InitializeComponent();

            IsFilled = false;
        }

        /// <summary>
        /// If the slot is filled, gets the <see cref="WizardData.WheelSetupData"/> via <see cref="WheelSetupPanel.GetWheelData()"/>
        /// </summary>
        public WizardData.WheelSetupData WheelData { get => IsFilled ? wheelSetupPanel.GetWheelData() : new WizardData.WheelSetupData(); }

        /// <summary>
        /// If the slot is filled, gets the <see cref="WizardData.WizardWheelType"/> from the panel via the <see cref="WheelSetupPanel.WheelType"/> property.
        /// </summary>
        public WizardData.WizardWheelType WheelType { get => IsFilled ? wheelSetupPanel.WheelType : 0; }

        /// <summary>
        /// Gets the <see cref="RigidNode_Base"/> from the <see cref="WheelSetupPanel"/> if it isn't null.
        /// </summary>
        public RigidNode_Base Node { get => wheelSetupPanel?.node; }

        /// <summary>
        /// Property used for filling the correct slots on the <see cref="DefineWheelsPage"/>
        /// </summary>
        public bool IsFilled { get; private set; }

        /// <summary>
        /// Event used for drive train verification in the parent <see cref="DefineWheelsPage"/>
        /// </summary>
        public event WheelTypeChangedEventHandler WheelTypeChanged;
        private void OnWheelTypeChanged()
        {
            WheelTypeChanged?.Invoke(this, new WheelTypeChangedEventArgs { NewWheelType = WheelType });
        }
    }
}
