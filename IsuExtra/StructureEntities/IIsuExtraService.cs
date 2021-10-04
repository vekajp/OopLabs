using System.Collections.Generic;

namespace IsuExtra.StructureEntities
{
    public interface IIsuExtraService : IElectiveManager, IFacultyManager
    {
        IReadOnlyCollection<IsuStudent> StudentsNotCheckedIn();
    }
}