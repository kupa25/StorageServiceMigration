namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface IApplicationPatch
    {
        string PatchName { get; }

        void ExecutePatch();
    }
}