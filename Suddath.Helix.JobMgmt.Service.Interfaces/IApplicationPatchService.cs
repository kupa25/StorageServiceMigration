namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface IApplicationPatchService
    {
        void LoadPatch(IApplicationPatch patch);

        void ExecutePatches();
    }
}