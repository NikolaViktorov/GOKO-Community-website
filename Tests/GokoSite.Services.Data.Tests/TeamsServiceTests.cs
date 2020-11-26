namespace GokoSite.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using GokoSite.Data;
    using GokoSite.Data.Models;
    using GokoSite.Data.Models.News;
    using GokoSite.Services.Data.StaticData;
    using GokoSite.Web.ViewModels.News;
    using Microsoft.EntityFrameworkCore;
    using RiotSharp;
    using RiotSharp.Endpoints.MatchEndpoint;
    using Xunit;

    public class TeamsServiceTests
    {
        private RiotApi api;

        public TeamsServiceTests()
        {
            this.api = RiotApi.GetDevelopmentInstance(PublicData.apiKey);
        }

        [Fact]
        public async Task GetTotalGoldByPlayersShouldReturnTheTotalGoldOfAllPlayersInATeam()
        {
            var gameId = 2655757524;
            var region = RiotSharp.Misc.Region.Eune;
            var game = await this.api.Match.GetMatchAsync(region, gameId);

            List<ParticipantIdentity> participantIdentities = game.ParticipantIdentities;
            List<Participant> participants = game.Participants;
            int firstTeamId = 100;
            int secondTeamId = 200;

            long firstTeamTotalExpectedGold = 71798;
            long secondTeamTotalExpectedGold = 59756;

            var service = new TeamsService();

            var firstTeamResult = service.GetTotalGoldByPlayers(participantIdentities, participants, firstTeamId);
            var secondTeamResult = service.GetTotalGoldByPlayers(participantIdentities, participants, secondTeamId);

            Assert.Equal(firstTeamTotalExpectedGold, firstTeamResult);
            Assert.Equal(secondTeamTotalExpectedGold, secondTeamResult);
        }
    }
}
