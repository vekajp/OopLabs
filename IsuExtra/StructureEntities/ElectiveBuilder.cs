using System.Collections.Generic;

namespace IsuExtra.StructureEntities
{
    public class ElectiveBuilder
    {
        private string _id;
        private Faculty _faculty;
        private List<ElectiveGroup> _divisions;
        public ElectiveBuilder(string electiveId, Faculty faculty)
        {
            _id = electiveId;
            _faculty = faculty;
            _divisions = new List<ElectiveGroup>();
        }

        public void AddDivision(ElectiveGroup electiveGroup)
        {
            _divisions.Add(electiveGroup);
        }

        public Elective MakeElective()
        {
            return new Elective(_id, _faculty, _divisions);
        }
    }
}