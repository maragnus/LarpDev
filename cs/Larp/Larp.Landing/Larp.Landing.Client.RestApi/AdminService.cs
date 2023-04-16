using JetBrains.Annotations;
using KiloTx.Restful;
using Larp.Landing.Shared;

namespace Larp.Landing.Client.RestApi;

[RestfulImplementation<IAdminService>, MeansImplicitUse]
public partial class AdminService
{
}