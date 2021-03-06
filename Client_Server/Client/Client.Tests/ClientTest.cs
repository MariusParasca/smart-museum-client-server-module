// <copyright file="ClientTest.cs">Copyright ©  2018</copyright>
using System;
using Client;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Client.Tests
{
    /// <summary>This class contains parameterized unit tests for Client</summary>
    [PexClass(typeof(global::Client.Client))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class ClientTest
    {
        /// <summary>Test stub for Main()</summary>
        [PexMethod]
        public void MainTest()
        {
            global::Client.Client.Main();

        }
    }
}
