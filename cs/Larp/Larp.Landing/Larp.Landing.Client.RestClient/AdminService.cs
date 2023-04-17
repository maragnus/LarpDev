using JetBrains.Annotations;
using KiloTx.Restful;
using Larp.Landing.Shared;

namespace Larp.Landing.Client.RestClient;

[RestfulImplementation<IAdminService>, MeansImplicitUse]
public partial class AdminServiceClient
{
}

[RestfulImplementation<ILandingService>, MeansImplicitUse]
public partial class LandingServiceClient
{
}
