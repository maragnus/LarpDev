using JetBrains.Annotations;
using KiloTx.Restful;
using Larp.Landing.Shared;

namespace Larp.Landing.Client.RestClient;

[RestfulImplementation<IAdminService, HttpClientFactory>, MeansImplicitUse]
public partial class AdminServiceClient
{
}

[RestfulImplementation<ILandingService, HttpClientFactory>, MeansImplicitUse]
public partial class LandingServiceClient
{
}
