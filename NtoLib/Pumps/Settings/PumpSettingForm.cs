﻿using System.Windows.Forms;

namespace NtoLib.Pumps.Settings
{
    public partial class PumpSettingForm : Form
    {
        private PumpControl _pumpControl;
        private PumpType _pumpType;



        public PumpSettingForm(PumpControl pumpControl)
        {
            _pumpControl = pumpControl;

            InitializeComponent();

            PumpFB pumpFB = _pumpControl.FBConnector.Fb as PumpFB;
            if(!pumpFB.UseNoConnectionLamp)
                flowLayoutPanel1.Controls.Remove(noConnectionLamp);

            if(!pumpFB.UseTemperatureLabel)
                flowLayoutPanel1.Controls.Remove(temperatureLabel);

            _pumpType = pumpFB.PumpType;
            if(_pumpType != PumpType.Turbine)
            {
                flowLayoutPanel1.Controls.Remove(speedLabel);
            }
            if(_pumpType != PumpType.Ion)
            {
                flowLayoutPanel1.Controls.Remove(safeModeLamp);

                flowLayoutPanel1.Controls.Remove(voltageLabel);
                flowLayoutPanel1.Controls.Remove(currentLabel);
                flowLayoutPanel1.Controls.Remove(powerLabel);
            }
            if(_pumpType != PumpType.Cryogen)
            {
                flowLayoutPanel1.Controls.Remove(temperatureInLabel);
                flowLayoutPanel1.Controls.Remove(temperatureOutLabel);
            }
        }



        protected override void OnPaint(PaintEventArgs e)
        {
            Status status = _pumpControl.Status;

            string state = string.Empty;
            if(!status.Use || !status.ConnectionOk)
                state = "нет данных";
            else if(status.Accelerating)
                state = "разгоняется";
            else if(status.Decelerating)
                state = "тормозит";
            else if(status.WorkOnNominalSpeed)
                state = "разогнан";
            else if(status.Stopped)
                state = "остановлен";
            stateLabel.Text = $"Состояние: {state}";

            temperatureLabel.ValueText =  $"{status.Temperature.ToString("F0")} C°";
                                                       
            switch(_pumpType)                          
            {                                          
                case PumpType.Forvacuum :              
                {                                      
                                                       
                    break;                             
                }                                      
                case PumpType.Turbine:
                {
                    string units = status.Units ? "%" : "об/мин";
                    speedLabel.ValueText =   $"{status.Speed.ToString("F1")} {units}";

                    break;                             
                }                                      
                case PumpType.Ion:                     
                {
                    safeModeLamp.Active = status.SafeMode;

                    voltageLabel.ValueText = $"{status.Voltage.ToString("F2")} В";
                    currentLabel.ValueText = $"{status.Current.ToString("F2")} А";
                    powerLabel.ValueText =   $"{status.Power.ToString("F2")} Вт";
                    break;                             
                }                                      
                case PumpType.Cryogen:                 
                {                                      
                    temperatureInLabel.ValueText =   $"{status.TemperatureIn.ToString("F2")} К";
                    temperatureOutLabel.ValueText =   $"{status.TemperatureOut.ToString("F2")} К";
                    break;
                }
            }


            forceStopLamp.Active = status.ForceStop;
            blockStartLamp.Active = status.BlockStart;
            blockStopLamp.Active = status.BlockStop;
            noConnectionLamp.Active = status.Use && !status.ConnectionOk;
            errorLamp.Active = status.Use && status.MainError;
            warningLamp.Active = status.Warning;

            base.OnPaint(e);
        }
    }
}
