﻿using Newtonsoft.Json;
using ScoreSaber.Core.Data.Models;
using System.Reflection;
using System.Threading.Tasks;

namespace ScoreSaber.Core.Services {

    internal class GlobalLeaderboardService {

        public enum GlobalPlayerScope {
            Global,
            AroundPlayer,
            Friends,
            Country
        }

        public GlobalLeaderboardService() {
            Plugin.Log.Debug("GlobalLeaderboardService Setup");
        }

        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public async Task<PlayerInfo[]> GetPlayerList(GlobalPlayerScope scope, int page) {

            string url = BuildUrl(scope, page);

            var response = await Plugin.HttpInstance.GetAsync(url);
            var globalLeaderboardData = JsonConvert.DeserializeObject<PlayerInfo[]>(response);
            return globalLeaderboardData;
        }

        private string BuildUrl(GlobalPlayerScope scope, int page) {

            string url = "/game/players";
            switch (scope) {
                case GlobalPlayerScope.Global:
                    url = $"{url}?page={page}";
                    break;
                case GlobalPlayerScope.AroundPlayer:
                    url = $"{url}/around-player";
                    break;
                case GlobalPlayerScope.Friends:
                    url = $"{url}/around-friends?page={page}";
                    break;
                case GlobalPlayerScope.Country:
                    url = $"{url}/around-country?page={page}";
                    break;
            }
            return url;
        }

    }
}