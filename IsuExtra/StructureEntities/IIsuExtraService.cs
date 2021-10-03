using System.Collections.Generic;

namespace IsuExtra.StructureEntities
{
    public interface IIsuExtraService : IElectiveManager, IFacultyManager
    {
        public IEnumerable<IsuStudent> StudentsNotCheckedIn();
    }
}