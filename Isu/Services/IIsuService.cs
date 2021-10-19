﻿using System.Collections.Generic;
using Isu.Entities;
using Isu.MyClasses;

namespace Isu.Services
{
    public interface IIsuService
    {
        Group AddGroup(GroupName name);
        Student AddStudent(Group group, string name);

        Student GetStudent(int id);
        Student FindStudent(string name);
        List<Student> FindStudents(GroupName groupName);
        List<Student> FindStudents(CourseNumber courseNumber);

        Group FindGroup(GroupName groupName);
        List<Group> FindGroups(CourseNumber course);

        void ChangeStudentGroup(Student student, Group newGroup);
    }
}