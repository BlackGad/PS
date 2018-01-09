using System;
using NUnit.Framework;
using PS.Threading;

namespace PS.Tests.Threading
{
    [TestFixture]
    public class DelegateSwitcherTests
    {
        [Test]
        public void InvalidDelegateStructure_Construct_Failure()
        {
            // ReSharper disable ObjectCreationAsStatement
            Assert.Throws<InvalidCastException>(() => new DelegateSwitcher<object>());
            Assert.Throws<InvalidCastException>(() => new DelegateSwitcher<Delegate>());
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void Switcher_DelegateRegisterAndSwitch_Success()
        {
            var swithcer = new DelegateSwitcher<Func<int>>();
            Assert.IsNull(swithcer.Active);
            swithcer.RegisterAndSwitch(1, () => 1);
            Assert.IsNotNull(swithcer.Active);
            Assert.AreEqual(1, swithcer.Active());
        }

        [Test]
        public void Switcher_DelegateRegistration_Success()
        {
            var swithcer = new DelegateSwitcher<Func<int>>();
            swithcer.Register(1, () => 1);
            Assert.IsNull(swithcer.Active);
            Assert.AreEqual(1, swithcer[1]());
        }

        [Test]
        public void Switcher_DelegateRegistrationReplace_Success()
        {
            var swithcer = new DelegateSwitcher<Func<int>>();
            swithcer.Register(1, () => 1);
            swithcer.Register(1, () => 2);
            Assert.IsNull(swithcer.Active);
            Assert.AreEqual(2, swithcer[1]());
        }

        [Test]
        public void Switcher_DelegateSwitch_Success()
        {
            var swithcer = new DelegateSwitcher<Func<int>>();
            Assert.IsNull(swithcer.Active);
            swithcer.Register(1, () => 1);
            Assert.IsNull(swithcer.Active);
            swithcer.Switch(1);
            Assert.IsNotNull(swithcer.Active);
            Assert.AreEqual(1, swithcer.Active());
        }

        [Test]
        public void Switcher_SwitchingPredicate_Success()
        {
            var swithcer = new DelegateSwitcher<Func<int>>(key => !Equals(key, 3));
            swithcer.RegisterAndSwitch(1, () => 1);
            Assert.AreEqual(1, swithcer.Active());
            swithcer.RegisterAndSwitch(2, () => 2);
            Assert.AreEqual(2, swithcer.Active());
            swithcer.RegisterAndSwitch(3, () => 3);
            Assert.AreEqual(2, swithcer.Active());
        }

        [Test]
        public void Switcher_UnregisteredDeleagateUsage_Failure()
        {
            var swithcer = new DelegateSwitcher<Func<int>>();
            Assert.Throws<InvalidOperationException>(() => swithcer[1]());
        }

        [Test]
        public void ValidDelegateStructure_Construct_Success()
        {
            // ReSharper disable ObjectCreationAsStatement
            new DelegateSwitcher<Action>();
            new DelegateSwitcher<Action<int>>();
            new DelegateSwitcher<Func<int>>();
            new DelegateSwitcher<Func<int, int>>();
            // ReSharper restore ObjectCreationAsStatement
        }
    }
}