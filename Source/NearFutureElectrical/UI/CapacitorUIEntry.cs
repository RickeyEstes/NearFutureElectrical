using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;
using KSP.Localization;
using NearFutureElectrical;

namespace NearFutureElectrical.UI
{

    public class CapacitorUIEntry
    {
      // Color of the temperature bar

      private DischargeCapacitorUI host;
      private DischargeCapacitor capacitor;

      // Constructor
      public CapacitorUIEntry(DischargeCapacitor toDraw, DischargeCapacitorUI uihost)
      {
          host = uihost;
         capacitor = toDraw;
      }

      // Draw the main control area
      private void DrawMainControls()
      {

        Rect controlRect = GUILayoutUtility.GetRect(100f, 32f);
        Rect iconRect = new Rect(0f, 0f, 32f, 32f);
        Rect titleRect = new Rect(34f, 0f, 68f, 32f);

        GUI.BeginGroup(controlRect);
        // STATIC: Icon
        GUI.DrawTextureWithTexCoords(iconRect, host.GUIResources.GetCapacitorIcon(0).iconAtlas, host.GUIResources.GetCapacitorIcon(0).iconRect);
        // STATIC: UI Name
        GUI.Label(titleRect, capacitor.part.partInfo.title, host.GUIResources.GetStyle("header_basic"));
        GUI.EndGroup();
      }

      // DrawControl bit
      private void DrawReadout()
      {
        Rect controlRect = GUILayoutUtility.GetRect(250f, 40f);

        Rect dischargeButtonRect = new Rect(0f, 0f, 36f, 36f);

        Rect dischargeRateIconRect = new Rect(40f, 0f, 20f, 20f);
        Rect dischargeRateSliderRect = new Rect(62f, 5f, 100f, 20f);
        Rect dischargeRateTextRect = new Rect(166f, 2f, 100f, 20f);

        Rect chargeBarIconRect = new Rect(40f, 20f, 20f, 20f);
        Rect chargeBarPanelRect = new Rect(62f, 20f, 100f, 20f);
        Rect chargeBarTextRect = new Rect(166f, 22f, 100f, 20f);

        GUI.BeginGroup(controlRect);

        // BUTTON: Discharge button
        GUI.color = host.GUIResources.GetColor("capacitor_blue");
        if (GUI.Button(dischargeButtonRect, ""))
        {
          capacitor.Discharge();
        }
        GUI.DrawTextureWithTexCoords(dischargeButtonRect, host.GUIResources.GetIcon("capacitor_discharge").iconAtlas, host.GUIResources.GetIcon("capacitor_discharge").iconRect);
        GUI.color = Color.white;

        // SLIDER: Discharge rate
        GUI.DrawTextureWithTexCoords(dischargeRateIconRect, host.GUIResources.GetIcon("capacitor_rate").iconAtlas, host.GUIResources.GetIcon("capacitor_rate").iconRect);
        capacitor.dischargeActual = GUI.HorizontalSlider(dischargeRateSliderRect, capacitor.dischargeActual, capacitor.DischargeRate*capacitor.DischargeRateMinimumScalar, capacitor.DischargeRate);
        GUI.Label(dischargeRateTextRect, String.Format("{0:F0} {1}", capacitor.dischargeActual,Localizer.Format("#LOC_NFElectrical_Units_EcS")), host.GUIResources.GetStyle("text_basic"));

        // PROGRESS BAR: Charge fraction bar
        GUI.DrawTextureWithTexCoords(chargeBarIconRect, host.GUIResources.GetIcon("capacitor_charge").iconAtlas, host.GUIResources.GetIcon("capacitor_charge").iconRect);

        Vector2 barBackgroundSize = new Vector2(100f, 10f);
        Vector2 barForegroundSize = new Vector2(Mathf.Max(barBackgroundSize.x * (GetChargePercent()/100.0f), 8f), 7f);

        Rect barBackgroundRect = new Rect(0f, 5f, barBackgroundSize.x, barBackgroundSize.y);
        Rect barForeroundRect = new Rect(0f, 6f, barForegroundSize.x, barForegroundSize.y);

        Color barColor = new Color();
            GUI.BeginGroup(chargeBarPanelRect);
            GUI.Box(barBackgroundRect, "", host.GUIResources.GetStyle("bar_background"));

            GUI.color = host.GUIResources.GetColor("capacitor_blue");
            if (GetChargePercent() <= 0.5f)
                GUI.color = Color.red;
            GUI.Box(barForeroundRect, "", host.GUIResources.GetStyle("bar_foreground"));
            GUI.color = Color.white;
         GUI.EndGroup();
         GUI.Label(chargeBarTextRect, String.Format("{0:F0}% ({1:F1} {2})", GetChargePercent(), GetCurrentRate(), Localizer.Format("#LOC_NFElectrical_Units_ScS")), host.GUIResources.GetStyle("text_basic"));
         GUI.EndGroup();
      }

      // Draw the basic control set
      private void DrawBasicControls()
      {
        Rect controlRect = GUILayoutUtility.GetRect(60f, 40f);
        Rect toggleRect = new Rect(22f, 3f, 20f, 20f);
        Rect iconRect = new Rect(0f, 8f, 20f, 20f);
        GUI.BeginGroup(controlRect);
        capacitor.Enabled = GUI.Toggle(toggleRect, capacitor.Enabled, "", host.GUIResources.GetStyle("button_toggle"));
        GUI.color = host.GUIResources.GetColor("readout_green");
        GUI.DrawTextureWithTexCoords(iconRect, host.GUIResources.GetIcon("capacitor_charging").iconAtlas, host.GUIResources.GetIcon("capacitor_charging").iconRect);
          GUI.color = Color.white;
        GUI.EndGroup();
      }

      // Draw the UI component
      public void Draw()
      {

          GUILayout.BeginHorizontal(host.GUIResources.GetStyle("block_background"), GUILayout.Width(445f));
        GUILayout.BeginHorizontal(host.GUIResources.GetStyle("item_box"));
        DrawMainControls();
        DrawReadout();
        DrawBasicControls();
        GUILayout.FlexibleSpace();
         GUILayout.EndHorizontal();
        GUILayout.EndHorizontal();
      }

      // Gets the current charge or discharge rate of a capacitor
      private float GetCurrentRate()
      {
          if (capacitor.Discharging)
          {
              return -capacitor.dischargeActual;
          } else if (capacitor.Enabled && capacitor.CurrentCharge < capacitor.MaximumCharge)
          {
              return capacitor.ChargeRate*capacitor.ChargeRatio;
          } else
          {
              return 0f;
          }
      }
      // Gets a capacitor's percent charge
      private float GetChargePercent()
      {
          return (capacitor.CurrentCharge / capacitor.MaximumCharge) *100f;
      }

    }

}
