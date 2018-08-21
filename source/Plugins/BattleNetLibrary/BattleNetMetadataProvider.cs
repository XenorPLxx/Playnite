﻿using Playnite;
using Playnite.SDK;
using Playnite.SDK.Metadata;
using Playnite.SDK.Models;
using Playnite.Web;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNetLibrary
{
    public class BattleNetMetadataProvider : IMetadataProvider
    {
        public BattleNetMetadataProvider()
        {
        }

        #region IMetadataProvider

        public GameMetadata GetMetadata(string metadataId)
        {
            var gameData = new Game("OriginGame")
            {
                GameId = metadataId
            };

            var data = UpdateGameWithMetadata(gameData);
            return new GameMetadata(gameData, data.Icon, data.Image, data.BackgroundImage);
        }

        public GameMetadata GetMetadata(Game game)
        {
            return GetMetadata(game.GameId);
        }

        public ICollection<MetadataSearchResult> SearchMetadata(Game game)
        {
            throw new NotImplementedException();
        }

        #endregion IMetadataProvider

        public GameMetadata UpdateGameWithMetadata(Game game)
        {
            var metadata = new GameMetadata();
            var product = BattleNetGames.GetAppDefinition(game.GameId);
            if (product == null)
            {
                return metadata;
            }

            if (string.IsNullOrEmpty(product.IconUrl))
            {
                return metadata;
            }

            game.Name = product.Name;
            var icon = HttpDownloader.DownloadData(product.IconUrl);
            var iconFile = Path.GetFileName(product.IconUrl);
            metadata.Icon = new MetadataFile($"images/battlenet/{game.GameId}/{iconFile}", iconFile, icon);
            var cover = HttpDownloader.DownloadData(product.CoverUrl);
            var coverFile = Path.GetFileName(product.CoverUrl);
            metadata.Image = new MetadataFile($"images/battlenet/{game.GameId}/{coverFile}", coverFile, cover);
            game.BackgroundImage = product.BackgroundUrl;
            metadata.BackgroundImage = product.BackgroundUrl;
            game.Links = new ObservableCollection<Link>(product.Links);
            return metadata;
        }
    }
}
