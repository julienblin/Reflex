// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EncryptionTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Utilities;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Core.Tests.Utilities
{
    public class EncryptionTest : BaseDbTest
    {
        [Test]
        public void It_should_encrypt_and_decrypt()
        {
            var value = Rand.String();
            var encrypted = Encryption.Encrypt(value);
            encrypted.Should().NotBe(value);

            var decrypted = Encryption.Decrypt(encrypted);
            decrypted.Should().Be(value);
        }

        [Test]
        public void It_should_hash()
        {
            var value = Rand.String();
            var hashed1 = Encryption.Hash(value);
            hashed1.Should().NotBe(value);

            var hashed2 = Encryption.Hash(value);
            hashed2.Should().Be(hashed1);
        }

        [Test]
        public void It_should_GenerateRandomToken()
        {
            const int Length = 50;
            var result = Encryption.GenerateRandomToken(Length);
            result.Length.Should().BeLessOrEqualTo(Length);
            result.Should().NotBeNullOrEmpty();
        }
    }
}
