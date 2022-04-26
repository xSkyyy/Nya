﻿using System;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.FloatingScreen;
using BeatSaberMarkupLanguage.GameplaySetup;
using HMUI;
using Nya.Configuration;
using Nya.UI.ViewControllers.ModalControllers;
using Nya.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Object = UnityEngine.Object;

namespace Nya.UI.ViewControllers.NyaViewControllers
{
    internal class NyaViewMenuController : NyaViewController, IInitializable, IDisposable
    {
        private readonly GameScenesManager _gameScenesManager;
        private readonly FloatingScreenUtils _floatingScreenUtils;
        private readonly SettingsModalMenuController _settingsModalMenuController;
        
        public NyaViewMenuController(PluginConfig pluginConfig, ImageUtils imageUtils, GameScenesManager gameScenesManager, FloatingScreenUtils floatingScreenUtils, SettingsModalMenuController settingsModalMenuController)
            : base(pluginConfig, imageUtils)
        {
            _gameScenesManager = gameScenesManager;
            _floatingScreenUtils = floatingScreenUtils;
            _settingsModalMenuController = settingsModalMenuController;
        }

        public void Initialize()
        {
            if (PluginConfig.InMenu)
            {
                if (_floatingScreenUtils.MenuFloatingScreen == null)
                {
                    _floatingScreenUtils.CreateNyaFloatingScreen(this, FloatingScreenUtils.FloatingScreenType.Menu);
                }

                _floatingScreenUtils.MenuFloatingScreen!.HandleReleased += FloatingScreen_HandleReleased;
            }
            else
            {
                GameplaySetup.instance.AddTab("Nya", "Nya.UI.Views.NyaView.bsml", this);
            }
            
            SceneManager.activeSceneChanged += SceneManagerOnactiveSceneChanged;
        }

        private void SceneManagerOnactiveSceneChanged(Scene currentScene, Scene nextScene)
        {
            if (nextScene.name == "MainMenu")
            {
                _gameScenesManager.transitionDidFinishEvent -= MenuActivated;
                _gameScenesManager.transitionDidFinishEvent += MenuActivated;
            }
            else
            {
                _gameScenesManager.transitionDidFinishEvent -= MenuDeactivated;
                _gameScenesManager.transitionDidFinishEvent += MenuDeactivated;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            
            if (GameplaySetup.IsSingletonAvailable)
            {
                GameplaySetup.instance.RemoveTab("Nya");
            }

            if (_floatingScreenUtils.MenuFloatingScreen != null)
            {
                _floatingScreenUtils.MenuFloatingScreen!.HandleReleased -= FloatingScreen_HandleReleased;
                Object.Destroy(_floatingScreenUtils.MenuFloatingScreen);
            }

            SceneManager.activeSceneChanged -= SceneManagerOnactiveSceneChanged;
        }

        private void MenuActivated(ScenesTransitionSetupDataSO transitionSetupData, DiContainer diContainer)
        {
            _gameScenesManager.transitionDidFinishEvent -= MenuActivated;
            
            if (_floatingScreenUtils.MenuFloatingScreen != null &&
                _floatingScreenUtils.MenuFloatingScreen.isActiveAndEnabled &&
                (_floatingScreenUtils.MenuFloatingScreen!.transform.position != PluginConfig.MenuPosition ||
                 _floatingScreenUtils.MenuFloatingScreen.transform.rotation.eulerAngles != PluginConfig.MenuRotation))
            {
                _floatingScreenUtils.MenuFloatingScreen.transform.position = PluginConfig.MenuPosition;
                _floatingScreenUtils.MenuFloatingScreen.transform.rotation = Quaternion.Euler(PluginConfig.MenuRotation);
            }

            NyaButton.interactable = false;
            ImageUtils.LoadCurrentNyaImage(NyaImage, () =>
            {
                NyaButton.interactable = true;
                if (ImageUtils.AutoNyaActive)
                {
                    AutoNyaToggle = false;
                    AutoNya();
                }
            });
        }
        
        private void MenuDeactivated(ScenesTransitionSetupDataSO transitionSetupData, DiContainer diContainer)
        {
            _gameScenesManager.transitionDidFinishEvent -= MenuDeactivated;
            
            if (AutoNyaToggle)
            {
                AutoNyaToggle = false;
                NyaAutoButton.gameObject.transform.Find("Underline").gameObject.GetComponent<ImageView>().color = new Color(1f, 1f, 1f, 0.502f);
                NyaButton.interactable = true;
            }
            
            _settingsModalMenuController.HideModal();
        }

        private void FloatingScreen_HandleReleased(object sender, FloatingScreenHandleEventArgs args)
        {
            var transform = _floatingScreenUtils.MenuFloatingScreen!.transform;
            PluginConfig.MenuPosition = transform.position;
            PluginConfig.MenuRotation = transform.eulerAngles;
        }

        [UIAction("settings-button-clicked")]
        protected void SettingsButtonClicked()
        {
            if (AutoNyaToggle)
            {
                AutoNya();
            }

            _settingsModalMenuController.ShowModal(SettingsButtonTransform);
        }
    }
}