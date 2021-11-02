﻿using AutoDarkModeConfig;
using AutoDarkModeConfig.ComponentSettings.Base;
using AutoDarkModeSvc.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDarkModeSvc.SwitchComponents.Base
{
    class ScriptSwitch : BaseComponent<ScriptSwitchSettings>
    {
        private Theme currentComponentTheme = Theme.Unknown;

        public override bool ThemeHandlerCompatibility { get; } = true;

        public override bool ComponentNeedsUpdate(Theme newTheme)
        {
            if (currentComponentTheme != newTheme)
            {
                return true;
            }
            return false;
        }

        protected override async void HandleSwitch(Theme newTheme)
        {
            string oldTheme = Enum.GetName(typeof(Theme), currentComponentTheme);
            Task switchTask = Task.Run(() =>
            {
                if (newTheme == Theme.Light)
                {
                    Settings.Component.Scripts.ForEach(s =>
                    {
                        ScriptHandler.Launch(s.Name, s.Command, s.ArgsLight);
                    });
                    currentComponentTheme = Theme.Light;
                }
                else
                {
                    Settings.Component.Scripts.ForEach(s =>
                    {
                        ScriptHandler.Launch(s.Name, s.Command, s.ArgsDark);
                    });
                    currentComponentTheme = Theme.Dark;
                }
            });
            await switchTask;           
            Logger.Info($"update info - previous: {oldTheme}, now: {Enum.GetName(typeof(Theme), currentComponentTheme)}");
        }

        public override void EnableHook()
        {
            currentComponentTheme = Theme.Unknown;
            base.EnableHook();
        }

        public override void DisableHook()
        {
            currentComponentTheme = Theme.Unknown;
            base.DisableHook();
        }

        public override void UpdateSettingsState(object newSettings)
        {
            bool isInit = Settings == null;
            base.UpdateSettingsState(newSettings);
            if (isInit) return;
            if (!Settings.Component.Equals(SettingsBefore))
            {
                currentComponentTheme = Theme.Unknown;
            }
        }
    }
}
