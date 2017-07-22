﻿using AutoMapper;
using FluentValidation;
using Maple.Core;
using Maple.Localization.Properties;
using System;

namespace Maple
{
    /// <summary>
    /// Provides logic to map between different domain objects of the Playlisttype
    /// </summary>
    /// <seealso cref="Maple.IPlaylistMapper" />
    public class PlaylistMapper : BaseMapper<Playlist>, IPlaylistMapper
    {
        private readonly IMediaItemMapper _mediaItemMapper;
        private readonly DialogViewModel _dialogViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistMapper"/> class.
        /// </summary>
        /// <param name="dialogViewModel">The dialog view model.</param>
        public PlaylistMapper(IMediaItemMapper mediaItemMapper, DialogViewModel dialogViewModel, ILocalizationService translator, ISequenceProvider sequenceProvider, IMapleLog log, IValidator<Playlist> validator)
            : base(translator, sequenceProvider, log, validator)
        {
            _dialogViewModel = dialogViewModel ?? throw new ArgumentNullException(nameof(dialogViewModel));
            _mediaItemMapper = mediaItemMapper ?? throw new ArgumentNullException(nameof(mediaItemMapper));

            InitializeMapper();
        }

        protected override void InitializeMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
            });

            config.AssertConfigurationIsValid();
            _mapper = config.CreateMapper();
        }

        public Playlist GetNewPlaylist(int sequence)
        {
            return new Playlist(_translationService, _mediaItemMapper, _sequenceProvider, _validator, _dialogViewModel, new Data.Playlist
            {
                Title = _translationService.Translate(nameof(Resources.New)),
                Description = string.Empty,
                Location = string.Empty,
                RepeatMode = 0,
                IsShuffeling = false,
                Sequence = sequence,
            });
        }

        public Playlist Get(Data.Playlist mediaitem)
        {
            return new Playlist(_translationService, _mediaItemMapper, _sequenceProvider, _validator, _dialogViewModel, mediaitem);
        }

        public Data.Playlist GetData(Playlist mediaitem)
        {
            return mediaitem.Model;
        }
    }
}
