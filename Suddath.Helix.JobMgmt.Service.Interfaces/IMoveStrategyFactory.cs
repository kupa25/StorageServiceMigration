namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    //https://adamstorr.azurewebsites.net/blog/aspnetcore-and-the-strategy-pattern
    public interface IMoveStrategyFactory
    {
        ILegacyMoveService[] Create();
    }
}
