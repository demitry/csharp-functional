using AuditManager;
using System;
using System.Collections.Generic;
using Xunit;

namespace AuditManagerTests
{
    public class AuditManagerTests
    {
        [Fact]
        public void AddRecord_Adds_a_record_to_an_existing_file_if_not_overflowed()
        {
            // Arrange
            var manager = new AuditManager.AuditManager(10);
            var file = new FileContent("Audit_1.txt", new[]
            {
                "1;Peter Peterson;4/6/2016 4:30:00 PM"
            });
            
            // Act
            FileAction action = manager.AddRecord(file, "Jane Doe", new DateTime(2016, 4, 6, 17, 0, 0));

            // Assert
            Assert.Equal(ActionType.Update, action.Type);
            Assert.Equal("Audit_1.txt", action.FileName);
            Assert.Equal(new[]
            {
                "1;Peter Peterson;4/6/2016 4:30:00 PM",
                "2;Jane Doe;4/6/2016 5:00:00 PM" // todo: verify time format and regional settings
            }, action.Content);
        }

        [Fact]
        public void AddRecord_Adds_a_new_record_if_overflowed()
        {
            // Arrange
            var manager = new AuditManager.AuditManager(3);
            var file = new FileContent("Audit_1.txt", new []
            {
                "1;Peter Peterson;4/6/2016 4:30:00 PM",
                "2;Jane Doe;4/6/2016 5:00:00 PM",
                "3;Jack Rich;4/6/2016 6:00:00 PM"
            });

            // Act
            FileAction action = manager.AddRecord(file, "Tom Tompson",
                new DateTime(2016, 4, 6, 18, 0, 0));
            
            //Assert
            Assert.Equal(ActionType.Create, action.Type);
            Assert.Equal("Audit_2.txt", action.FileName);
            Assert.Equal(new[]
            {
                "1;Tom Tompson;4/6/2016 6:00:00 PM"
            }, action.Content);
        }

        [Fact]
        public void RemoveMentionsAbout_removes_mentions_from_filed_in_derectory()
        {
            var manager = new AuditManager.AuditManager(10);
            var file = new FileContent("Audit_1.txt", new[]
            {
                "1;Peter Peterson;4/6/2016 4:30:00 PM",
                "2;Jane Doe;4/6/2016 5:00:00 PM",
                "3;Jack Rich;4/6/2016 6:00:00 PM"
            });

            IReadOnlyList<FileAction> actions = manager.RemoveMentionsAbout("Peter Peterson", new[] {file});
            Assert.Equal(1, actions.Count);
            Assert.Equal("Audit_1.txt", actions[0].FileName);
            Assert.Equal(ActionType.Update, actions[0].Type);
            Assert.Equal( actions[0].Content,
                new[]
                {
                    "1;Jane Doe;4/6/2016 5:00:00 PM",
                    "2;Jack Rich;4/6/2016 6:00:00 PM"
                });
        }

        [Fact]
        public void RemoveMentionsAbout_removes_whole_file_if_it_doesnot_contain_anything_else()
        {
            var manager = new AuditManager.AuditManager(10);
            var file = new FileContent("Audit_1.txt", new[]
            {
                "1;Peter Peterson;4/6/2016 4:30:00 PM"
            });

            IReadOnlyList<FileAction> actions = manager.RemoveMentionsAbout("Peter Peterson", new[] { file });

            Assert.Equal(1, actions.Count);
            Assert.Equal("Audit_1.txt", actions[0].FileName);
            Assert.Equal(ActionType.Delete, actions[0].Type);
        }

        [Fact]
        public void RemoveMentionsAbout_does_not_do_anything_if_no_mentions_found()
        {
            var manager = new AuditManager.AuditManager(10);
            var file = new FileContent("Audit_1.txt", new[]
{
                "1;Peter Peterson;4/6/2016 4:30:00 PM"
            });
            IReadOnlyList<FileAction> actions = manager.RemoveMentionsAbout("Joe Cocker", new[] { file });

            Assert.Equal(0, actions.Count);
        }

    }
}