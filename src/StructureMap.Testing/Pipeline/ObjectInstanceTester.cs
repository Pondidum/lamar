using Shouldly;
using StructureMap.Building;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget3;
using System;
using Xunit;

namespace StructureMap.Testing.Pipeline
{
    public class ObjectInstanceTester
    {
        public interface ITarget
        {
        }

        public class ATarget : ITarget
        {
            public override string ToString()
            {
                return "the description of ATarget";
            }
        }

        [Fact]
        public void to_dependency_source()
        {
            var gateway = new StubbedGateway();
            new ObjectInstance(gateway).ToDependencySource(typeof(IGateway))
                .ShouldBe(Constant.For<IGateway>(gateway));
        }

        [Fact]
        public void Build_happy_path()
        {
            var target = new ATarget();
            var instance = new ObjectInstance(target);

            instance.Build<ATarget>().ShouldBeTheSameAs(target);
        }

        [Fact]
        public void Create_description_should_return_the_ToString_of_the_inner_instance()
        {
            new ObjectInstance(this)
                .Description.ShouldBe("Object:  " + ToString());
        }

        [Fact]
        public void Throw_NullArgumentException_if_literal_instance_is_null()
        {
            Exception<ArgumentNullException>.ShouldBeThrownBy(() => { new ObjectInstance(null); });
        }

        [Fact]
        public void object_instance_is_a_singleton()
        {
            new ObjectInstance(new DisposableGuy()).Lifecycle.ShouldBeOfType<ObjectLifecycle>();
        }

        [Fact]
        public void object_is_not_disposed_in_nested_container()
        {
            var guy = new DisposableGuy();

            var container = new Container(x => { x.For<DisposableGuy>().Use(guy); });

            using (var nested = container.GetNestedContainer())
            {
            }

            guy.WasDisposed.ShouldBeFalse();

            container.Dispose();

            guy.WasDisposed.ShouldBeTrue();
        }
    }

    public class DisposableGuy : IDisposable
    {
        public bool WasDisposed;

        public void Dispose()
        {
            WasDisposed = true;
        }
    }
}