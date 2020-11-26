
namespace GokoSite.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using GokoSite.Data;
    using GokoSite.Services.Data.StaticData;
    using GokoSite.Web.ViewModels.Games.DTOs;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using RiotSharp;
    using RiotSharp.Endpoints.MatchEndpoint;
    using RiotSharp.Misc;
    using Xunit;

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
    }
}
