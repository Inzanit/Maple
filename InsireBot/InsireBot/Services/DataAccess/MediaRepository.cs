﻿using Maple.Core;
using Maple.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maple
{
    // helpful: https://blog.oneunicorn.com/2012/03/10/secrets-of-detectchanges-part-1-what-does-detectchanges-do/
    /// <summary>
    /// Provides a way to access all playback related data on the DAL
    /// </summary>
    /// <seealso cref="Maple.IMediaRepository" />
    public class MediaRepository : IMediaRepository
    {
        private const int _saveThreshold = 100;

        private readonly IMediaItemRepository _mediaItemRepository;
        private readonly IMediaPlayerRepository _mediaPlayerRepository;
        private readonly IPlaylistRepository _playlistRepository;

        private readonly IPlaylistMapper _playlistMapper;
        private readonly IMediaPlayerMapper _mediaPlayerMapper;
        private readonly IMediaItemMapper _mediaItemMapper;

        private readonly BusyStack _busyStack;

        private bool _disposed = false;

        public bool IsBusy { get; private set; }

        public MediaRepository(IPlaylistMapper playlistMapper,
                               IMediaItemMapper mediaItemMapper,
                               IMediaPlayerMapper mediaPlayerMapper,
                               IMediaItemRepository mediaItemRepository,
                               IMediaPlayerRepository mediaPlayerRepository,
                               IPlaylistRepository playlistRepository)
        {
            _mediaItemMapper = mediaItemMapper ?? throw new ArgumentNullException(nameof(mediaItemMapper));
            _playlistMapper = playlistMapper ?? throw new ArgumentNullException(nameof(playlistMapper));
            _mediaPlayerMapper = mediaPlayerMapper ?? throw new ArgumentNullException(nameof(mediaPlayerMapper));

            _mediaItemRepository = mediaItemRepository ?? throw new ArgumentNullException(nameof(mediaItemRepository));
            _mediaPlayerRepository = mediaPlayerRepository ?? throw new ArgumentNullException(nameof(mediaPlayerRepository));
            _playlistRepository = playlistRepository ?? throw new ArgumentNullException(nameof(playlistRepository));

            _busyStack = new BusyStack();
            _busyStack.OnChanged += (hasItems) => { IsBusy = hasItems; };
        }

        public void Save(MediaItem viewModel)
        {
            using (_busyStack.GetToken())
                _mediaItemRepository.Save(viewModel.Model);
        }

        public void Save(MediaItems viewModel)
        {
            using (_busyStack.GetToken())
            {
                foreach (var item in viewModel.Items)
                    Save(item);
            }
        }

        public async Task<MediaItem> GetMediaItemByIdAsync(int id)
        {
            using (_busyStack.GetToken())
            {
                var item = await _mediaItemRepository.GetByIdAsync(id);
                return _mediaItemMapper.Get(item);
            }
        }

        public async Task<IList<MediaItem>> GetMediaItemsAsync()
        {
            using (_busyStack.GetToken())
            {
                var items = await _mediaItemRepository.GetAsync();
                return items.Select(p => _mediaItemMapper.Get(p))
                            .ToList();
            }
        }

        public async Task<MediaItem> GetMediaItemByPlaylistIdAsync(int id)
        {
            using (_busyStack.GetToken())
            {
                var item = await _mediaItemRepository.GetMediaItemByPlaylistIdAsync(id);
                return _mediaItemMapper.Get(item);
            }
        }

        public void Save(Playlist viewModel)
        {
            using (_busyStack.GetToken())
                _playlistRepository.Save(viewModel.Model);
        }

        public void Save(Playlists viewModel)
        {
            using (_busyStack.GetToken())
            {
                foreach (var item in viewModel.Items)
                    Save(item);
            }
        }

        public async Task<Playlist> GetPlaylistByIdAsync(int id)
        {
            using (_busyStack.GetToken())
            {
                var item = await _playlistRepository.GetByIdAsync(id);
                return _playlistMapper.Get(item);
            }
        }

        public async Task<IList<Playlist>> GetPlaylistsAsync()
        {
            using (_busyStack.GetToken())
            {
                var items = await _playlistRepository.GetAsync();
                return items.Select(p => _playlistMapper.Get(p))
                            .ToList();
            }
        }

        public void Save(MediaPlayer viewModel)
        {
            using (_busyStack.GetToken())
                _mediaPlayerRepository.Save(viewModel.Model);
        }

        public void Save(MediaPlayers viewModel)  // performance (?)
        {
            using (_busyStack.GetToken())
            {
                foreach (var item in viewModel.Items)
                    Save(item);
            }
        }

        public async Task<MediaPlayer> GetMainMediaPlayerAsync()
        {
            using (_busyStack.GetToken())
            {
                var item = await _mediaPlayerRepository.GetMainMediaPlayerAsync();
                return _mediaPlayerMapper.GetMain(item, _playlistMapper.Get(item.Playlist));
            }
        }

        public async Task<MediaPlayer> GetMediaPlayerByIdAsync(int id)
        {
            using (_busyStack.GetToken())
            {
                var item = await _mediaPlayerRepository.GetByIdAsync(id);
                return _mediaPlayerMapper.Get(item);
            }
        }

        public async Task<IList<MediaPlayer>> GetAllOptionalMediaPlayersAsync()
        {
            using (_busyStack.GetToken())
            {
                var items = await _mediaPlayerRepository.GetOptionalMediaPlayersAsync();
                return items.Select(p => _mediaPlayerMapper.Get(p))
                            .ToList();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {

                // Free any other managed objects here.
            }

            // Free any unmanaged objects here.
            _disposed = true;
        }
    }
}
