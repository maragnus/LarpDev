using JetBrains.Annotations;
using KiloTx.Restful;
using Larp.Landing.Shared;

namespace Larp.Landing.Client.RestClient;

[RestfulImplementation<IAssistantService, HttpClientFactory>, MeansImplicitUse]
public partial class AssistantClient
{
}