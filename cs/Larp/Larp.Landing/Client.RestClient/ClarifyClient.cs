using JetBrains.Annotations;
using KiloTx.Restful;
using Larp.Landing.Shared;

namespace Larp.Landing.Client.RestClient;

[RestfulImplementation<IClarifyService, HttpClientFactory>, MeansImplicitUse]
public partial class ClarifyClient
{
}