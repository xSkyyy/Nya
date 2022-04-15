﻿using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using HMUI;
using Nya.Configuration;
using Nya.UI.FlowControllers;
using Nya.UI.ViewControllers.SettingsControllers;
using Nya.Utils;
using Tweening;
using UnityEngine;

namespace Nya.UI.ViewControllers.ModalControllers
{
    internal class SettingsModalMenuController : SettingsModalController
    {
        private readonly MainFlowCoordinator _mainFlowCoordinator;
        private readonly NyaSettingsFlowCoordinator _nyaSettingsFlowCoordinator;
        private readonly NyaSettingsMainViewController _nyaSettingsMainViewController;

        public SettingsModalMenuController(UIUtils uiUtils, ImageUtils imageUtils, MainCamera mainCamera, PluginConfig pluginConfig, TimeTweeningManager timeTweeningManager, NsfwConfirmModalController nsfwConfirmModalController, MainFlowCoordinator mainFlowCoordinator, NyaSettingsFlowCoordinator nyaSettingsFlowCoordinator, NyaSettingsMainViewController nyaSettingsMainViewController)
            : base(uiUtils, imageUtils, mainCamera, pluginConfig, timeTweeningManager, nsfwConfirmModalController)
        {
            _mainFlowCoordinator = mainFlowCoordinator;
            _nyaSettingsFlowCoordinator = nyaSettingsFlowCoordinator;
            _nyaSettingsMainViewController = nyaSettingsMainViewController;
        }

        public void ShowModal(Transform parentTransform)
        {
            ShowModal(parentTransform, this);
            if (!PluginConfig.InMenu)
            {
                ScreenTab.IsVisible = false;
            }
        }

        [UIAction("show-nya-settings")]
        private void ShowNyaSettings()
        {
            HideModal();
            if (_nyaSettingsMainViewController.isActiveAndEnabled)
            {
                return;
            }

            _nyaSettingsMainViewController.parentFlowCoordinator = _mainFlowCoordinator.YoungestChildFlowCoordinatorOrSelf();
            _nyaSettingsMainViewController.parentFlowCoordinator.PresentFlowCoordinator(_nyaSettingsFlowCoordinator, animationDirection: ViewController.AnimationDirection.Vertical);
        }
    }
}