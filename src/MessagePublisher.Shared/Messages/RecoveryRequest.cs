using MessagePublisher.Shared.Models;
using System.Collections.Generic;

namespace MessagePublisher.Shared.Messages
{
    public class RecoveryRequest
    {
        public List<RecoveryInfo> Informations { get; private set; }

        public RecoveryRequest(List<RecoveryInfo> informations)
        {
            Informations = informations;
        }
    }
}
