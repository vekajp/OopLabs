using System.Collections.Generic;
using Isu.Services;

namespace IsuExtra.StructureEntities
{
    public class ElectiveBuilder
    {
        private string _id;
        private Faculty _faculty;
        private List<Division> _divisions;
        public ElectiveBuilder(string electiveId, Faculty faculty)
        {
            _id = electiveId;
            _faculty = faculty;
            _divisions = new List<Division>();
        }

        public void AddDivision(Division division)
        {
            _divisions.Add(division);
        }

        public Elective MakeElective()
        {
            return new Elective(_id, _faculty, _divisions);
        }
    }
}