
namespace GokoSite.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using GokoSite.Data;
    using GokoSite.Data.Models;
    using GokoSite.Data.Models.LoL;
    using GokoSite.Services.Data.StaticData;
    using GokoSite.Web.ViewModels.Games;
    using GokoSite.Web.ViewModels.Games.DTOs;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using RiotSharp;
    using RiotSharp.Endpoints.MatchEndpoint;
    using RiotSharp.Misc;
    using Xunit;
    using Region = RiotSharp.Misc.Region;

    public class GamesServiceTests
    {
        private Mock<ITeamsService> teamsService;
        private Mock<IPlayersService> playersService;
        private RiotApi api;

        public GamesServiceTests()
        {
            this.playersService = new Mock<IPlayersService>();
            this.teamsService = new Mock<ITeamsService>();
            this.api = RiotApi.GetDevelopmentInstance(PublicData.apiKey);
        }

        [Fact]
        public async Task GetBasicSummonerDataAsyncShouldReturnSummonerBySummonerName()
        {
            string username = "Nikolcho";
            var region = Region.Eune;

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("lolSummonerGet");
            var db = new ApplicationDbContext(options.Options);

            int expectedProfileIconId = 4813;
            string expectedAccountId = "-0TmH2cZiK9xbRvB8GjSArr79ZNl2eWKLwuyFSHITqr8ow";

            var service = new GamesService(db, this.playersService.Object, this.teamsService.Object);

            var result = await service.GetBasicSummonerDataAsync(username, region);

            Assert.NotNull(result);
            Assert.Equal(username, result.Name);
            Assert.Equal(region, result.Region);
            Assert.Equal(expectedProfileIconId, result.ProfileIconId);
            Assert.Equal(expectedAccountId, result.AccountId);
        }

        [Fact]
        public async Task GetBasicSummonerDataAsyncShouldReturnNullIfGivenInvalidSummonerName()
        {
            string username = "afweawefgrawrgawfwrafrw";
            var region = Region.Eune;

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("lolSummonerGetWrong");
            var db = new ApplicationDbContext(options.Options);

            var service = new GamesService(db, this.playersService.Object, this.teamsService.Object);

            var result = await service.GetBasicSummonerDataAsync(username, region);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetGameAsyncShouldReturnMatchWithDataForTheGame()
        {
            long gameId = 2652692459;
            var region = Region.Eune;

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("lolSummonerGet");
            var db = new ApplicationDbContext(options.Options);

            int expectedSeasonId = 13;
            string expectedGameMode = "CLASSIC";
            int expectedQueueId = 420;
            string seventhParticipantExpectedUsername = "lnvictum";
            int firstTeamThirdBanChampionId = 58;

            var service = new GamesService(db, this.playersService.Object, this.teamsService.Object);

            var result = await service.GetGameAsync(gameId, region);

            Assert.NotNull(result);
            Assert.IsType<RiotSharp.Endpoints.MatchEndpoint.Match>(result);
            Assert.Equal(gameId, result.GameId);
            Assert.Equal(expectedQueueId, result.QueueId);
            Assert.Equal(expectedGameMode, result.GameMode);
            Assert.Equal(expectedSeasonId, result.SeasonId);
            Assert.Equal(firstTeamThirdBanChampionId, result.Teams[0].Bans[2].ChampionId);
            Assert.Equal(seventhParticipantExpectedUsername, result.ParticipantIdentities[6].Player.SummonerName);
        }

        [Fact]
        public async Task GetGameAsyncShouldReturnNullIfGivenInvalidGameId()
        {
            long gameId = -321312312;
            var region = Region.Eune;

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("lolSummonerGet");
            var db = new ApplicationDbContext(options.Options);

            var service = new GamesService(db, this.playersService.Object, this.teamsService.Object);

            var result = await service.GetGameAsync(gameId, region);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetGamesAsyncShouldReturnCollectionoFMatchByInput()
        {
            var summonerName = "Nikolcho";
            var count = 2;
            var region = Region.Eune;

            var summoner = await this.api.Summoner.GetSummonerByNameAsync(region, summonerName);

            var matchList = await this.api.Match.GetMatchListAsync(region, summoner.AccountId);
            var expectedGames = new List<RiotSharp.Endpoints.MatchEndpoint.Match>();

            for (int i = 0; i < count; i++)
            {
                var game = await this.api.Match.GetMatchAsync(region, matchList.Matches[i].GameId);

                expectedGames.Add(game);
            }

            GetGamesInputModel input = new GetGamesInputModel()
            {
                Username = summonerName,
                Count = count,
                RegionId = 1, // Eune
            };

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("lolSummonerGetMatches");
            var db = new ApplicationDbContext(options.Options);

            var service = new GamesService(db, this.playersService.Object, this.teamsService.Object);

            var games = (await service.GetGamesAsync(input)).ToList();

            Assert.NotNull(games);
            Assert.IsType<List<RiotSharp.Endpoints.MatchEndpoint.Match>>(games);
            Assert.Equal(expectedGames[0].GameId, games[0].GameId);
            Assert.Equal(expectedGames[0].GameCreation, games[0].GameCreation);
            Assert.Equal(expectedGames[0].QueueId, games[0].QueueId);
            Assert.Equal(expectedGames[0].SeasonId, games[0].SeasonId);
            Assert.Equal(expectedGames[1].GameId, games[1].GameId);
            Assert.Equal(expectedGames[1].GameCreation, games[1].GameCreation);
            Assert.Equal(expectedGames[1].QueueId, games[1].QueueId);
            Assert.Equal(expectedGames[1].SeasonId, games[1].SeasonId);
        }

        [Theory]
        [InlineData("wafewarwa")]
        [InlineData("1321312321")]
        [InlineData("mamffaafa")]
        [InlineData("xasdgwafwaf")]
        public async Task GetGamesAsyncShouldThrowArgumentNullExceptionIfGivenNonExistentUsername(string username)
        {
            GetGamesInputModel input = new GetGamesInputModel()
            {
                Username = username,
                Count = 1,
                RegionId = 1, // Eune
            };

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("lolSummonerGetMatches");
            var db = new ApplicationDbContext(options.Options);

            var service = new GamesService(db, this.playersService.Object, this.teamsService.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await service.GetGamesAsync(input));
        }

        [Theory]
        [InlineData("")]
        [InlineData("qwqwqwqwqwqwqwqwqwqw")]
        [InlineData("fewefweagrwokfapworwokwarof")]
        [InlineData("1234567891011121314")]
        public async Task GetGamesAsyncShouldThrowArgumentExceptionIfGivenInvalidUsername(string username)
        {
            GetGamesInputModel input = new GetGamesInputModel()
            {
                Username = username,
                Count = 1,
                RegionId = 1, // Eune
            };

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("lolSummonerGetMatches");
            var db = new ApplicationDbContext(options.Options);

            var service = new GamesService(db, this.playersService.Object, this.teamsService.Object);

            await Assert.ThrowsAsync<ArgumentException>(async () => await service.GetGamesAsync(input));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(12)]
        [InlineData(18)]
        [InlineData(-12313)]
        public async Task GetGamesAsyncShouldThrowArgumentExceptionIfGivenInvalidCount(int count)
        {
            GetGamesInputModel input = new GetGamesInputModel()
            {
                Username = "Nikolcho",
                Count = count,
                RegionId = 1, // Eune
            };

            GetGamesInputModel secondInput = new GetGamesInputModel()
            {
                Username = "Nikolcho",
                Count = count,
                RegionId = 1, // Eune
            };

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("lolSummonerGetMatches");
            var db = new ApplicationDbContext(options.Options);

            var service = new GamesService(db, this.playersService.Object, this.teamsService.Object);

            await Assert.ThrowsAsync<ArgumentException>(async () => await service.GetGamesAsync(input));
            await Assert.ThrowsAsync<ArgumentException>(async () => await service.GetGamesAsync(secondInput));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(12)]
        [InlineData(18)]
        [InlineData(-12313)]
        public async Task GetGamesAsyncShouldThrowArgumentExceptionIfGivenInvalidRegionId(int regionId)
        {
            GetGamesInputModel input = new GetGamesInputModel()
            {
                Username = "Nikolcho",
                Count = regionId,
                RegionId = 1, // Eune
            };

            GetGamesInputModel secondInput = new GetGamesInputModel()
            {
                Username = "Nikolcho",
                Count = regionId,
                RegionId = 1, // Eune
            };

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("lolSummonerGetMatches");
            var db = new ApplicationDbContext(options.Options);

            var service = new GamesService(db, this.playersService.Object, this.teamsService.Object);

            await Assert.ThrowsAsync<ArgumentException>(async () => await service.GetGamesAsync(input));
            await Assert.ThrowsAsync<ArgumentException>(async () => await service.GetGamesAsync(secondInput));
        }

        [Fact]
        public async Task GetModelByMatchesShouldReturnListOfHomePageGameViewModel()
        {
            var summonerName = "Nikolcho";
            var count = 2;
            var region = Region.Eune;
            var regionId = 1;

            var summoner = await this.api.Summoner.GetSummonerByNameAsync(region, summonerName);

            var matchList = await this.api.Match.GetMatchListAsync(region, summoner.AccountId);
            var games = new List<RiotSharp.Endpoints.MatchEndpoint.Match>();

            for (int i = 0; i < count; i++)
            {
                var game = await this.api.Match.GetMatchAsync(region, matchList.Matches[i].GameId);

                games.Add(game);
            }

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase("lolSummonerGetMatches");
            var db = new ApplicationDbContext(options.Options);

            var service = new GamesService(db, this.playersService.Object, this.teamsService.Object);

            var result = (await service.GetModelByMatches(games, regionId)).ToList();

            Assert.NotNull(result);
            Assert.IsType<List<HomePageGameViewModel>>(result);
            Assert.NotNull(result[0].BlueTeam);
            Assert.NotNull(result[0].RedTeam);
            Assert.IsType<TeamDTO>(result[0].BlueTeam);
            Assert.IsType<TeamDTO>(result[0].RedTeam);
            Assert.Equal(games[0].GameId, result[0].GameId);
            Assert.Equal(games[1].GameId, result[1].GameId);
            Assert.Equal(games[0].Participants.First().Stats.Winner, result[0].BlueTeam.State != "Fail");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(12)]
        [InlineData(18)]
        [InlineData(-12313)]
        public async Task GetModelByMatchesShouldThrowArgumentExceptionIfGivenInvalidRegionId(int regionId)
        {
            var games = new List<RiotSharp.Endpoints.MatchEndpoint.Match>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("lolSummonerGetMatches");
            var db = new ApplicationDbContext(options.Options);

            var service = new GamesService(db, this.playersService.Object, this.teamsService.Object);

            await Assert.ThrowsAsync<ArgumentException>(async () => await service.GetModelByMatches(games, regionId));
        }

        [Fact]
        public async Task GetModelByMatchesShouldThrowArgumentNullExceptionIfGivenInvalidGameCollection()
        {
            var collectionWithoutGames = new List<RiotSharp.Endpoints.MatchEndpoint.Match>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("lolSummonerGetMatches");
            var db = new ApplicationDbContext(options.Options);

            var service = new GamesService(db, this.playersService.Object, this.teamsService.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await service.GetModelByMatches(null, 2));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await service.GetModelByMatches(collectionWithoutGames, 2));
        }

        [Fact]
        public async Task GetModelByGameIdShouldReturnHomePageGameViewModelOfGivenGame()
        {
            var riotGameId = 2657118595;
            var regionId = 1;

            var user = new ApplicationUser()
            {
                Email = "f@a.b",
            };
            var game = new Game()
            {
                RiotGameId = riotGameId,
            };

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("lolSummonerGetMatches");
            var db = new ApplicationDbContext(options.Options);

            await db.Users.AddAsync(user);
            await db.Games.AddAsync(game);
            await db.UserGames.AddAsync(new UserGames
            {
                UserId = user.Id,
                GameId = game.GameId,
            });
            await db.SaveChangesAsync();

            var userId = user.Id;

            var service = new GamesService(db, this.playersService.Object, this.teamsService.Object);

            var result = await service.GetModelByGameId(riotGameId, regionId, userId);

            Assert.NotNull(result);
        }
    }
}
