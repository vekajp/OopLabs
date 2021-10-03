using System.Collections.Generic;
using Isu.Services;

namespace IsuExtra.StructureEntities
{
    public interface IIsuExtraService : IElectiveManager, IFacultyManager
    {
        public IEnumerable<Student> StudentsNotCheckedIn();
    }
}