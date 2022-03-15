﻿using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using ScoreSaber.Core.Services;
using ScoreSaber.Extensions;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ScoreSaber.Core.ReplaySystem.UI {
    internal class ResultsViewReplayButtonController : IInitializable, IDisposable {
        [UIComponent("watch-replay-button")]
        protected readonly Button watchReplayButton = null;

        private ResultsViewController _resultsViewController;
        private IDifficultyBeatmap _difficultyBeatmap;
        private LevelCompletionResults _levelCompletionResults;
        private readonly PlayerService _playerService;
        private readonly ReplayLoader _replayLoader;

        public ResultsViewReplayButtonController(ResultsViewController resultsViewController, PlayerService playerService, ReplayLoader replayLoader) {

            _resultsViewController = resultsViewController;
            _playerService = playerService;
            _replayLoader = replayLoader;
        }

        public void Initialize() {

            _resultsViewController.didActivateEvent += ResultsViewController_didActivateEvent;
        }

        private void ResultsViewController_didActivateEvent(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling) {

            if (firstActivation) {
                BSMLParser.instance.Parse(
                    "<button-with-icon id=\"watch-replay-button\" icon=\"ScoreSaber.Resources.replay.png\" hover-hint=\"Watch Replay\" pref-width=\"15\" pref-height=\"13\" interactable=\"false\" on-click=\"replay-click\" />",
                    _resultsViewController.gameObject,
                    this
                );
                watchReplayButton.transform.localScale *= 0.4f;
                watchReplayButton.transform.localPosition = new Vector2(42.5f, 27f);
            }
            _difficultyBeatmap = Accessors.resultsViewControllerDifficultyBeatmap(ref _resultsViewController);
            _levelCompletionResults = Accessors.resultsViewControllerLevelCompletionResults(ref _resultsViewController);
            WaitForReplay().RunTask();
        }

        private async Task WaitForReplay() {

            await TaskEx.WaitUntil(() => Plugin.ReplayState.serializedReplay != null);
            watchReplayButton.interactable = true;
        }

        public void Dispose() {

            _resultsViewController.didActivateEvent -= ResultsViewController_didActivateEvent;
        }

        [UIAction("replay-click")]
        protected void ClickedReplayButton() {

            _replayLoader.Load(Plugin.ReplayState.serializedReplay, _difficultyBeatmap, _levelCompletionResults.gameplayModifiers, _playerService.localPlayerInfo.playerName).RunTask();
            watchReplayButton.interactable = false;
        }
    }
}