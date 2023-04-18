using JetBrains.Annotations;
using KiloTx.Restful;
using Larp.Landing.Shared.MwFifth;

namespace Larp.Landing.Client.RestClient;

[RestfulImplementation<IMwFifthService, HttpClientFactory>, MeansImplicitUse]
public partial class MwFifthServiceClient
{
}