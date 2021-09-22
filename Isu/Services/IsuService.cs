using System;
using System.Collections.Generic;
using System.Linq;
using Isu.MyClasses;
using Isu.Tools;

namespace Isu.Services
{
    public class IsuService : IIsuService
    {
        private int _nextId = 100000;

        public IsuService()
        {
            EducationalProgram = new EducationalProgram();
        }

        public EducationalProgram EducationalProgram { get; }

        public List<Group> FindGroups(CourseNumber courseNumber)
        {
            Course course =
                EducationalProgram.Courses.FirstOrDefault(c => c.CourseNumber.Number == courseNumber.Number);
            if (course == null)
            {
                throw new IsuException($"Error. There is no course {courseNumber.Number}");
            }

            return course.Groups.ToList();
        }

        public Group FindGroup(GroupName groupName)
        {
            List<Group> groups = FindGroups(new CourseNumber((int)char.GetNumericValue(groupName.Name[2])));

            if (groups.Count == 0)
            {
                return null;
            }

            return groups.FirstOrDefault(g => g.GroupName.Name == groupName.Name);
        }

        public List<Student> FindStudents(GroupName groupName)
        {
            Group group = FindGroup(groupName);

            if (group == null)
            {
                throw new IsuException($"Error. Group {groupName} doesn't exist.");
            }

            return group.Students.ToList();
        }

        public List<Student> FindStudents(CourseNumber courseNumber)
        {
            if (EducationalProgram.Courses.FirstOrDefault(c => c.CourseNumber.Number == courseNumber.Number) == null)
            {
                throw new IsuException($"Error. There is no course {courseNumber.Number}");
            }

            List<Group> groups = FindGroups(courseNumber);

            return groups.SelectMany(g => g.Students).ToList();
        }

        public Student GetStudent(int id)
        {
            List<Group> groups = EducationalProgram.Courses.SelectMany(c => c.Groups).ToList();

            if (groups.Count == 0)
            {
                throw new IsuException($"Error. There is no groups, thus no {id} student.");
            }

            Student student = groups.SelectMany(g => g.Students).FirstOrDefault(x => x.Id == id);

            if (student == null)
            {
                throw new IsuException($"Error. Wasn't able to find a student with id: {id}");
            }

            return student;
        }

        public Student FindStudent(string name)
        {
            List<Group> groups = EducationalProgram.Courses.SelectMany(c => c.Groups).ToList();

            if (groups.Count == 0)
            {
                return null;
            }

            return groups.SelectMany(g => g.Students).FirstOrDefault(x => x.Name == name);
        }

        public Group AddGroup(GroupName name)
        {
            if (FindGroup(name) != null)
            {
                throw new IsuException($"Error. Group {name.Name} already exists");
            }

            Group newGroup = new Group(name);
            Course course = EducationalProgram.Courses.FirstOrDefault(x =>
                x.CourseNumber.Number == (int)char.GetNumericValue(newGroup.GroupName.Name[2]));

            if (course == null)
            {
                throw new IsuException($"Error. Course {newGroup.GroupName.Name[2]} doesn't exist");
            }

            course.AddGroup(newGroup);

            return newGroup;
        }

        public Student AddStudent(Group group, string name)
        {
            if (FindGroup(group.GroupName) == null)
            {
                throw new IsuException(
                    $"Error. Group {group.GroupName.Name} to which student {name} has to be assigned doesn't exist.");
            }

            Student student = new Student(name, _nextId++);
            Course course = EducationalProgram.Courses.FirstOrDefault(x =>
                x.CourseNumber.Number == (int)char.GetNumericValue(group.GroupName.Name[2]));

            if (course == null)
            {
                throw new IsuException($"Error. Course {group.GroupName.Name[2]} doesn't exist");
            }

            course.AddStudent(group, student);
            return student;
        }

        public Group FindStudentsGroup(Student student)
        {
            return EducationalProgram.Courses.SelectMany(c => c.Groups).FirstOrDefault(g => g.Contain(student));
        }

        public void ChangeStudentGroup(Student student, Group newGroup)
        {
            if (FindGroup(newGroup.GroupName) == null)
            {
                throw new IsuException($"Error. There is no group {newGroup.GroupName.Name}");
            }

            Group previousGroup = FindStudentsGroup(student);

            if (previousGroup == null)
            {
                throw new IsuException($"There is no such student {student.Name}, id: {student.Id}");
            }

            previousGroup.RemoveStudent(student);
            newGroup.AddStudent(student);
        }
    }
}