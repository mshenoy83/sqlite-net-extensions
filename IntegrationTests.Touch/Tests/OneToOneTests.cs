﻿using System;
using Cirrious.MvvmCross.Plugins.Sqlite;
using NUnit.Framework;
using SQLite;
using SQLiteNetExtensions.Attributes;
using SQLiteNetExtensions.Extensions;

#if USING_MVVMCROSS
using Cirrious.MvvmCross.Plugins.Sqlite;
#endif

namespace SQLiteNetExtensions.IntegrationTests
{
    public class O2OClassA
    {
        [ForeignKey(typeof(O2OClassB))]     // Explicit foreign key attribute
        public int OneClassBKey { get; set; }

        [OneToOne]
        public O2OClassB OneClassB { get; set; }
    }

    public class O2OClassB
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Foo { get; set; }
    }

    public class O2OClassC
    {
        [PrimaryKey, AutoIncrement]
        public int ClassId { get; set; }

        [OneToOne]                          // Foreign key can be declared in the other class
        public O2OClassD ElementD { get; set; }

        public string Bar { get; set; }
    }

    public class O2OClassD
    {
        [ForeignKey(typeof (O2OClassC))]    // Explicit foreign key attribute for a inverse relationship
        public int ObjectCKey { get; set; }

        public string Foo { get; set; }
    }

    public class O2OClassE
    {
        public int ObjectFKey { get; set; }

        [OneToOne("ObjectFKey")]        // Explicit foreign key declaration
        public O2OClassF ObjectF { get; set; }

        public string Foo { get; set; }
    }

    public class O2OClassF
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [OneToOne]      // Inverse relationship, doesn't need foreign key
        public O2OClassE ObjectE { get; set; }

        public string Bar { get; set; }
    }   

    [TestFixture]
    public class OneToOneTests
    {

        [Test]
        public void TestGetOneToOneDirect()
        {
            var conn = new SQLiteConnection("database");
            conn.DropTable<O2OClassA>();
            conn.DropTable<O2OClassB>();
            conn.CreateTable<O2OClassA>();
            conn.CreateTable<O2OClassB>();

            // Use standard SQLite-Net API to create a new relationship
            var objectB = new O2OClassB
                {
                    Foo = string.Format("Foo String {0}", new Random().Next(100))
                };
            conn.Insert(objectB);

            var objectA = new O2OClassA();
            conn.Insert(objectA);

            Assert.Null(objectA.OneClassB);

            // Fetch (yet empty) the relationship
            conn.GetChildren(ref objectA);
            Assert.Null(objectA.OneClassB);

            // Set the relationship using IDs
            objectA.OneClassBKey = objectB.Id;

            Assert.Null(objectA.OneClassB);

            // Fetch the relationship
            conn.GetChildren(ref objectA);

            Assert.NotNull(objectA.OneClassB);
            Assert.AreEqual(objectB.Id, objectA.OneClassB.Id);
            Assert.AreEqual(objectB.Foo, objectA.OneClassB.Foo);
        }

        [Test]
        public void TestGetOneToOneInverseForeignKey()
        {
            var conn = new SQLiteConnection("database");
            conn.DropTable<O2OClassC>();
            conn.DropTable<O2OClassD>();
            conn.CreateTable<O2OClassC>();
            conn.CreateTable<O2OClassD>();

            // Use standard SQLite-Net API to create a new relationship
            var objectC = new O2OClassC
            {
                Bar = string.Format("Bar String {0}", new Random().Next(100))
            };
            conn.Insert(objectC);

            Assert.Null(objectC.ElementD);

            // Fetch (yet empty) the relationship
            conn.GetChildren(ref objectC);

            Assert.Null(objectC.ElementD);

            var objectD = new O2OClassD
            {
                ObjectCKey = objectC.ClassId,
                Foo = string.Format("Foo String {0}", new Random().Next(100))
            };
            conn.Insert(objectD);

            Assert.Null(objectC.ElementD);

            // Fetch the relationship
            conn.GetChildren(ref objectC);

            Assert.NotNull(objectC.ElementD);
            Assert.AreEqual(objectC.ClassId, objectC.ElementD.ObjectCKey);
            Assert.AreEqual(objectD.Foo, objectC.ElementD.Foo);
        }

        [Test]
        public void TestGetOneToOneWithInverseRelationship()
        {
            var conn = new SQLiteConnection("database");
            conn.DropTable<O2OClassE>();
            conn.DropTable<O2OClassF>();
            conn.CreateTable<O2OClassE>();
            conn.CreateTable<O2OClassF>();

            // Use standard SQLite-Net API to create a new relationship
            var objectF = new O2OClassF
            {
                Bar = string.Format("Bar String {0}", new Random().Next(100))
            };
            conn.Insert(objectF);

            var objectE = new O2OClassE
                {
                    Foo = string.Format("Foo String {0}", new Random().Next(100))
                };
            conn.Insert(objectE);

            Assert.Null(objectE.ObjectF);

            // Fetch (yet empty) the relationship
            conn.GetChildren(ref objectE);
            Assert.Null(objectE.ObjectF);

            // Set the relationship using IDs
            objectE.ObjectFKey = objectF.Id;

            Assert.Null(objectE.ObjectF);

            // Fetch the relationship
            conn.GetChildren(ref objectE);

            Assert.NotNull(objectE.ObjectF);
            Assert.AreEqual(objectF.Id, objectE.ObjectF.Id);
            Assert.AreEqual(objectF.Bar, objectE.ObjectF.Bar);

            // Check the inverse relationship
            Assert.NotNull(objectE.ObjectF.ObjectE);
            Assert.AreEqual(objectE.Foo, objectE.ObjectF.ObjectE.Foo);
            Assert.AreSame(objectE, objectE.ObjectF.ObjectE);
        }
    }
}
