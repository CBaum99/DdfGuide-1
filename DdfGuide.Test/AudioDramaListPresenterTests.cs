﻿using System;
using System.Collections.Generic;
using System.Linq;
using DdfGuide.Core;
using DdfGuide.Core.Filtering;
using DdfGuide.Core.Sorting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace DdfGuide.Test
{
    [TestClass]
    public class AudioDramaListPresenterTests
    {
        private SampleAudioDramaProvider _sampleAudioDramaProvider;
        private List<AudioDrama> _audioDramas;
        private AutoMocker _mocker;
        private AudioDramaListPresenter _sut;

        [TestInitialize]
        public void Init()
        {
            _mocker = new AutoMocker();

            _sampleAudioDramaProvider = new SampleAudioDramaProvider();
            _audioDramas = _sampleAudioDramaProvider.Get().ToList();

            _mocker.Use<IEnumerable<AudioDrama>>(_audioDramas);

            _sut = _mocker.CreateInstance<AudioDramaListPresenter>();
            _sut.SetAudioDramas(_audioDramas);
        }
        

        [TestMethod]
        public void SetNewModel_UpdateAudioDramasOnView()
        {
            var listView = _mocker.GetMock<IAudioDramaListView>();
            listView.Invocations.Clear();

            _sut.SetAudioDramas(_audioDramas);

            listView.Verify(x => x.SetAudioDramaInfos(It.IsAny<IEnumerable<AudioDrama>>()), Times.Once);
        }

        [TestMethod]
        public void SetNewModel_UpdateSortInformationOnView()
        {
            var listView = _mocker.GetMock<IAudioDramaListView>();
            listView.Invocations.Clear();

            _sut.SetAudioDramas(_audioDramas);

            listView.Verify(x => x.SetSelectedSortMode(It.IsAny<EAudioDramaSortMode>()), Times.Once);
        }

        [TestMethod]
        public void SetNewModel_UpdateFilterInformationOnView()
        {
            var listView = _mocker.GetMock<IAudioDramaListView>();
            listView.Invocations.Clear();

            _sut.SetAudioDramas(_audioDramas);

            listView.Verify(x => x.SetFilterInfos(It.IsAny<EAudioDramaFilterMode>()), Times.Once);
        }


        [TestMethod]
        public void HeardChangedOnView_UpdateModel()
        {
            var listView = _mocker.GetMock<IAudioDramaListView>();

            Assert.IsFalse(_audioDramas.First().AudioDramaUserData.Heard);

            listView.Raise(x => x.HeardChanged += null, this, _audioDramas.First().AudioDramaUserData.Id);
            Assert.IsTrue(_audioDramas.First().AudioDramaUserData.Heard);

            listView.Raise(x => x.HeardChanged += null, this, _audioDramas.First().AudioDramaUserData.Id);
            Assert.IsFalse(_audioDramas.First().AudioDramaUserData.Heard);
        }

        [TestMethod]
        public void HeardChangedOnModel_UpdateAudioDramaOnView()
        {
            var listView = _mocker.GetMock<IAudioDramaListView>();

            listView.Invocations.Clear();

            var audioDrama = _audioDramas.First();
            var currentValue = audioDrama.AudioDramaUserData.Heard;
            audioDrama.AudioDramaUserData.Heard = !currentValue;
            
            listView.Verify(x => x.SetAudioDramaInfos(It.IsAny<IEnumerable<AudioDrama>>()), Times.Once);
        }

        [TestMethod]
        public void IsFavoriteChangedOnView_UpdateModel()
        {
            var listView = _mocker.GetMock<IAudioDramaListView>();

            Assert.IsFalse(_audioDramas.First().AudioDramaUserData.IsFavorite);

            listView.Raise(x => x.IsFavoriteChanged += null, this, _audioDramas.First().AudioDramaUserData.Id);
            Assert.IsTrue(_audioDramas.First().AudioDramaUserData.IsFavorite);

            listView.Raise(x => x.IsFavoriteChanged += null, this, _audioDramas.First().AudioDramaUserData.Id);
            Assert.IsFalse(_audioDramas.First().AudioDramaUserData.IsFavorite);
        }

        [TestMethod]
        public void IsFavoriteChangedOnModel_UpdateAudioDramasOnView()
        {
            var listView = _mocker.GetMock<IAudioDramaListView>();

            listView.Invocations.Clear();

            var audioDrama = _audioDramas.First();
            var currentValue = audioDrama.AudioDramaUserData.IsFavorite;
            audioDrama.AudioDramaUserData.IsFavorite = !currentValue;

            listView.Verify(x => x.SetAudioDramaInfos(It.IsAny<IEnumerable<AudioDrama>>()), Times.Once);
        }

        [TestMethod]
        public void AudioDramaClicked_OpenAudioDramaView()
        {
            var listView = _mocker.GetMock<IAudioDramaListView>();
            var viewer = _mocker.GetMock<IViewer>();
            var view = _mocker.GetMock<IAudioDramaView>();

            listView.Raise(x => x.AudioDramaClicked += null, this, _audioDramas.First().AudioDramaDto.Id);
            
            viewer.Verify(x => x.Show(view.Object), Times.Once);
        }

        [TestMethod]
        public void OrderByHeardFirstClicked_UpdateExplorer_UpdateAudioDramaOnView_UpdateSortModeOnView()
        {
            var listView = _mocker.GetMock<IAudioDramaListView>();
            var explorer = _mocker.GetMock<IAudioDramaExplorer>();

            listView.Invocations.Clear();
            explorer.Invocations.Clear();

            listView.Raise(x => x.OrderByHeardFirstClicked += null, this, EventArgs.Empty);
            
            explorer.Verify(x => x.SetSortMode(EAudioDramaSortMode.HeardFirst), Times.Once);
            listView.Verify(x => x.SetSelectedSortMode(EAudioDramaSortMode.HeardFirst), Times.Once);
            listView.Verify(x => x.SetAudioDramaInfos(It.IsAny<IEnumerable<AudioDrama>>()));
        }

        [TestMethod]
        public void OrderByHeardLastClicked_UpdateExplorer_UpdatAudioDramasOnView_UpdateSortModeOnView()
        {
            var listView = _mocker.GetMock<IAudioDramaListView>();
            var explorer = _mocker.GetMock<IAudioDramaExplorer>();

            listView.Invocations.Clear();
            explorer.Invocations.Clear();

            listView.Raise(x => x.OrderByHeardLastClicked += null, this, EventArgs.Empty);

            explorer.Verify(x => x.SetSortMode(EAudioDramaSortMode.HeardLast), Times.Once);
            listView.Verify(x => x.SetSelectedSortMode(EAudioDramaSortMode.HeardLast), Times.Once);
            listView.Verify(x => x.SetAudioDramaInfos(It.IsAny<IEnumerable<AudioDrama>>()));
        }

        [TestMethod]
        public void OrderByIsFavoriteFirstClicked_UpdateExplorer_UpdatAudioDramasOnView_UpdateSortModeOnView()
        {
            var listView = _mocker.GetMock<IAudioDramaListView>();
            var explorer = _mocker.GetMock<IAudioDramaExplorer>();

            listView.Invocations.Clear();
            explorer.Invocations.Clear();

            listView.Raise(x => x.OrderByIsFavoriteFirstClicked += null, this, EventArgs.Empty);

            explorer.Verify(x => x.SetSortMode(EAudioDramaSortMode.IsFavoriteFirst), Times.Once);
            listView.Verify(x => x.SetSelectedSortMode(EAudioDramaSortMode.IsFavoriteFirst), Times.Once);
            listView.Verify(x => x.SetAudioDramaInfos(It.IsAny<IEnumerable<AudioDrama>>()));
        }

        [TestMethod]
        public void OrderIsFavoriteLastClicked_UpdateExplorer_UpdatAudioDramasOnView_UpdateSortModeOnView()
        {
            var listView = _mocker.GetMock<IAudioDramaListView>();
            var explorer = _mocker.GetMock<IAudioDramaExplorer>();

            listView.Invocations.Clear();
            explorer.Invocations.Clear();

            listView.Raise(x => x.OrderByIsFavoriteLastClicked += null, this, EventArgs.Empty);

            explorer.Verify(x => x.SetSortMode(EAudioDramaSortMode.IsFavoriteLast), Times.Once);
            listView.Verify(x => x.SetSelectedSortMode(EAudioDramaSortMode.IsFavoriteLast), Times.Once);
            listView.Verify(x => x.SetAudioDramaInfos(It.IsAny<IEnumerable<AudioDrama>>()));
        }

        [TestMethod]
        public void OrderByNumberAscendingClicked_UpdateExplorer_UpdatAudioDramasOnView_UpdateSortModeOnView()
        {
            var listView = _mocker.GetMock<IAudioDramaListView>();
            var explorer = _mocker.GetMock<IAudioDramaExplorer>();

            listView.Invocations.Clear();
            explorer.Invocations.Clear();

            listView.Raise(x => x.OrderByNumberAscendingClicked += null, this, EventArgs.Empty);

            explorer.Verify(x => x.SetSortMode(EAudioDramaSortMode.NumberAscending), Times.Once);
            listView.Verify(x => x.SetSelectedSortMode(EAudioDramaSortMode.NumberAscending), Times.Once);
            listView.Verify(x => x.SetAudioDramaInfos(It.IsAny<IEnumerable<AudioDrama>>()));
        }

        [TestMethod]
        public void OrderByNumberDescendingClicked_UpdateExplorer_UpdatAudioDramasOnView_UpdateSortModeOnView()
        {
            var listView = _mocker.GetMock<IAudioDramaListView>();
            var explorer = _mocker.GetMock<IAudioDramaExplorer>();

            listView.Invocations.Clear();
            explorer.Invocations.Clear();

            listView.Raise(x => x.OrderByNumberDescendingClicked += null, this, EventArgs.Empty);

            explorer.Verify(x => x.SetSortMode(EAudioDramaSortMode.NumberDescending), Times.Once);
            listView.Verify(x => x.SetSelectedSortMode(EAudioDramaSortMode.NumberDescending), Times.Once);
            listView.Verify(x => x.SetAudioDramaInfos(It.IsAny<IEnumerable<AudioDrama>>()));
        }

        [TestMethod]
        public void OrderByReleaseDateAscendingClicked_UpdateExplorer_UpdatAudioDramasOnView_UpdateSortModeOnView()
        {
            var listView = _mocker.GetMock<IAudioDramaListView>();
            var explorer = _mocker.GetMock<IAudioDramaExplorer>();

            listView.Invocations.Clear();
            explorer.Invocations.Clear();

            listView.Raise(x => x.OrderByReleaseDateAscendingClicked += null, this, EventArgs.Empty);

            explorer.Verify(x => x.SetSortMode(EAudioDramaSortMode.ReleaseDateAscending), Times.Once);
            listView.Verify(x => x.SetSelectedSortMode(EAudioDramaSortMode.ReleaseDateAscending), Times.Once);
            listView.Verify(x => x.SetAudioDramaInfos(It.IsAny<IEnumerable<AudioDrama>>()));
        }

        [TestMethod]
        public void OrderByReleaseDateDescendingClicked_UpdateExplorer_UpdatAudioDramasOnView_UpdateSortModeOnView()
        {
            var listView = _mocker.GetMock<IAudioDramaListView>();
            var explorer = _mocker.GetMock<IAudioDramaExplorer>();

            listView.Invocations.Clear();
            explorer.Invocations.Clear();

            listView.Raise(x => x.OrderByReleaseDateDescendingClicked += null, this, EventArgs.Empty);

            explorer.Verify(x => x.SetSortMode(EAudioDramaSortMode.ReleaseDateDescending), Times.Once);
            listView.Verify(x => x.SetSelectedSortMode(EAudioDramaSortMode.ReleaseDateDescending), Times.Once);
            listView.Verify(x => x.SetAudioDramaInfos(It.IsAny<IEnumerable<AudioDrama>>()));
        }

        [TestMethod]
        public void OrderByNameAscendingClicked_UpdateExplorer_UpdatAudioDramasOnView_UpdateSortModeOnView()
        {
            var listView = _mocker.GetMock<IAudioDramaListView>();
            var explorer = _mocker.GetMock<IAudioDramaExplorer>();

            listView.Invocations.Clear();
            explorer.Invocations.Clear();

            listView.Raise(x => x.OrderByNameAscendingClicked += null, this, EventArgs.Empty);

            explorer.Verify(x => x.SetSortMode(EAudioDramaSortMode.NameAscending), Times.Once);
            listView.Verify(x => x.SetSelectedSortMode(EAudioDramaSortMode.NameAscending), Times.Once);
            listView.Verify(x => x.SetAudioDramaInfos(It.IsAny<IEnumerable<AudioDrama>>()));
        }

        [TestMethod]
        public void OrderByNameDescendingClicked_UpdateExplorer_UpdatAudioDramasOnView_UpdateSortModeOnView()
        {
            var listView = _mocker.GetMock<IAudioDramaListView>();
            var explorer = _mocker.GetMock<IAudioDramaExplorer>();

            listView.Invocations.Clear();
            explorer.Invocations.Clear();

            listView.Raise(x => x.OrderByNameDescendingClicked += null, this, EventArgs.Empty);

            explorer.Verify(x => x.SetSortMode(EAudioDramaSortMode.NameDescending), Times.Once);
            listView.Verify(x => x.SetSelectedSortMode(EAudioDramaSortMode.NameDescending), Times.Once);
            listView.Verify(x => x.SetAudioDramaInfos(It.IsAny<IEnumerable<AudioDrama>>()));
        }

        [TestMethod]
        public void MainAudioDramasOnlyClicked_UpdateExplorer_UpdatAudioDramasOnView_UpdateFilterModeOnView()
        {
            var listView = _mocker.GetMock<IAudioDramaListView>();
            var explorer = _mocker.GetMock<IAudioDramaExplorer>();

            listView.Invocations.Clear();
            explorer.Invocations.Clear();

            listView.Raise(x => x.MainsOnlyClicked += null, this, EventArgs.Empty);

            explorer.Verify(x => x.SetFilterMode(EAudioDramaFilterMode.MainsOnly), Times.Once);
            listView.Verify(x => x.SetFilterInfos(EAudioDramaFilterMode.MainsOnly), Times.Once);
            listView.Verify(x => x.SetAudioDramaInfos(It.IsAny<IEnumerable<AudioDrama>>()));
        }

        [TestMethod]
        public void AllAudioDramasClicked_UpdateExplorer_UpdatAudioDramasOnView_UpdateFilterModeOnView()
        {
            var listView = _mocker.GetMock<IAudioDramaListView>();
            var explorer = _mocker.GetMock<IAudioDramaExplorer>();

            listView.Invocations.Clear();
            explorer.Invocations.Clear();

            listView.Raise(x => x.AllClicked += null, this, EventArgs.Empty);

            explorer.Verify(x => x.SetFilterMode(EAudioDramaFilterMode.All), Times.Once);
            listView.Verify(x => x.SetFilterInfos(EAudioDramaFilterMode.All), Times.Once);
            listView.Verify(x => x.SetAudioDramaInfos(It.IsAny<IEnumerable<AudioDrama>>()));
        }

        [TestMethod]
        public void FavoritesOnlyClicked_UpdateExplorer_UpdatAudioDramasOnView_UpdateFilterModeOnView()
        {
            var listView = _mocker.GetMock<IAudioDramaListView>();
            var explorer = _mocker.GetMock<IAudioDramaExplorer>();

            listView.Invocations.Clear();
            explorer.Invocations.Clear();

            listView.Raise(x => x.FavoritesOnlyClicked += null, this, EventArgs.Empty);

            explorer.Verify(x => x.SetFilterMode(EAudioDramaFilterMode.FavoritesOnly), Times.Once);
            listView.Verify(x => x.SetFilterInfos(EAudioDramaFilterMode.FavoritesOnly), Times.Once);
            listView.Verify(x => x.SetAudioDramaInfos(It.IsAny<IEnumerable<AudioDrama>>()));
        }

        [TestMethod]
        public void UnheardOnlyClicked_UpdateExplorer_UpdatAudioDramasOnView_UpdateFilterModeOnView()
        {
            var listView = _mocker.GetMock<IAudioDramaListView>();
            var explorer = _mocker.GetMock<IAudioDramaExplorer>();

            listView.Invocations.Clear();
            explorer.Invocations.Clear();

            listView.Raise(x => x.UnheardOnlyClicked += null, this, EventArgs.Empty);

            explorer.Verify(x => x.SetFilterMode(EAudioDramaFilterMode.UnheardOnly), Times.Once);
            listView.Verify(x => x.SetFilterInfos(EAudioDramaFilterMode.UnheardOnly), Times.Once);
            listView.Verify(x => x.SetAudioDramaInfos(It.IsAny<IEnumerable<AudioDrama>>()));
        }

        [TestMethod]
        public void SpecialsOnlyClicked_UpdateExplorer_UpdatAudioDramasOnView_UpdateFilterModeOnView()
        {
            var listView = _mocker.GetMock<IAudioDramaListView>();
            var explorer = _mocker.GetMock<IAudioDramaExplorer>();

            listView.Invocations.Clear();
            explorer.Invocations.Clear();

            listView.Raise(x => x.SpecialsOnlyClicked += null, this, EventArgs.Empty);

            explorer.Verify(x => x.SetFilterMode(EAudioDramaFilterMode.SpecialsOnly), Times.Once);
            listView.Verify(x => x.SetFilterInfos(EAudioDramaFilterMode.SpecialsOnly), Times.Once);
            listView.Verify(x => x.SetAudioDramaInfos(It.IsAny<IEnumerable<AudioDrama>>()));
        }

        [TestMethod]
        public void SearchRequested_GetCurrentSearchText_UpdateExplorer_UpdateAudioDramasOnView()
        {
            var listView = _mocker.GetMock<IAudioDramaListView>();
            var explorer = _mocker.GetMock<IAudioDramaExplorer>();

            listView.Invocations.Clear();
            explorer.Invocations.Clear();

            listView.Setup(x => x.GetCurrentSearchText()).Returns("Homer Simpson");

            listView.Raise(x => x.SearchTextChanged += null, this, EventArgs.Empty);
            
            explorer.Verify(x => x.SetSearchText("Homer Simpson"), Times.Once);
            listView.Verify(x => x.SetAudioDramaInfos(It.IsAny<IEnumerable<AudioDrama>>()));
        }

        [TestMethod]
        public void RandomAudioDramaRequested_PickRandomFromExplorer_ShowAudioDramaView_SetRandomToView()
        {
            var listView = _mocker.GetMock<IAudioDramaListView>();
            var picker = _mocker.GetMock<IRandomAudioDramaPicker>();
            var viewer = _mocker.GetMock<IViewer>();
            var view = _mocker.GetMock<IAudioDramaView>();
            var explorer = _mocker.GetMock<IAudioDramaExplorer>();
            var audioDrama = _audioDramas.First();

            explorer.Setup(x => x.GetMatchingAudioDramas(It.IsAny<IEnumerable<AudioDrama>>())).Returns(_audioDramas);
            picker.Setup(x => x.Pick(It.IsAny<IEnumerable<AudioDrama>>())).Returns(audioDrama);

            listView.Raise(x => x.RandomClicked += null, this, EventArgs.Empty);

            picker.Verify(x => x.Pick(_audioDramas));
            viewer.Verify(x => x.Show(view.Object));
            _mocker.Verify<IAudioDramaPresenter>(x => x.SetAudioDrama(audioDrama), Times.Once);
        }

        [TestMethod]
        public void SetNewModelMultipleTimes_DontRaiseEventsMultipleTimes()
        {
            var view = _mocker.GetMock<IAudioDramaListView>();

            _sut.SetAudioDramas(_audioDramas);
            _sut.SetAudioDramas(_audioDramas);
            _sut.SetAudioDramas(_audioDramas);
            _sut.SetAudioDramas(_audioDramas);

            view.Invocations.Clear();

            view.Raise(x => x.HeardChanged += null, this, _audioDramas.First().AudioDramaDto.Id);

            view.Verify(x => x.SetAudioDramaInfos(It.IsAny<IEnumerable<AudioDrama>>()), Times.Once());
        }

        [TestMethod]
        public void OnBackClicked_ShowLastView()
        {
            var view = _mocker.GetMock<IAudioDramaListView>();
            view.Raise(x => x.BackClicked += null, this, EventArgs.Empty);

            _mocker.Verify<IViewer>(x => x.ShowLast());
        }

        [TestMethod]
        public void OnAudioDramaClicked_MakeSureViewGetsShownBeforeSettingAudioDramas()
        {
            var order = 1;
            _mocker.GetMock<IViewer>().Setup(x => x.Show(It.IsAny<IAudioDramaView>())).Callback(() =>
            {
                if (order++ != 1)
                {
                    Assert.Fail("View needs to be shown first.");
                }
            });

            _mocker.GetMock<IAudioDramaPresenter>().Setup(x => x.SetAudioDrama(It.IsAny<AudioDrama>())).Callback(() =>
            {
                if (order++ != 2)
                {
                    Assert.Fail("Audio drama can only be set AFTER the view is shown.");
                }
            });

            _mocker.GetMock<IAudioDramaListView>().Raise(x => x.AudioDramaClicked += null, this,
                _audioDramas.First().AudioDramaDto.Id);
        }

        [TestMethod]
        public void OnRandomClicked_MakeSureViewGetsShownBeforeSettingAudioDramas()
        {
            var order = 1;
            _mocker.GetMock<IViewer>().Setup(x => x.Show(It.IsAny<IAudioDramaView>())).Callback(() =>
            {
                if (order++ != 1)
                {
                    Assert.Fail("View needs to be shown first.");
                }
            });

            _mocker.GetMock<IAudioDramaPresenter>().Setup(x => x.SetAudioDrama(It.IsAny<AudioDrama>())).Callback(() =>
            {
                if (order++ != 2)
                {
                    Assert.Fail("Audio drama can only be set AFTER the view is shown.");
                }
            });

            _mocker.GetMock<IAudioDramaListView>().Raise(x => x.RandomClicked += null, this,
                EventArgs.Empty);
        }
    }
}
