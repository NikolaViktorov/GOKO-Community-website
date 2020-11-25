namespace GokoSite.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using GokoSite.Data;
    using GokoSite.Data.Models.LoL;
    using GokoSite.Services.Data.StaticData;
    using GokoSite.Web.ViewModels.Games;
    using GokoSite.Web.ViewModels.Games.DTOs;
    using RiotSharp;
    using RiotSharp.Endpoints.MatchEndpoint;
    using RiotSharp.Endpoints.SummonerEndpoint;

    public class GamesService : IGamesService
    {
        private readonly ApplicationDbContext db;
        private readonly IPlayersService playersService;
        private readonly ITeamsService teamsService;

        public RiotApi Api { get; set; }

        public GamesService(
            ApplicationDbContext db,
            IPlayersService playersService,
            ITeamsService teamsService)
        {
            this.db = db;
            this.playersService = playersService;
            this.teamsService = teamsService;
            this.Api = RiotApi.GetDevelopmentInstance(PublicData.apiKey);
        }

        public async Task<Summoner> GetBasicSummonerDataAsync(string summonerName, RiotSharp.Misc.Region region)
        {
            try
            {
                var summoner = await this.Api.Summoner.GetSummonerByNameAsync(region, summonerName);

                return summoner;
            }
            catch (RiotSharpException ex)
            {
                return null;
            }
        }

        public async Task<Match> GetGameAsync(long gameId, RiotSharp.Misc.Region region)
        {
            var game = await this.Api.Match.GetMatchAsync(region, gameId);

            return game;
        }

        public async Task<ICollection<Match>> GetGamesAsync(GetGamesInputModel input)
        {
            RiotSharp.Misc.Region region = (RiotSharp.Misc.Region)input.RegionId;

            var summoner = await this.GetBasicSummonerDataAsync(input.Username, region);

            if (summoner == null)
            {
                throw new ArgumentException("Wrong summoner name!");
            }

            var matches = await this.Api.Match.GetMatchListAsync(region, summoner.AccountId);

            var games = new List<Match>();

            for (int i = 0; i < input.Count; i++)
            {
                var game = await this.GetGameAsync(matches.Matches[i].GameId, region);

                games.Add(game);
            }

            return games;
        }

        public async Task<IEnumerable<HomePageGameViewModel>> GetModelByMatches(ICollection<Match> games, int regionId)
        {
            var viewModel = new List<HomePageGameViewModel>();

            foreach (var game in games)
            {
                viewModel.Add(new HomePageGameViewModel
                {
                    GameId = game.GameId,
                    RegionId = regionId,
                    BlueTeam = new TeamDTO
                    {
                        Players = await this.playersService.GetPlayersByParticipantsDto(game.ParticipantIdentities, game.Participants, 100),
                        State = game.Teams[0].Win,
                    },
                    RedTeam = new TeamDTO
                    {
                        Players = await this.playersService.GetPlayersByParticipantsDto(game.ParticipantIdentities, game.Participants, 200),
                        State = game.Teams[1].Win,
                    },
                });
            }

            return viewModel.ToList();
        }

        public async Task<HomePageGameViewModel> GetModelByGameId(long gameId, int regionId, string userId) // More eficient..
        {
            var region = (RiotSharp.Misc.Region)regionId;
            var game = await this.Api.Match.GetMatchAsync(region, gameId);

            var dbGame = this.db.Games.FirstOrDefault(g => g.RiotGameId == game.GameId);

            if (dbGame == null)
            {
                throw new ArgumentNullException("gameId", "Game with that Game Id does not exist in the database!");
            }

            var isInUserCollection = this.db.UserGames.Any(ug => ug.UserId == userId && ug.GameId == dbGame.GameId);

            if (isInUserCollection == false)
            {
                throw new InvalidOperationException($"This user ({userId}) does not contain a game with id({gameId}) in his collection!");
            }

            var viewModel = new HomePageGameViewModel
            {
                GameId = game.GameId,
                RegionId = regionId,
                BlueTeam = new TeamDTO
                {
                    Players = await this.playersService.GetPlayersByParticipantsDto(game.ParticipantIdentities, game.Participants, 100),
                    State = game.Teams[0].Win,
                    DragonsSlain = game.Teams[0].DragonKills,
                    BaronsSlain = game.Teams[0].BaronKills,
                    TurretsDestroyed = game.Teams[0].TowerKills,
                    TotalGold = this.teamsService.GetTotalGoldByPlayers(game.ParticipantIdentities, game.Participants, 100),
                },
                RedTeam = new TeamDTO
                {
                    Players = await this.playersService.GetPlayersByParticipantsDto(game.ParticipantIdentities, game.Participants, 200),
                    State = game.Teams[1].Win,
                    DragonsSlain = game.Teams[1].DragonKills,
                    BaronsSlain = game.Teams[1].BaronKills,
                    TurretsDestroyed = game.Teams[1].TowerKills,
                    TotalGold = this.teamsService.GetTotalGoldByPlayers(game.ParticipantIdentities, game.Participants, 200),
                },
            };

            return viewModel;
        }

        // Reorganize the code... FIXME
        public async Task AddGameToCollection(long gameId, int regionId)
        {
            var curGame = await this.GetGameAsync(gameId, (RiotSharp.Misc.Region)regionId);

            var region = this.db.Regions.FirstOrDefault(r => r.RiotRegionId == regionId);

            var game = new Game()
            {
                Region = region,
                RegionId = region.RegionId,
                RiotGameId = gameId,
            };

            var firstTeam = new Team
            {
                State = curGame.Teams[0].Win,
            };

            var secondTeam = new Team
            {
                State = curGame.Teams[1].Win,
            };

            game.Teams.Add(firstTeam);
            game.Teams.Add(secondTeam);

            this.db.Games.Add(game);
            await this.db.SaveChangesAsync();

            var firstTeamPlayers = this.playersService.GetPlayersByParticipants(curGame.ParticipantIdentities, curGame.Participants, 100).ToList();
            // 100 first team / 200 second team
            var secondTeamPlayers = this.playersService.GetPlayersByParticipants(curGame.ParticipantIdentities, curGame.Participants, 200).ToList();

            firstTeamPlayers.ForEach(p => game.Teams[0].Players.Add(p));
            secondTeamPlayers.ForEach(p => game.Teams[1].Players.Add(p));

            await this.db.SaveChangesAsync();

            var dbGame = this.db.Games.OrderByDescending(g => g.GameId).FirstOrDefault();

            var firstTeamId = dbGame.Teams[0].TeamId;
            var secondTeamId = dbGame.Teams[1].TeamId;

            var players = this.db.Players.Where(p => p.TeamId == firstTeamId || p.TeamId == secondTeamId).Select(p => p).ToList();
            var champions = this.db.ChampionsStatic.ToList();

            for (int i = 0; i < players.Count; i++)
            {
                var playerId = players[i].PlayerId;
                var participantIndex = curGame.ParticipantIdentities.FirstOrDefault(p => p.Player.SummonerName == players[i].Username).ParticipantId - 1;
                var champRiotId = curGame.Participants[participantIndex].ChampionId;
                var champId = champions.FirstOrDefault(c => c.ChampionRiotId == champRiotId.ToString()).ChampionId;

                this.db.PlayerChampion.Add(new PlayerChampion
                {
                    PlayerId = playerId,
                    ChampionId = champId,
                });
            }

            await this.db.SaveChangesAsync();
        }

        public void AddGameToUser(string userId)
        {
            var dbGame = this.db.Games.OrderByDescending(g => g.GameId).FirstOrDefault();

            this.db.UserGames.Add(new UserGames
            {
                UserId = userId,
                GameId = dbGame.GameId,
            });
            this.db.SaveChanges();
        }

        public int GetGameCount(string userId)
        {
            return this.db.UserGames.Where(u => u.UserId == userId).Count();
        }

        public ICollection<CollectionPageGameViewModel> GetCollectionGames(string userId)
        {
            var viewModel = new List<CollectionPageGameViewModel>();
            var gameIds = this.db.UserGames
                .Where(ug => ug.UserId == userId)
                .Select(ug => new { ug.GameId })
                .ToList();
            this.db.SaveChanges();

            foreach (var gameId in gameIds)
            {
                var curGame = this.db.Games
                    .FirstOrDefault(g => g.GameId == gameId.GameId);

                var curGameTeams = this.db.Teams
                    .Where(t => t.GameId == curGame.GameId)
                    .ToArray();

                // first team
                Team fTeam = curGameTeams[0];
                List<GokoSite.Data.Models.LoL.Player> fPlayers = this.db.Players
                    .Where(p => p.TeamId == fTeam.TeamId)
                    .ToList();

                List<Champion> fChampions = new List<Champion>();

                foreach (var player in fPlayers)
                {
                    fChampions.Add(this.db.PlayerChampion
                    .Where(pc => pc.PlayerId == player.PlayerId)
                    .Select(pc => new Champion
                    {
                        ChampionIconUrl = pc.Champion.ChampionIconUrl,
                        ChampionName = pc.Champion.ChampionName,
                        ChampionRiotId = pc.Champion.ChampionRiotId,
                        ChampionId = pc.Champion.ChampionId
                    })
                    .FirstOrDefault());
                }

                // second team
                Team sTeam = curGameTeams[1];
                List<GokoSite.Data.Models.LoL.Player> sPlayers = this.db.Players
                    .Where(p => p.TeamId == sTeam.TeamId)
                    .ToList();

                List<Champion> sChampions = new List<Champion>();

                foreach (var player in sPlayers)
                {
                    sChampions.Add(this.db.PlayerChampion
                    .Where(pc => pc.PlayerId == player.PlayerId)
                    .Select(pc => new Champion
                    {
                        ChampionIconUrl = pc.Champion.ChampionIconUrl,
                        ChampionName = pc.Champion.ChampionName,
                        ChampionRiotId = pc.Champion.ChampionRiotId,
                        ChampionId = pc.Champion.ChampionId
                    })
                    .FirstOrDefault());
                }

                viewModel.Add(this.GetModelByGame(curGame, fChampions, sChampions));
            }

            return viewModel;
        }

        public void RemoveGameFromCollection(string userId, long gameId)
        {
            var game = this.db.Games
                .FirstOrDefault(g => g.RiotGameId == gameId);

            var teams = this.db.Teams
                .Where(t => t.GameId == game.GameId)
                .ToArray();

            // first team
            var fTeam = teams[0];
            var fPlayers = this.db.Players
                .Where(p => p.TeamId == fTeam.TeamId)
                .ToList();

            fPlayers.ForEach(p => p.PlayerChampions.Clear());
            this.db.Players.RemoveRange(fPlayers);

            // second team
            var sTeam = teams[1];
            var sPlayers = this.db.Players
                .Where(p => p.TeamId == sTeam.TeamId)
                .ToList();

            sPlayers.ForEach(p => p.PlayerChampions.Clear());
            this.db.Players.RemoveRange(sPlayers);

            this.db.Teams.RemoveRange(teams);

            this.db.Games.FirstOrDefault(g => g.RiotGameId == gameId).UserGames.Clear();

            this.db.Games.Remove(game);

            this.db.SaveChanges();
        }

        private CollectionPageGameViewModel GetModelByGame(Game game, List<Champion> fChampions, List<Champion> sChampions)
        {
            var regionId = this.db.Regions.FirstOrDefault(r => r.RegionId == game.RegionId).RiotRegionId;

            var curModel = new CollectionPageGameViewModel
            {
                GameId = game.RiotGameId,
                RegionId = regionId,
                BlueTeam = new TeamDTO
                {
                    Players = GetPlayersDtoList(game.Teams[0].Players, fChampions),
                    State = game.Teams[0].State,
                },
                RedTeam = new TeamDTO
                {
                    Players = GetPlayersDtoList(game.Teams[1].Players, sChampions),
                    State = game.Teams[1].State,
                },
            };

            return curModel;
        }

        private List<PlayerDTO> GetPlayersDtoList(ICollection<GokoSite.Data.Models.LoL.Player> players, List<Champion> champions)
        {
            var dtos = new List<PlayerDTO>();

            int i = 0;
            foreach (var player in players)
            {
                var champion = champions[i];

                dtos.Add(new PlayerDTO
                {
                    Username = player.Username,
                    ProfileIconUrl = player.ProfileIconUrl,
                    Champion = new ChampionDTO
                    {
                        ChampionIconUrl = champion.ChampionIconUrl,
                        ChampionName = champion.ChampionName,
                    },
                });
                i++;
            }

            return dtos;
        }
    }
}
