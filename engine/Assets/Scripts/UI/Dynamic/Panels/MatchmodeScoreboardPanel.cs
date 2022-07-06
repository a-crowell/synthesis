using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace Synthesis.UI.Dynamic {
    public class MatchmodeScoreboardPanel : PanelDynamic {

        public const string ROBOTO_BOLD = "Roboto-Bold SDF";
        public const string ROBOTO_REGULAR = "Roboto-Regular SDF";

        public MatchmodeScoreboardPanel() : base(new Vector2(200, 100)) { }

        private Label time, redScore, blueScore;

        public override void Create() {

            CancelButton.RootGameObject.SetActive(false);
            AcceptButton.StepIntoLabel(l => l.SetText("Close"));
            AcceptButton.AddOnClickedEvent(b => DynamicUIManager.CloseActivePanel());

            var normalFont = SynthesisAssetCollection.GetFont(ROBOTO_REGULAR);
            Func<Label, Label> nonHighlightedLabel =
                l => l.SetFont(normalFont).SetFontSize(14).SetVerticalAlignment(VerticalAlignmentOptions.Middle).SetHorizontalAlignment(HorizontalAlignmentOptions.Left);

            MainContent.CreateLabel(15f).ApplyTemplate(nonHighlightedLabel).SetTopStretch().SetText("Position");
            time = MainContent.CreateLabel(15f).ApplyTemplate(nonHighlightedLabel).SetTopStretch(leftPadding: 10f, anchoredY: 15f).SetText("Time Remaining: " + targetTime);
            redScore = MainContent.CreateLabel(15f).ApplyTemplate(nonHighlightedLabel).SetTopStretch(leftPadding: 10f, anchoredY: 30f).SetText("Red: 0");
            blueScore = MainContent.CreateLabel(15f).ApplyTemplate(nonHighlightedLabel).SetTopStretch(leftPadding: 10f, anchoredY: 45f).SetText("Blue: 0");
        }
        float targetTime = 135;
        public override void Update() {
            Vector3 robotPosition = new Vector3();
            if (RobotSimObject.CurrentlyPossessedRobot != string.Empty) {
                robotPosition = RobotSimObject.GetCurrentlyPossessedRobot().GroundedNode.transform.position;
            }

            targetTime -= Time.deltaTime;
            if (targetTime >= 0) time.SetText($"Time Remaining: {Mathf.RoundToInt(targetTime).ToString()}");
            else
            {
                
            }
            
        }

        public override void Delete() { }
    }
}