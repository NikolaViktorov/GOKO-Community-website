namespace GokoSite.Services.Data
{
    using System;
    using System.Collections.Generic;

    using RiotSharp.Endpoints.MatchEndpoint;

    public class TeamsService : ITeamsService
    {
        public long GetTotalGoldByPlayers(List<ParticipantIdentity> participantIdentities, List<Participant> participants, int teamId)
        {
            if (participantIdentities == null)
            {
                throw new ArgumentNullException("participantIdentities", "The participants identities must not be null.");
            }

            if (participants == null)
            {
                throw new ArgumentNullException("participants", "The participants must not be null.");
            }

            if (teamId != 100 && teamId != 200)
            {
                throw new ArgumentException("Invalid Team Id. It must be 100(first team) or 200(second team)!");
            }

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
    }
}
