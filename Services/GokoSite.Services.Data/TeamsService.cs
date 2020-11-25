namespace GokoSite.Services.Data
{
    using System;
    using System.Collections.Generic;

    using RiotSharp.Endpoints.MatchEndpoint;

    public class TeamsService : ITeamsService
    {
        public long GetTotalGoldByPlayers(List<ParticipantIdentity> participantIdentities, List<Participant> participants, int teamId)
        {
            long totalGold = 0;

            for (int i = 0; i < participants.Count; i++)
            {
                if (teamId == participants[i].TeamId && participants[i].ParticipantId == participantIdentities[i].ParticipantId)
                {
                    totalGold += participants[i].Stats.GoldEarned;
                }
            }

            return totalGold;
        }

        public int GetTotalKillsByPlayers(List<ParticipantIdentity> participantIdentities, List<Participant> participants, int teamId)
        {
            throw new NotImplementedException();
        }
    }
}
